using Play.Common.Extensions;
using Play.Common.Settings;
using Play.Inventory.Service.Entities;

var builder = WebApplication.CreateBuilder(args);
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var originSettings = builder.Configuration["AllowedOrigin"];

builder.Services
    .AddMongo()
    .AddMongoRespository<InventoryEntity>("inventory")
    .AddMongoRespository<CatalogItem>("catalog")
    .AddMassTransitWithRabbitMq();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder =>
    {
        builder.WithOrigins(originSettings)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
