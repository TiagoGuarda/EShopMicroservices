var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssembly(assembly);
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));
    x.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddCarter(new DependencyContextAssemblyCatalog([assembly]));

builder.Services.AddMarten(x => 
{
    x.Connection(builder.Configuration.GetConnectionString("Database")!);
    x.Schema.For<ShoppingCart>().Identity(x => x.UserName); // use UserName as the identity for ShoppingCart
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(x =>
{
    x.Configuration = builder.Configuration.GetConnectionString("Redis");
    //x.InstanceName = "Basket";
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();

app.UseExceptionHandler(x => { });

app.UseHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

app.Run();
