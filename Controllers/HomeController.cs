using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnsoocialLandingPage.Models;
using UnsoocialLandingPage.Services;

namespace UnsoocialLandingPage.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
	private readonly GoogleSheetService _googleSheetService;

	public HomeController(ILogger<HomeController> logger, GoogleSheetService subscriptionService)
    {
        _logger = logger;
		_googleSheetService = subscriptionService;
	}

    public IActionResult Index()
    {
        return View();
    }

	[HttpPost]
	public async Task<IActionResult> Subscribe(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return Json(new { result = "error", message = "Invalid email format." });
		}

		var result = await _googleSheetService.AddSubscriptionAsync(email);

		return Json(new { result });
	}

	public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
