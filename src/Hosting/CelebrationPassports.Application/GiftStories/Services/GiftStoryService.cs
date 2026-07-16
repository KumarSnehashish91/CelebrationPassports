using System.Text.Json;
using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Gifting.DTOs;
using CelebrationPassports.Application.Gifting.Interfaces;
using CelebrationPassports.Application.GiftStories.DTOs;
using CelebrationPassports.Application.GiftStories.Interfaces;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Infrastructure.AI.Interfaces;
using CelebrationPassports.Infrastructure.Storage.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CelebrationPassports.Application.GiftStories.Services;

public class GiftStoryService : IGiftStoryService
{
    private const int MinPhotos = 4;
    private const int MaxPhotos = 30;
    private const long MaxFileSizeBytes = 25 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".heic", ".gif" };

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    // ai-model-strategy.md, Section 2: no fine-tuning yet — anchor style with a system
    // prompt + a few strong examples instead. The failure mode to avoid is generic
    // "this is a lovely photo" filler; examples show the specific, concrete alternative.
    // NumPredict/Temperature (see the call site) do real work here too: capping output
    // length is the main lever on response time for a small local vision model, and a
    // slightly higher temperature pushes away from the safe/generic default completion.
    private const string InsightPrompt =
        "You are writing a one-sentence caption for a family keepsake photo album. Look closely " +
        "at this photo and describe the specific, concrete things you can actually see — people's " +
        "expressions, what they're doing, the setting, objects, weather, colors. " +
        "Do not use any of these generic phrases or anything like them: \"this is a photo of\", " +
        "\"this is a lovely/wonderful/nice photo\", \"a beautiful moment\", \"capturing a moment\". " +
        "If you don't have a specific detail, look again rather than filling in with a generic phrase.\n\n" +
        "Examples of the style to match:\n" +
        "- \"Two kids mid-laugh on a wooden dock, one pointing at something off-frame, the lake " +
        "glowing orange behind them at sunset.\"\n" +
        "- \"A birthday cake with five lit candles, a small hand reaching toward it, party hats " +
        "scattered across the table.\"\n" +
        "- \"An older couple slow-dancing barefoot on grass, string lights strung between the trees " +
        "above them.\"\n\n" +
        "Now write ONE sentence like that for this photo. Under 25 words. Respond with only the " +
        "sentence — no preamble, no quotation marks.";

    // ~25 words is roughly 35-45 tokens for English; capped higher than that purely as a
    // hard ceiling against rambling, not the target length itself.
    private const int InsightMaxTokens = 60;
    private const double InsightTemperature = 1.0;

    private readonly IGenericRepository<GiftDraft> _draftRepository;
    private readonly IGenericRepository<GiftPhoto> _photoRepository;
    private readonly IGenericRepository<GeneratedStory> _storyRepository;
    private readonly IGenericRepository<PassportGift> _giftRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICelebrationAIService _aiService;
    private readonly IPassportGiftService _passportGiftService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GiftStoryService> _logger;

    public GiftStoryService(
        IGenericRepository<GiftDraft> draftRepository,
        IGenericRepository<GiftPhoto> photoRepository,
        IGenericRepository<GeneratedStory> storyRepository,
        IGenericRepository<PassportGift> giftRepository,
        IFileStorageService fileStorageService,
        ICelebrationAIService aiService,
        IPassportGiftService passportGiftService,
        IUnitOfWork unitOfWork,
        ILogger<GiftStoryService> logger)
    {
        _draftRepository = draftRepository;
        _photoRepository = photoRepository;
        _storyRepository = storyRepository;
        _giftRepository = giftRepository;
        _fileStorageService = fileStorageService;
        _aiService = aiService;
        _passportGiftService = passportGiftService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GiftDraftDto> StartAsync(Guid senderUserId, PurchaseGiftRequest recipientInfo)
    {
        if (string.IsNullOrWhiteSpace(recipientInfo.RecipientName))
        {
            throw new ValidationException("Recipient name is required.");
        }

        var draft = new GiftDraft
        {
            Id = Guid.NewGuid(),
            SenderUserId = senderUserId,
            RecipientName = recipientInfo.RecipientName.Trim(),
            RecipientEmail = string.IsNullOrWhiteSpace(recipientInfo.RecipientEmail) ? null : recipientInfo.RecipientEmail.Trim(),
            PassportTitle = string.IsNullOrWhiteSpace(recipientInfo.PassportTitle) ? null : recipientInfo.PassportTitle.Trim(),
            PersonalMessage = string.IsNullOrWhiteSpace(recipientInfo.GiftMessage) ? null : recipientInfo.GiftMessage.Trim(),
            Status = GiftDraftStatus.DraftPhotos,
            CreatedOn = DateTime.UtcNow
        };

        await _draftRepository.AddAsync(draft);
        await _unitOfWork.SaveChangesAsync();

        return await MapDraftAsync(draft);
    }

    public async Task<GiftDraftDto?> GetAsync(Guid senderUserId, Guid draftId)
    {
        var draft = await _draftRepository.GetByIdAsync(draftId);

        if (draft is null || draft.SenderUserId != senderUserId)
        {
            return null;
        }

        return await MapDraftAsync(draft);
    }

    public async Task<GiftPhotoDto> AddPhotoAsync(Guid senderUserId, Guid draftId, FileUploadRequest file)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);

        if (file.Length <= 0)
        {
            throw new ValidationException("The uploaded file is empty.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new ValidationException("The uploaded file exceeds the 25 MB limit.");
        }

        if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName)))
        {
            throw new ValidationException("Only JPG, PNG, WEBP, HEIC, or GIF photos are supported.");
        }

        var existing = await _photoRepository.FindAsync(p => p.GiftDraftId == draftId);

        if (existing.Count >= MaxPhotos)
        {
            throw new ValidationException($"You can add up to {MaxPhotos} photos.");
        }

        var url = await _fileStorageService.SaveAsync(file.Content, file.FileName);

        var photo = new GiftPhoto
        {
            Id = Guid.NewGuid(),
            GiftDraftId = draft.Id,
            Url = url,
            DisplayOrder = existing.Count,
            CreatedOn = DateTime.UtcNow
        };

        await _photoRepository.AddAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return MapPhoto(photo);
    }

    public async Task RemovePhotoAsync(Guid senderUserId, Guid photoId)
    {
        var photo = await _photoRepository.GetByIdAsync(photoId)
            ?? throw new NotFoundException("Photo not found.");

        await GetOwnedDraftAsync(senderUserId, photo.GiftDraftId);

        _photoRepository.Remove(photo);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GiftDraftDto> ContinueToInsightsAsync(Guid senderUserId, Guid draftId)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);
        var photos = await _photoRepository.FindAsync(p => p.GiftDraftId == draftId);

        if (photos.Count < MinPhotos)
        {
            throw new ValidationException($"Add at least {MinPhotos} photos before continuing.");
        }

        if (draft.Status == GiftDraftStatus.DraftPhotos)
        {
            draft.Status = GiftDraftStatus.DraftInsights;
            await _unitOfWork.SaveChangesAsync();
        }

        return await MapDraftAsync(draft);
    }

    public async Task SetInsightsAsync(Guid senderUserId, Guid draftId, Dictionary<Guid, string?> insightsByPhotoId)
    {
        await GetOwnedDraftAsync(senderUserId, draftId);

        var photos = await _photoRepository.FindAsync(p => p.GiftDraftId == draftId);

        foreach (var photo in photos)
        {
            if (insightsByPhotoId.TryGetValue(photo.Id, out var text))
            {
                photo.UserInsight = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GeneratedStoryDto> GenerateStoryAsync(Guid senderUserId, Guid draftId)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);
        var photos = (await _photoRepository.FindAsync(p => p.GiftDraftId == draftId))
            .OrderBy(p => p.DisplayOrder)
            .ToList();

        if (photos.Count < MinPhotos)
        {
            throw new ValidationException($"Add at least {MinPhotos} photos before generating a story.");
        }

        await FillBlankInsightsAsync(photos);

        var story = await BuildNarrativeAsync(draft, photos);

        if (draft.Status is GiftDraftStatus.DraftPhotos or GiftDraftStatus.DraftInsights)
        {
            draft.Status = GiftDraftStatus.DraftStory;
        }

        await _unitOfWork.SaveChangesAsync();

        return MapStory(story);
    }

    public async Task<GeneratedStoryDto> RegenerateStoryAsync(Guid senderUserId, Guid draftId)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);
        var photos = (await _photoRepository.FindAsync(p => p.GiftDraftId == draftId))
            .OrderBy(p => p.DisplayOrder)
            .ToList();

        // Regenerate only redoes the narrative — insights (AI or user) are already
        // settled by this point and are never re-requested from the vision model.
        var story = await BuildNarrativeAsync(draft, photos);

        await _unitOfWork.SaveChangesAsync();

        return MapStory(story);
    }

    public async Task<GeneratedStoryDto?> GetStoryAsync(Guid senderUserId, Guid draftId)
    {
        await GetOwnedDraftAsync(senderUserId, draftId);

        var story = await _storyRepository.FirstOrDefaultAsync(s => s.GiftDraftId == draftId);

        return story is null ? null : MapStory(story);
    }

    public async Task<PrintPreviewDto> GetPrintPreviewAsync(Guid senderUserId, Guid draftId, PrintFormat format)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);
        var photos = (await _photoRepository.FindAsync(p => p.GiftDraftId == draftId))
            .OrderBy(p => p.DisplayOrder)
            .ToList();

        var story = await _storyRepository.FirstOrDefaultAsync(s => s.GiftDraftId == draftId)
            ?? throw new ValidationException("Generate the story before previewing the print layout.");

        if (draft.Status is GiftDraftStatus.DraftPhotos or GiftDraftStatus.DraftInsights or GiftDraftStatus.DraftStory)
        {
            draft.Status = GiftDraftStatus.DraftPrint;
        }

        draft.PrintFormat = format;
        await _unitOfWork.SaveChangesAsync();

        var (label, trimSize, price) = format == PrintFormat.LifeBook
            ? ("The Life Book", "190mm × 190mm", 2499m)
            : ("Passport Edition", "297mm × 210mm", 3499m);

        var pages = photos.Select((photo, index) =>
        {
            var text = photo.UserInsight ?? photo.AiGeneratedInsight ?? "A cherished moment.";

            return new PrintPageDto
            {
                PageNumber = index + 1,
                Title = ExcerptTitle(text),
                PhotoUrl = photo.Url,
                Text = text
            };
        }).ToList();

        return new PrintPreviewDto
        {
            Format = format.ToString(),
            FormatLabel = label,
            TrimSize = trimSize,
            PageCount = 4 + photos.Count,
            Price = price,
            CoverTitle = story.Title,
            CoverSubtitle = $"A Gift for {draft.RecipientName} · {DateTime.UtcNow.Year}",
            Pages = pages
        };
    }

    public async Task<PassportGiftDto> FinalizeAsync(Guid senderUserId, Guid draftId, PrintFormat format)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);

        var hasStory = await _storyRepository.ExistsAsync(s => s.GiftDraftId == draftId);

        if (!hasStory)
        {
            throw new ValidationException("Generate the story before finalizing this gift.");
        }

        var gift = await _passportGiftService.PurchaseAsync(senderUserId, new PurchaseGiftRequest
        {
            RecipientName = draft.RecipientName,
            RecipientEmail = draft.RecipientEmail,
            GiftMessage = draft.PersonalMessage,
            PassportTitle = draft.PassportTitle
        });

        var giftEntity = await _giftRepository.GetByIdAsync(gift.Id)
            ?? throw new NotFoundException("Gift not found.");

        var price = format == PrintFormat.LifeBook ? 2499m : 3499m;
        giftEntity.Amount = price;
        giftEntity.MessageType = draft.MessageType;
        giftEntity.MessageMediaUrl = draft.MessageMediaUrl;
        giftEntity.DeliveryMode = draft.DeliveryMode;
        giftEntity.ScheduledDeliveryDate = draft.ScheduledDeliveryDate;

        draft.PassportGiftId = gift.Id;
        draft.PrintFormat = format;
        draft.Status = GiftDraftStatus.Sent;

        await _unitOfWork.SaveChangesAsync();

        gift.Amount = price;
        gift.DeliveryMode = draft.DeliveryMode.ToString();
        gift.ScheduledDeliveryDate = draft.ScheduledDeliveryDate;
        return gift;
    }

    public async Task<GiftDraftDto> SetMessageAsync(
        Guid senderUserId, Guid draftId, GiftMessageType messageType, FileUploadRequest? mediaFile, string? writtenText)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);

        switch (messageType)
        {
            case GiftMessageType.Written:
                draft.PersonalMessage = string.IsNullOrWhiteSpace(writtenText) ? null : writtenText.Trim();
                draft.MessageMediaUrl = null;
                break;

            case GiftMessageType.Voice or GiftMessageType.Video:
                if (mediaFile is null || mediaFile.Length <= 0)
                {
                    throw new ValidationException("Choose a recording or file to upload.");
                }

                if (mediaFile.Length > MaxFileSizeBytes)
                {
                    throw new ValidationException("The uploaded file exceeds the 25 MB limit.");
                }

                draft.MessageMediaUrl = await _fileStorageService.SaveAsync(mediaFile.Content, mediaFile.FileName);
                break;
        }

        draft.MessageType = messageType;

        if (draft.Status == GiftDraftStatus.DraftPrint)
        {
            draft.Status = GiftDraftStatus.DraftMessage;
        }

        await _unitOfWork.SaveChangesAsync();

        return await MapDraftAsync(draft);
    }

    public async Task<GiftDraftDto> SetDeliveryScheduleAsync(Guid senderUserId, Guid draftId, GiftDeliveryMode mode, DateTime? scheduledDate)
    {
        var draft = await GetOwnedDraftAsync(senderUserId, draftId);

        if (mode == GiftDeliveryMode.Scheduled && !scheduledDate.HasValue)
        {
            throw new ValidationException("Choose a delivery date.");
        }

        draft.DeliveryMode = mode;
        draft.ScheduledDeliveryDate = mode == GiftDeliveryMode.Scheduled ? scheduledDate : null;

        await _unitOfWork.SaveChangesAsync();

        return await MapDraftAsync(draft);
    }

    private async Task FillBlankInsightsAsync(List<GiftPhoto> photos)
    {
        foreach (var photo in photos)
        {
            if (!string.IsNullOrWhiteSpace(photo.UserInsight) || !string.IsNullOrWhiteSpace(photo.AiGeneratedInsight))
            {
                continue;
            }

            try
            {
                var bytes = await _fileStorageService.ReadAsync(photo.Url);
                var insight = await _aiService.GenerateWithImageAsync(InsightPrompt, bytes, InsightMaxTokens, InsightTemperature);

                photo.AiGeneratedInsight = string.IsNullOrWhiteSpace(insight)
                    ? "A moment worth remembering."
                    : Truncate(insight.Trim().Trim('"'), 1000);
            }
            catch (Exception ex)
            {
                // Vision call failed (model unavailable, bad image, etc.) — never block
                // the whole story over one photo. Same "degrade, don't crash" approach
                // as PhotoMetadataService/GooglePhotosImportParser.
                _logger.LogWarning(ex, "Falling back to a generic insight for gift photo {PhotoId}.", photo.Id);
                photo.AiGeneratedInsight = "A moment worth remembering.";
            }
        }
    }

    private async Task<GeneratedStory> BuildNarrativeAsync(GiftDraft draft, List<GiftPhoto> photos)
    {
        var effective = photos.Select(p => new
        {
            Photo = p,
            Text = p.UserInsight ?? p.AiGeneratedInsight ?? "A cherished moment.",
            Origin = p.UserInsight is not null ? ParagraphOrigin.User : ParagraphOrigin.Ai
        }).ToList();

        var pullQuoteSource = effective.FirstOrDefault(e => e.Origin == ParagraphOrigin.User) ?? effective.First();

        var bodyParagraphs = effective
            .Where(e => e.Photo.Id != pullQuoteSource.Photo.Id)
            .Select(e => new StoryParagraphDto
            {
                Text = e.Text,
                Origin = e.Origin.ToString(),
                SourcePhotoId = e.Photo.Id
            })
            .ToList();

        var insightList = string.Join("\n", effective.Select((e, i) => $"{i + 1}. {e.Text}"));

        var (title, opening) = await GenerateTitleAndOpeningAsync(draft.RecipientName, insightList);
        var closing = await GenerateClosingAsync(draft.RecipientName, insightList);

        var existing = await _storyRepository.FirstOrDefaultAsync(s => s.GiftDraftId == draft.Id);

        var story = existing ?? new GeneratedStory
        {
            Id = Guid.NewGuid(),
            GiftDraftId = draft.Id
        };

        story.Title = Truncate(title, 200);
        story.OpeningParagraph = opening;
        story.ClosingParagraph = closing;
        story.PullQuoteText = Truncate(pullQuoteSource.Text, 500);
        story.PullQuoteOrigin = pullQuoteSource.Origin;
        story.BodyParagraphsJson = JsonSerializer.Serialize(bodyParagraphs, JsonOptions);
        story.GeneratedAt = DateTime.UtcNow;
        story.RegenerationCount = existing is null ? 0 : existing.RegenerationCount + 1;

        if (existing is null)
        {
            await _storyRepository.AddAsync(story);
        }

        return story;
    }

    private async Task<(string Title, string Opening)> GenerateTitleAndOpeningAsync(string recipientName, string insightList)
    {
        var prompt =
            $"You are a warm memoir writer creating a personalized gift story about {recipientName}, " +
            $"based on these real moments described by the people who love them:\n\n{insightList}\n\n" +
            "Example of the tone and specificity to match (do not reuse this content, it's a style " +
            "reference only):\n" +
            "TITLE: The Summer of Small Adventures\n" +
            "OPENING: Some people spend their whole lives chasing wonder. Priya found it before " +
            "breakfast most days — in a garden, on a bicycle, in the space of one ordinary afternoon " +
            "that somehow became a memory worth keeping.\n\n" +
            "Now write your own, about the real moments listed above. Respond in exactly this format, " +
            "nothing else:\n" +
            "TITLE: <a short, warm headline for this story, under 8 words>\n" +
            $"OPENING: <a short opening paragraph, 2-3 sentences, that sets a warm personal tone for a story about {recipientName} — an introduction, not a retelling of the moments above>";

        string raw;

        try
        {
            raw = await _aiService.GenerateAsync(prompt, maxTokens: 150);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Gift story title/opening generation failed — using fallback text.");
            return ("A Story Worth Keeping", $"Every so often, someone reminds us what it means to be truly present. This is {recipientName}'s story.");
        }

        var titleLine = ExtractLine(raw, "TITLE:");
        var openingLine = ExtractLine(raw, "OPENING:");

        var title = string.IsNullOrWhiteSpace(titleLine) ? "A Story Worth Keeping" : titleLine;
        var opening = string.IsNullOrWhiteSpace(openingLine) ? raw.Trim() : openingLine;

        return (title, opening);
    }

    private async Task<string> GenerateClosingAsync(string recipientName, string insightList)
    {
        var prompt =
            $"You are a warm memoir writer. Given these real moments about {recipientName}:\n\n{insightList}\n\n" +
            "Example of the tone to match (style reference only, do not reuse this content): " +
            "\"There's a particular kind of joy that only shows up in moments like these — unguarded, " +
            "a little loud, entirely certain that this is exactly where you're meant to be.\"\n\n" +
            "Now write your own short closing paragraph (2-3 sentences) that reflects warmly on the " +
            "real moments listed above as a whole, without repeating them word-for-word — write it as " +
            "the final paragraph of a heartfelt gift story. Respond with ONLY the paragraph, no " +
            "preamble, no headers.";

        try
        {
            var raw = await _aiService.GenerateAsync(prompt, maxTokens: 100);

            return string.IsNullOrWhiteSpace(raw)
                ? $"These are just a few of the moments that make {recipientName} who they are — and there are so many more still to come."
                : raw.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Gift story closing generation failed — using fallback text.");
            return $"These are just a few of the moments that make {recipientName} who they are — and there are so many more still to come.";
        }
    }

    private static string ExtractLine(string raw, string marker)
    {
        var line = raw
            .Split('\n')
            .Select(l => l.Trim())
            .FirstOrDefault(l => l.StartsWith(marker, StringComparison.OrdinalIgnoreCase));

        return line is null ? string.Empty : line[marker.Length..].Trim();
    }

    private static string ExcerptTitle(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var excerpt = string.Join(' ', words.Take(5));

        return words.Length > 5 ? excerpt + "…" : excerpt;
    }

    // The local model doesn't reliably obey the length asked for in the prompt (its own
    // vision responses have come back well past the requested word count) — this is a
    // hard backstop against DB column limits (GiftPhoto.AiGeneratedInsight,
    // GeneratedStory.Title/PullQuoteText), not a substitute for the prompt's own guidance.
    private static string Truncate(string text, int maxLength) =>
        text.Length <= maxLength ? text : text[..maxLength];

    private async Task<GiftDraft> GetOwnedDraftAsync(Guid senderUserId, Guid draftId)
    {
        var draft = await _draftRepository.GetByIdAsync(draftId)
            ?? throw new NotFoundException("Gift draft not found.");

        if (draft.SenderUserId != senderUserId)
        {
            throw new ForbiddenAccessException("You do not have access to this gift draft.");
        }

        return draft;
    }

    private async Task<GiftDraftDto> MapDraftAsync(GiftDraft draft)
    {
        var photos = (await _photoRepository.FindAsync(p => p.GiftDraftId == draft.Id))
            .OrderBy(p => p.DisplayOrder)
            .Select(MapPhoto)
            .ToList();

        return new GiftDraftDto
        {
            Id = draft.Id,
            RecipientName = draft.RecipientName,
            RecipientEmail = draft.RecipientEmail,
            PassportTitle = draft.PassportTitle,
            PersonalMessage = draft.PersonalMessage,
            Status = draft.Status.ToString(),
            PrintFormat = draft.PrintFormat?.ToString(),
            MessageType = draft.MessageType?.ToString(),
            MessageMediaUrl = draft.MessageMediaUrl,
            DeliveryMode = draft.DeliveryMode.ToString(),
            ScheduledDeliveryDate = draft.ScheduledDeliveryDate,
            Photos = photos
        };
    }

    private static GiftPhotoDto MapPhoto(GiftPhoto photo) => new()
    {
        Id = photo.Id,
        Url = photo.Url,
        UserInsight = photo.UserInsight,
        AiGeneratedInsight = photo.AiGeneratedInsight,
        DisplayOrder = photo.DisplayOrder
    };

    private static GeneratedStoryDto MapStory(GeneratedStory story)
    {
        var bodyParagraphs = JsonSerializer.Deserialize<List<StoryParagraphDto>>(story.BodyParagraphsJson, JsonOptions) ?? [];

        return new GeneratedStoryDto
        {
            Title = story.Title,
            OpeningParagraph = story.OpeningParagraph,
            ClosingParagraph = story.ClosingParagraph,
            PullQuoteText = story.PullQuoteText,
            PullQuoteOrigin = story.PullQuoteOrigin?.ToString(),
            BodyParagraphs = bodyParagraphs,
            RegenerationCount = story.RegenerationCount
        };
    }
}
