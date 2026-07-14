using System.Threading.Tasks;

namespace CelebrationPassports.Infrastructure.AI.Interfaces
{
    public interface ICelebrationAIService
    {
        Task<string> GenerateAsync(string prompt);
    }
}