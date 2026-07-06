using CelebrationPassports.Application.Features.Users.Interfaces;
using CelebrationPassports.Application.Features.Users.Services;
using CelebrationPassports.Application.Features.Authentication.Interfaces;
using CelebrationPassports.Application.Features.Authentication.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

        return services;
    }
}