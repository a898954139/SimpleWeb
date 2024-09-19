using Seq;
using Seq.ExtensionMethods;
using Seq.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplication();

// Seq appsettings logging configure
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Seq http request logging middleware configure
app.UseSerilogRequestLogging();

// Mediator configure
app.UseMiddleware<RequestLogContextMiddleware>();

app
    .MapGet("/weatherforecast", () =>
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", 
            "Mild", "Warm", "Balmy", "Hot", 
            "Sweltering", "Scorching"
        };
        
        var forecast = Enumerable
            .Range(1, 5)
            .Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}