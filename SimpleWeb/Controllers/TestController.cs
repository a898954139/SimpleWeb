using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SimpleWeb.Controllers;

public class TestController : Controller
{
    private readonly IOptionsMonitor<AppSettingsModel> _settings;
    private readonly ILogger<TestController> _logger;
    private int _id;
    private string _name;
    private bool _Istest;

    private TestController(IOptionsMonitor<AppSettingsModel> settings, ILogger<TestController> logger)
    {
        _settings = settings;
        _logger = logger;

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

    // GET
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
}

public class AppSettingsModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsTest { get; set; }
}