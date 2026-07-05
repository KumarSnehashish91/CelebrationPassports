using CelebrationPassports.Web.Interfaces;

namespace CelebrationPassports.Web.Services;

public class GreetingService : IGreetingService
{
    public string GetGreeting()
    {
        int hour = DateTime.Now.Hour;

        if (hour >= 5 && hour < 12)
            return "Good Morning";

        if (hour >= 12 && hour < 17)
            return "Good Afternoon";

        if (hour >= 17 && hour < 21)
            return "Good Evening";

        return "Good Night";
    }
}