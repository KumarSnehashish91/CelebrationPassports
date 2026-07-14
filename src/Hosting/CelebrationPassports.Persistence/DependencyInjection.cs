using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Repositories.Implementations;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CelebrationPassports.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CelebrationPassportsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IPassportRepository, PassportRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IStoryRepository, StoryRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<IPassportInvitationRepository, PassportInvitationRepository>();
        services.AddScoped<IPlaceRepository, PlaceRepository>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}