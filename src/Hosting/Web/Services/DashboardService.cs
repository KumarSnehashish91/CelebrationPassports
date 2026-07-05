using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Dashboard;
using CelebrationPassports.Web.ViewModels.Dashboard;

namespace CelebrationPassports.Web.Services;

public class DashboardService : IDashboardService
{
    public DashboardViewModel GetDashboard()
    {
        return new DashboardViewModel
        {
            Summary = new DashboardSummary
            {
                UpcomingCelebrations = 5,
                Invitations = 8,
                PassportStamps = 32,
                Memories = 1248,
                Trips = 18,
                Countries = 7
            },

            UpcomingCelebrations =
            [
                new()
                {
                    Title = "Birthday",
                    Date = DateTime.Today.AddDays(5),
                    Type = "Birthday",
                    DaysRemaining = 5
                },
                new()
                {
                    Title = "Goa Trip",
                    Date = DateTime.Today.AddDays(18),
                    Type = "Trip",
                    DaysRemaining = 18
                }
            ],

            RecentActivities =
            [
                new()
                {
                    Title = "Added Birthday",
                    Description = "Created Rahul's Birthday",
                    ActivityDate = DateTime.Now,
                    Icon = "bi-gift"
                },
                new()
                {
                    Title = "Uploaded Memories",
                    Description = "Added 20 new photos",
                    ActivityDate = DateTime.Now,
                    Icon = "bi-camera"
                }
            ],

            PassportProgress = new PassportProgress
            {
                Percentage = 68,
                CompletedItems = 34,
                TotalItems = 50
            },

            QuickActions =
            [
                new()
                {
                    Title = "New Celebration",
                    Icon = "bi-plus-circle",
                    Url = "#"
                },
                new()
                {
                    Title = "New Trip",
                    Icon = "bi-airplane",
                    Url = "#"
                }
            ],
            MemoryHighlight = new MemoryHighlight
            {
                Title = "Wedding Anniversary",

                Location = "Udaipur, Rajasthan",

                Date = new DateTime(2025, 12, 10),

                Photos = 120,

                Videos = 3,

                People = 12,

                Description = "A perfect evening by Lake Pichola, beautiful memories with the one who makes life special.",

                ImageUrl = "/images/udaipur.jpg"
            },
        };
    }
}