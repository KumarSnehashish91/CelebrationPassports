using CelebrationPassports.Infrastructure.Authentication.Configuration;
using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Infrastructure.Authentication.Services;
using CelebrationPassports.Infrastructure.Storage.Interfaces;
using CelebrationPassports.Infrastructure.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Infrastructure.Authentication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddScoped<ITokenService, JwtTokenService>();

            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}
