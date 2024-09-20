using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using PuppeteerSharp;
using SimpleWeb.Controllers;
using SimpleWeb.TelegramBot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.Configure<AppSettingsModel>(builder.Configuration);

builder.Services.AddSingleton<TelegramBot>(services =>
{
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<TelegramBot>();
    var telegramToken = builder.Configuration["Telegram:Token"];
    var telegramChatId = builder.Configuration["Telegram:ChartId"];    
    var httpclient = services.GetRequiredService<IHttpClientFactory>().CreateClient();
    var memoryCache = services.GetRequiredService<IMemoryCache>();
    return new TelegramBot(logger, telegramToken, telegramChatId, httpclient, memoryCache);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

var telegramBot = app.Services.GetRequiredService<TelegramBot>(); 
const string proxyServer = "http://104.154.185.78:6666";
const string selector = "#keno-play > div.keno__winning-numbers > div:nth-child(1) > div.keno__winning-numbers__results.results.keno__winning-numbers__results_grid";
app.MapGet("redis", async () =>
{

});
app.MapGet("/v2", async () =>
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    try
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = [$"--proxy-server={proxyServer}"]
        });

        await using var page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = 1920,
            Height = 1080
        });

        await page.GoToAsync("https://www.playnow.com/keno/winning-numbers/");
        

        // Scroll to the specific element
        await ScrollToElementV2(page, selector, -500);
    
        // Take screenshot
        Console.WriteLine("Start taking screen shot...");
        await page.EvaluateFunctionAsync(@"() => {
            if (!window.html2canvas) {
                var script = document.createElement('script');
                script.src = 'https://cdnjs.cloudflare.com/ajax/libs/html2canvas/0.4.1/html2canvas.min.js';
                script.onload = () => console.log('html2canvas loaded');
                document.head.appendChild(script);
            }
        }");

        // Wait for html2canvas to be available
        await page.WaitForFunctionAsync("() => window.html2canvas !== undefined");

        var base64Screenshot = await page.EvaluateFunctionAsync<string>(@"() => {
            return new Promise((resolve, reject) => {
                try {
                    window.scrollTo(0, 0);
                    document.body.style.background = 'white';
                    window.html2canvas(document.body, {
                        onrendered: function(canvas) {
                            resolve(canvas.toDataURL('image/png').split(',')[1]);
                        }
                    });
                } catch (error) {
                    reject(error);
                }
            });
        }");

        var screenshotBytes = Convert.FromBase64String(base64Screenshot);

        Console.WriteLine("Done screenshot.");
        await page.CloseAsync();
        stopwatch.Stop();
        
        // Console.WriteLine("Start reading image from png...");
        // var bytes = await File.ReadAllBytesAsync("screenshotTest.png");
        // Console.WriteLine($"Done reading image.");

        await telegramBot.SendPhotoAsync(screenshotBytes, $"Execution completed successfully 共花 {stopwatch.Elapsed.TotalSeconds:0.00} 秒");
        return Results.Ok($"Execution completed successfully 共花 {stopwatch.Elapsed.TotalSeconds:0.00} 秒");
    }
    catch (Exception ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.MapGet("/v1", async () =>
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    try
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = [$"--proxy-server={proxyServer}"]
        });

        await using var page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = 1920,
            Height = 1080
        });

        await page.GoToAsync("https://www.playnow.com/keno/winning-numbers/");
        
        // Scroll to the specific element
        await ScrollToElement(page, selector, -100);
    
        // Take screenshot
        await page.ScreenshotAsync("screenshotTest.png");
        await browser.CloseAsync();
        
        var screenshotBytes = await File.ReadAllBytesAsync("screenshotTest.png");
        await telegramBot.SendPhotoAsync(screenshotBytes, $"Execution completed successfully 共花 {stopwatch.Elapsed.TotalSeconds:0.00} 秒");
        stopwatch.Stop();

        return Results.Ok($"Execution completed successfully 共花 {stopwatch.Elapsed.TotalSeconds:0.00} 秒");
    }
    catch (Exception ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
return;

async Task ScrollToElementV2(Page page, string selector, int verticalOffset = 0)
{
    var isElementVisible = await page.EvaluateFunctionAsync<bool>(@"(selector) => {
        const element = document.querySelector(selector);
        return !!element;
    }", selector);

    if (!isElementVisible)
    {
        throw new Exception($"Element with selector '{selector}' not found on the page.");
    }

    await page.EvaluateFunctionAsync(@"(selector, offset) => {
        const element = document.querySelector(selector);
        if (element) {
            const elementRect = element.getBoundingClientRect();
            const absoluteElementTop = elementRect.top + window.pageYOffset;
            const middle = absoluteElementTop - (window.innerHeight / 2) + (elementRect.height / 2);
            const scrollPosition = middle + offset;
            window.scrollTo({
                top: scrollPosition,
                behavior: 'smooth'
            });
        }
    }", selector, verticalOffset);
}

async Task ScrollToElement(Page page, string selector, int verticalOffset = 0)
{
    var isElementVisible = await page.EvaluateFunctionAsync<bool>(@"(selector) => {
        const element = document.querySelector(selector);
        return !!element;
    }", selector);

    if (!isElementVisible)
    {
        throw new Exception($"Element with selector '{selector}' not found on the page.");
    }

    await page.EvaluateFunctionAsync(@"(selector, offset) => {
        const element = document.querySelector(selector);
        if (element) {
            const elementRect = element.getBoundingClientRect();
            const absoluteElementTop = elementRect.top + window.pageYOffset;
            const middle = absoluteElementTop - (window.innerHeight / 2) + (elementRect.height / 2);
            const scrollPosition = middle + offset;
            window.scrollTo({
                top: scrollPosition,
                behavior: 'smooth'
            });
        }
    }", selector, verticalOffset);
}