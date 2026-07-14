using CelebrationPassports.Infrastructure.AI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly ICelebrationAIService _ai;

    public AIController(ICelebrationAIService ai)
    {
        _ai = ai;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] string prompt)
    {
        var result = await _ai.GenerateAsync(prompt);

        return Ok(result);
    }
}