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
using CelebrationPassports.Persistence.Repositories.Interfaces;
using System.Security.Claims;

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

        // A JWT's signature/expiry alone can't be revoked early — without this check, a
        // token stays fully usable for its full lifetime even after the user logs out.
        // session_id is embedded in the token at issuance (see JwtTokenService) and
        // checked here against the UserSession row Logout marks inactive, so a logged-out
        // token is rejected on the very next request instead of quietly working until it
        // naturally expires.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var sessionIdClaim = context.Principal?.FindFirstValue("session_id");
                var userIdClaim = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(sessionIdClaim, out var sessionId) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    context.Fail("Invalid session.");
                    return;
                }

                var sessionRepository = context.HttpContext.RequestServices.GetRequiredService<IUserSessionRepository>();
                var session = await sessionRepository.GetActiveSessionByUserIdAsync(userId, sessionId);

                if (session is null || !session.IsActive || session.RevokedOn != null)
                {
                    context.Fail("Session has been revoked. Please log in again.");
                }
            }
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

// Deliberately no UseHttpsRedirection() here: this API is called server-to-server by
// Web over plain HTTP (see Web's Api:BaseUrl). Redirecting that to HTTPS makes
// HttpClient strip the Authorization header on the cross-port redirect (a different
// port counts as a different origin), silently turning every authenticated call into
// a 401 — which is exactly what caused "stuck on Create Passport" / "create passport
// doesn't work" when both projects ran on their https launch profiles. TLS termination
// belongs in front of this API (e.g. Cloudflare per the system design doc), not here.

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
