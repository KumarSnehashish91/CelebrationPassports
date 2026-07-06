using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Infrastructure.Authentication.Services;
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
            return services;
        }
    }
}
