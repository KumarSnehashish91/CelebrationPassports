using CelebrationPassports.Infrastructure.Authentication.Configuration;
using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Infrastructure.Authentication.Services;
using CelebrationPassports.Infrastructure.Storage.Interfaces;
using CelebrationPassports.Infrastructure.Storage.Services;
using CelebrationPassports.Infrastructure.Photo.Interfaces;
using CelebrationPassports.Infrastructure.Photo.Services;
using CelebrationPassports.Infrastructure.Imports.Interfaces;
using CelebrationPassports.Infrastructure.Imports.Services;
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
            services.AddScoped<IPhotoMetadataService, PhotoMetadataService>();
            services.AddScoped<IImportArchiveStorage, LocalImportArchiveStorage>();
            services.AddScoped<IGooglePhotosImportParser, GooglePhotosImportParser>();

            return services;
        }
    }
}
