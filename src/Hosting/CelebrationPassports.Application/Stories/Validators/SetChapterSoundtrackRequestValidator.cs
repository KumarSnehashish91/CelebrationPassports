using CelebrationPassports.Application.Stories.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.Stories.Validators;

public class SetChapterSoundtrackRequestValidator : AbstractValidator<SetChapterSoundtrackRequest>
{
    public SetChapterSoundtrackRequestValidator()
    {
        RuleFor(x => x.SongTitle)
            .MaximumLength(200);

        RuleFor(x => x.SongArtist)
            .MaximumLength(200);

        RuleFor(x => x.SongLinkUrl)
            .MaximumLength(500)
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("The link must be a valid http:// or https:// URL.");
    }
}
