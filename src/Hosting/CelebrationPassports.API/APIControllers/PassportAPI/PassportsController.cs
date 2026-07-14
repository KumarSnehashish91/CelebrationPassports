using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Passports.DTOs;
using CelebrationPassports.Application.Passports.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.PassportAPI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PassportsController : ControllerBase
{
    private readonly IPassportService _passportService;

    public PassportsController(IPassportService passportService)
    {
        _passportService = passportService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePassportRequest request)
    {
        var result = await _passportService.CreateAsync(User.GetUserId(), request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var result = await _passportService.GetMineAsync(User.GetUserId());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _passportService.GetByIdAsync(User.GetUserId(), id);
        return Ok(result);
    }
}
