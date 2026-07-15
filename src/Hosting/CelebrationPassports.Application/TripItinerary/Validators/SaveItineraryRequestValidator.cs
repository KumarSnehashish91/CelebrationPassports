using CelebrationPassports.Application.TripItinerary.DTOs;
using FluentValidation;

namespace CelebrationPassports.Application.TripItinerary.Validators;

public class SaveItineraryRequestValidator : AbstractValidator<SaveItineraryRequest>
{
    public SaveItineraryRequestValidator()
    {
        RuleFor(x => x.Days)
            .NotEmpty()
            .WithMessage("At least one itinerary day is required.");

        RuleForEach(x => x.Days).ChildRules(day =>
        {
            day.RuleFor(d => d.DayNumber).GreaterThan(0);
            day.RuleFor(d => d.Title).NotEmpty().MaximumLength(200);
            day.RuleFor(d => d.Description).NotEmpty().MaximumLength(1000);
        });
    }
}
