using CelebrationPassports.Application;
using CelebrationPassports.Infrastructure.AI.Clients;
using CelebrationPassports.Infrastructure.AI.Configuration;
using CelebrationPassports.Infrastructure.Authentication;
using CelebrationPassports.Persistence;

using CelebrationPassports.Infrastructure.AI.Clients;
using CelebrationPassports.Infrastructure.AI.Configuration;
using CelebrationPassports.Infrastructure.AI.Services;
using CelebrationPassports.Infrastructure.AI.Interfaces;

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
