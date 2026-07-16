using CelebrationPassports.Web.Handlers;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5026/";

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddScoped<IGreetingService, GreetingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ITripPlannerService, TripPlannerService>();
builder.Services.AddScoped<IStoryNarrativeService, StoryNarrativeService>();
builder.Services.AddScoped<IRecapService, RecapService>();

// No BearerTokenHandler here — refresh calls carry the refresh token, not an access
// token, and this is the client BearerTokenHandler itself calls out to.
builder.Services.AddHttpClient<ITokenRefreshService, TokenRefreshService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IPassportService, PassportService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IEventService, EventService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IInvitationService, InvitationService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IPlaceService, PlaceService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IMediaService, MediaService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IStoryService, StoryService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IUserProfileService, UserProfileService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IAIService, AIService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ISomedayIdeaService, SomedayIdeaService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IMemoryMapService, MemoryMapService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ITimeCapsuleService, TimeCapsuleService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IWishService, WishService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IExpenseService, ExpenseService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ITripItineraryService, TripItineraryService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IDashboardStatsService, DashboardStatsService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IGuestbookService, GuestbookService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IChapterSharingService, ChapterSharingService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IImportService, ImportService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Without this, the browser's back/forward cache (bfcache) can replay a previously
// rendered authenticated page (Dashboard, Create Passport, etc.) straight from cache
// after the session has since died server-side — no request even reaches the server, so
// [Authorize] never gets a chance to redirect to Login. The page LOOKS logged in, but any
// real action on it fails because the server-side session is actually gone. Scoped to
// text/html only so static assets (css/js/images) keep their normal caching.
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.Response.ContentType?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true)
        {
            context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
            context.Response.Headers.Pragma = "no-cache";
            context.Response.Headers.Expires = "-1";
        }

        return Task.CompletedTask;
    });

    await next();
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
