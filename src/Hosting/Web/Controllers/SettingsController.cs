using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Places;
using CelebrationPassports.Web.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly IUserProfileService _userProfileService;
    private readonly IPlaceService _placeService;

    public SettingsController(IUserProfileService userProfileService, IPlaceService placeService)
    {
        _userProfileService = userProfileService;
        _placeService = placeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await _userProfileService.GetSettingsAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SetHomePlace(string placeName, string city, decimal? latitude, decimal? longitude)
    {
        if (string.IsNullOrWhiteSpace(placeName) || string.IsNullOrWhiteSpace(city) || latitude is null || longitude is null)
        {
            ModelState.AddModelError("", "Place name, city, and coordinates are all required — trip detection can't measure distance without real coordinates.");
            var model = await _userProfileService.GetSettingsAsync();
            model.PlaceName = placeName;
            model.City = city;
            model.Latitude = latitude;
            model.Longitude = longitude;
            return View("Index", model);
        }

        var placeId = await _placeService.CreateAsync(new CreatePlaceViewModel
        {
            Name = placeName,
            City = city,
            Country = "India",
            Latitude = latitude,
            Longitude = longitude
        });

        if (placeId is null)
        {
            ModelState.AddModelError("", "Could not save your home location. Please try again.");
            var model = await _userProfileService.GetSettingsAsync();
            return View("Index", model);
        }

        await _userProfileService.SetHomePlaceAsync(placeId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ClearHomePlace()
    {
        await _userProfileService.SetHomePlaceAsync(null);
        return RedirectToAction("Index");
    }
}
