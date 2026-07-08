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

            UpcomingCelebrations = new List<UpcomingCelebration>
            {
            new UpcomingCelebration
            {
                Title = "Son's Birthday",
                Type = "Birthday Celebration",
                Date = new DateTime(2026, 7, 11),
                ImageUrl = "/images/udaipur.jpg"
            },

            new UpcomingCelebration
            {
                Title = "Wedding Anniversary",
                Type = "Anniversary",
                Date = new DateTime(2026, 12, 10),
                ImageUrl = "/images/udaipur.jpg"
            },

            new UpcomingCelebration
            {
                Title = "Goa Vacation",
                Type = "Trip",
                Date = new DateTime(2026, 5, 18),
                ImageUrl = "/images/udaipur.jpg"
            },

            new UpcomingCelebration
            {
                Title = "Holi Celebration",
                Type = "Family Celebration",
                Date = new DateTime(2027, 3, 14),
                ImageUrl = "/images/udaipur.jpg"
            }
                },

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
            MemoryHero = new MemoryHighlight
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
            Timeline = new List<TimelineItem>
            {
                new TimelineItem
                {
                    Year = 2026,
                    Month = "DEC",
                    Title = "Wedding Anniversary",
                    Location = "Udaipur, Rajasthan",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#7C3AED"
                },

                new TimelineItem
                {
                    Year = 2026,
                    Month = "JUL",
                    Title = "Son's Birthday",
                    Location = "Delhi",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#34C759"
                },

                new TimelineItem
                {
                    Year = 2026,
                    Month = "MAY",
                    Title = "Goa Vacation",
                    Location = "Goa",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#FF9500"
                },

                new TimelineItem
                {
                    Year = 2026,
                    Month = "FEB",
                    Title = "Valentine's Dinner",
                    Location = "Olive Bistro",
                    ImageUrl="/images/udaipur.jpg",
                    StampColor="#FF2D55"
                }
            },
        };
    }
}