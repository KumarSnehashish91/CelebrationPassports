using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Stories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class StoriesController : Controller
{
    private readonly IStoryService _storyService;
    private readonly IPassportService _passportService;
    private readonly ICategoryService _categoryService;
    private readonly IMediaService _mediaService;

    public StoriesController(
        IStoryService storyService,
        IPassportService passportService,
        ICategoryService categoryService,
        IMediaService mediaService)
    {
        _storyService = storyService;
        _passportService = passportService;
        _categoryService = categoryService;
        _mediaService = mediaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var stories = await _storyService.GetMineAsync();
        ViewData["PendingDrafts"] = await _storyService.GetDraftsAsync();
        return View(stories);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var passports = await _passportService.GetMineAsync();
        var passportId = passports.FirstOrDefault()?.Id;

        if (passportId is null)
        {
            return RedirectToAction("Create", "Passports");
        }

        return View(new CreateStoryViewModel { PassportId = passportId.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var storyId = await _storyService.CreateAsync(model);

        if (storyId is null)
        {
            ModelState.AddModelError("", "Could not create the story. Please try again.");
            return View(model);
        }

        return RedirectToAction("Details", new { id = storyId });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var story = await _storyService.GetByIdAsync(id);

        if (story is null)
        {
            return RedirectToAction("Index");
        }

        return View(story);
    }

    [HttpGet]
    public async Task<IActionResult> AddChapter(Guid storyId)
    {
        var story = await _storyService.GetByIdAsync(storyId);

        if (story is null)
        {
            return RedirectToAction("Index");
        }

        var model = new CreateChapterViewModel
        {
            StoryId = storyId,
            StoryTitle = story.Title,
            EventDate = DateOnly.FromDateTime(DateTime.Today),
            Categories = await _categoryService.ListAllAsync()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddChapter(CreateChapterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await _categoryService.ListAllAsync();
            return View(model);
        }

        var chapterId = await _storyService.AddChapterAsync(model);

        if (chapterId is null)
        {
            ModelState.AddModelError("", "Could not create the chapter. Please try again.");
            model.Categories = await _categoryService.ListAllAsync();
            return View(model);
        }

        return RedirectToAction("Chapter", new { id = chapterId });
    }

    [HttpGet]
    public async Task<IActionResult> Chapter(Guid id)
    {
        var chapter = await _storyService.GetChapterByIdAsync(id);

        if (chapter is null)
        {
            return RedirectToAction("Index");
        }

        return View(chapter);
    }

    [HttpPost]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> UploadMedia(Guid chapterId, List<IFormFile> files)
    {
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                await _mediaService.UploadToChapterAsync(chapterId, file);
            }
        }

        return RedirectToAction("Chapter", new { id = chapterId });
    }

    // ---------- Quick Upload (feeds AI trip detection) ----------

    [HttpGet]
    public IActionResult QuickUpload()
    {
        return View();
    }

    [HttpPost]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async Task<IActionResult> QuickUpload(List<IFormFile> files)
    {
        var mediaIds = new List<Guid>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var mediaId = await _mediaService.UploadAsync(file);

                if (mediaId.HasValue)
                {
                    mediaIds.Add(mediaId.Value);
                }
            }
        }

        if (mediaIds.Count == 0)
        {
            ViewData["UploadError"] = "Nothing uploaded — please choose at least one photo.";
            return View();
        }

        var detectedChapterId = await _storyService.DetectTripAsync(mediaIds);

        if (detectedChapterId.HasValue)
        {
            return RedirectToAction("ReviewChapter", new { id = detectedChapterId });
        }

        ViewData["UploadedCount"] = mediaIds.Count;
        ViewData["NoTripDetected"] = true;
        return View();
    }

    // ---------- Review a Draft (AI-detected) Chapter ----------

    [HttpGet]
    public async Task<IActionResult> ReviewChapter(Guid id)
    {
        var chapter = await _storyService.GetChapterByIdAsync(id);

        if (chapter is null)
        {
            return RedirectToAction("Index");
        }

        if (chapter.Status != 1) // ChapterStatus.Draft
        {
            return RedirectToAction("Chapter", new { id });
        }

        var stories = await _storyService.GetMineAsync();

        var model = new ConfirmChapterViewModel
        {
            ChapterId = chapter.Id,
            PlaceId = chapter.PlaceId,
            Title = chapter.Title,
            CategoryId = chapter.CategoryId,
            EventDate = chapter.EventDate,
            Categories = await _categoryService.ListAllAsync(),
            ExistingStories = stories
        };

        ViewData["Chapter"] = chapter;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ReviewChapter(ConfirmChapterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var chapter = await _storyService.GetChapterByIdAsync(model.ChapterId);
            model.Categories = await _categoryService.ListAllAsync();
            model.ExistingStories = await _storyService.GetMineAsync();
            ViewData["Chapter"] = chapter;
            return View(model);
        }

        var confirmed = await _storyService.ConfirmDraftAsync(model);

        if (!confirmed)
        {
            ModelState.AddModelError("", "Could not confirm this chapter. Please try again.");
            var chapter = await _storyService.GetChapterByIdAsync(model.ChapterId);
            model.Categories = await _categoryService.ListAllAsync();
            model.ExistingStories = await _storyService.GetMineAsync();
            ViewData["Chapter"] = chapter;
            return View(model);
        }

        return RedirectToAction("Chapter", new { id = model.ChapterId });
    }

    [HttpPost]
    public async Task<IActionResult> DiscardChapter(Guid chapterId)
    {
        await _storyService.DiscardDraftAsync(chapterId);
        return RedirectToAction("Index");
    }
}
