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
    public async Task<IActionResult> Generate([FromBody] string prompt, [FromQuery] int? maxTokens = null)
    {
        var result = await _ai.GenerateAsync(prompt, maxTokens);

        // Wrapped in an object rather than returned bare — a bare `string` action
        // result gets picked up by ASP.NET Core's built-in StringOutputFormatter and
        // served as text/plain (unquoted, unescaped), not JSON. The Web-side caller's
        // ReadFromJsonAsync<string>() then fails trying to parse that plain text as
        // JSON ("'O' is an invalid start of a value", for a response starting with
        // "OVERVIEW:" or similar). Every other endpoint in this API already returns an
        // object/DTO, which sidesteps this entirely — this was the one bare-string case.
        return Ok(new GenerateResponse { Response = result });
    }
}

public class GenerateResponse
{
    public string? Response { get; set; }
}