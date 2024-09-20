using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SimpleWeb.Models;

namespace SimpleWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOptionsMonitor<AppSettingsModel> _settings;
    private int _id;
    private string _name;
    private bool _Istest;
    private ITest _test;

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

    public async Task test()
    {
        const string sql = 
            $@"INSERT INTO ScoredWiningNumberTest (LocationId, Issue, LotteryCode, Balls, Status, DrawTime, RowAtUtc, CreateAtUtc) 
                VALUES
                (1, 113000003, 'test2', '12,29,31,37,44,49|03', 0, NULL, '2024-01-10 03:23:00', '2024-01-10 03:23:00');";

        await using DbConnection dbConnection = new MySqlConnection("Server=localhost;Port=3306;Database=ZdGameCrawler;User Id=root;Password=root;Charset=utf8mb4;Convert Zero Datetime=True;");
        await dbConnection.OpenAsync();
        var transaction = await dbConnection.BeginTransactionAsync();
        try
        {
            await dbConnection.ExecuteAsync(sql, transaction: transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(e.Message);
            Console.ResetColor();
        }
            
    }
}

internal interface ITest
{
    void testtest(string tt);
}

