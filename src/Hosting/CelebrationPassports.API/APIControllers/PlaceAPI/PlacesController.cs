using CelebrationPassports.Application.Places.DTOs;
using CelebrationPassports.Application.Places.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.PlaceAPI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlacesController : ControllerBase
{
    private readonly IPlaceService _placeService;

    public PlacesController(IPlaceService placeService)
    {
        _placeService = placeService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search)
    {
        var result = await _placeService.SearchAsync(search ?? string.Empty);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlaceRequest request)
    {
        var result = await _placeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _placeService.GetByIdAsync(id);
        return Ok(result);
    }
}
