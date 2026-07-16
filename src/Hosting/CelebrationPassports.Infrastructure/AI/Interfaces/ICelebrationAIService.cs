using System.Threading.Tasks;

namespace CelebrationPassports.Infrastructure.AI.Interfaces
{
    public interface ICelebrationAIService
    {
        Task<string> GenerateAsync(string prompt, int? maxTokens = null);

        Task<string> GenerateWithImageAsync(string prompt, byte[] imageBytes, int? maxTokens = null, double? temperature = null);
    }
}