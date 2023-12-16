using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleWeb.Models;

namespace SimpleWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOptionsMonitor<AppSettingsModel> _settings;
    private int _id;
    private string _name;
    private bool _Istest;

    public HomeController(ILogger<HomeController> logger, IOptionsMonitor<AppSettingsModel> settings)
    {
        _logger = logger;
        _settings = settings;
        
        _id = settings.CurrentValue.Id;
        _name = settings.CurrentValue.Name;
        _Istest = settings.CurrentValue.IsTest;
        settings.OnChange(currentValue =>
        {
            _id = currentValue.Id;
            _name = currentValue.Name;
            _Istest = currentValue.IsTest;
        });
    }

    public IActionResult Index()
    {
        var testModel = new AppSettingsModel
        {
            Id = _id,
            Name = _name,
            IsTest = _Istest
        };
        ViewBag.AppSettings = testModel;
        return View();
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