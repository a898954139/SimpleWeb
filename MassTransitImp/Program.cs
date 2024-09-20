using MassTransit;
using MassTransitImp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var service = builder.Services;
service.AddEndpointsApiExplorer();
service.AddSwaggerGen();

service.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();
    
    // Single consumer register
    // cfg.AddConsumer<CurrentTimeConsumer>();
    
    // Multi consumer register
    cfg.AddConsumers(typeof(Program).Assembly);
    
    cfg.UsingInMemory(((context, configurator) =>
    {
      configurator.ConfigureEndpoints(context);  
    }));
});

service.AddHostedService<MessagePublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
