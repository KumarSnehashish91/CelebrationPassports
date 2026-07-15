namespace CelebrationPassports.Application.Calendar.Interfaces;

public interface ICalendarFeedService
{
    // Feature: Milestone Calendar Sync (feature-backlog-v1.1.md, PLAN #15), adapted to
    // this schema's actual dated data — MilestoneDefinition/PassportMilestoneProgress
    // are achievement-tracker milestones (count/boolean progress, no future date), not
    // calendar-worthy countdowns, so this generates the feed from upcoming/ongoing
    // Events instead (the Dashboard's own "Upcoming Celebrations").
    Task<string> GenerateIcsAsync(Guid userId);
}
