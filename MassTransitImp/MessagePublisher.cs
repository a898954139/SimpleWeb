using MassTransit;

namespace MassTransitImp;

public class MessagePublisher(IBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await bus.Publish(
                new CurrentTime
                {
                    Value = $"The current time is {DateTime.UtcNow}"
                },
                stoppingToken);
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}

public record CurrentTime
{
    public string Value { get; init; } = string.Empty;
};