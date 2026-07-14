using CelebrationPassports.API.Middlewares;
using CelebrationPassports.Application;
using CelebrationPassports.Infrastructure.AI.Clients;
using CelebrationPassports.Infrastructure.AI.Configuration;
using CelebrationPassports.Infrastructure.Authentication;
using CelebrationPassports.Infrastructure.Authentication.Configuration;
using CelebrationPassports.Persistence;

using CelebrationPassports.Infrastructure.AI.Clients;
using CelebrationPassports.Infrastructure.AI.Configuration;
using CelebrationPassports.Infrastructure.AI.Services;
using CelebrationPassports.Infrastructure.AI.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// WebRootFileProvider binds to a NullFileProvider (and never re-checks the disk) if
// wwwroot doesn't exist at host-build time — so this has to run before CreateBuilder.
Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AIOptions>(
    builder.Configuration.GetSection("CelebrationAI"));

builder.Services.AddHttpClient<AIClient>();

builder.Services.AddScoped<ICelebrationAIService, CelebrationAIService>();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JwtSigningKeyFactory.CreateKey(jwtOptions.Key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseDeveloperExceptionPage();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
