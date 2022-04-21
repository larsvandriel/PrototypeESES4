using InventoryManagementSystem.API.Kafka;
using InventoryManagementSystem.DataAccessLayer;
using InventoryManagementSystem.KafkaAccessLayer;
using InventoryManagementSystem.Logic;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IHostedService, KafkaConsumerDecreaseStockEvent>();
builder.Services.AddSingleton<IInventoryManager, InventoryManager>();
builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
builder.Services.AddSingleton<IInventoryEventSender, KafkaAccessLayer>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.Configure<IISOptions>(options =>
{

});

ConfigurationManager config = builder.Configuration;
var connectionString = config["mssqlconnection:connectionString"];
builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
