using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using CelebrationPassports.Application.Authentication.Interfaces;
using CelebrationPassports.Application.Authentication.Services;
using CelebrationPassports.Application.Users.Interfaces;
using CelebrationPassports.Application.Users.Services;

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
        


        return services;
    }
}