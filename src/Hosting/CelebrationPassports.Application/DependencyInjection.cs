using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using CelebrationPassports.Application.Authentication.Interfaces;
using CelebrationPassports.Application.Authentication.Services;
using CelebrationPassports.Application.Users.Interfaces;
using CelebrationPassports.Application.Users.Services;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Passports.Services;
using CelebrationPassports.Application.Events.Interfaces;
using CelebrationPassports.Application.Events.Services;
using CelebrationPassports.Application.Stories.Interfaces;
using CelebrationPassports.Application.Stories.Services;
using CelebrationPassports.Application.Media.Interfaces;
using CelebrationPassports.Application.Media.Services;
using CelebrationPassports.Application.Invitations.Interfaces;
using CelebrationPassports.Application.Invitations.Services;
using CelebrationPassports.Application.Stamps.Interfaces;
using CelebrationPassports.Application.Stamps.Services;
using CelebrationPassports.Application.Places.Interfaces;
using CelebrationPassports.Application.Places.Services;
using CelebrationPassports.Application.Categories.Interfaces;
using CelebrationPassports.Application.Categories.Services;
using CelebrationPassports.Application.Notifications.Interfaces;
using CelebrationPassports.Application.Notifications.Services;
using CelebrationPassports.Application.Someday.Interfaces;
using CelebrationPassports.Application.Someday.Services;
using CelebrationPassports.Application.Calendar.Interfaces;
using CelebrationPassports.Application.Calendar.Services;
using CelebrationPassports.Application.TimeCapsule.Interfaces;
using CelebrationPassports.Application.TimeCapsule.Services;
using CelebrationPassports.Application.Wishes.Interfaces;
using CelebrationPassports.Application.Wishes.Services;
using CelebrationPassports.Application.Expenses.Interfaces;
using CelebrationPassports.Application.Expenses.Services;
using CelebrationPassports.Application.TripItinerary.Interfaces;
using CelebrationPassports.Application.TripItinerary.Services;
using CelebrationPassports.Application.Dashboard.Interfaces;
using CelebrationPassports.Application.Dashboard.Services;

namespace CelebrationPassports.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });



        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IuserProfileService, UserProfileService>();
        services.AddScoped<IPassportAccessGuard, PassportAccessGuard>();
        services.AddScoped<IPassportService, PassportService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IStoryService, StoryService>();
        services.AddScoped<IChapterService, ChapterService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IPassportInvitationService, PassportInvitationService>();
        services.AddScoped<IPassportStampService, PassportStampService>();
        services.AddScoped<IPlaceService, PlaceService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ITripDetectionService, TripDetectionService>();
        services.AddScoped<IMediaClusteringService, MediaClusteringService>();
        services.AddScoped<IAutoChapterClusteringService, AutoChapterClusteringService>();
        services.AddScoped<ISomedayIdeaService, SomedayIdeaService>();
        services.AddScoped<ICalendarFeedTokenService, CalendarFeedTokenService>();
        services.AddScoped<ICalendarFeedService, CalendarFeedService>();
        services.AddScoped<ITimeCapsuleMessageService, TimeCapsuleMessageService>();
        services.AddScoped<ITimeCapsuleUnlockService, TimeCapsuleUnlockService>();
        services.AddScoped<IWishService, WishService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ITripItineraryService, TripItineraryService>();
        services.AddScoped<IDashboardStatsService, DashboardStatsService>();


        return services;
    }
}