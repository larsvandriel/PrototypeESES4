using InventoryManagementSystem.API.Kafka;
using InventoryManagementSystem.DataAccessLayer;
using InventoryManagementSystem.KafkaAccessLayer;
using InventoryManagementSystem.Logic;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
var kafkaBootstrapServers = config["Kafka:bootstrapServers"];

Console.WriteLine(connectionString);
Console.WriteLine();
Console.WriteLine(kafkaBootstrapServers);

Console.WriteLine(connectionString);
builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IInventoryManager, InventoryManager>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryEventSender>(x => new KafkaAccessLayer(kafkaBootstrapServers));

builder.Services.AddHostedService(x => new KafkaConsumer(x, kafkaBootstrapServers));
//builder.Services.AddHostedService(x => new KafkaConsumerDeleteProductEvent(x, kafkaBootstrapServers));
//builder.Services.AddHostedService(x => new KafkaConsumerUpdateProductEvent(x, kafkaBootstrapServers));
//builder.Services.AddHostedService(x => new KafkaConsumerCreateProductEvent(x, kafkaBootstrapServers));
//builder.Services.AddHostedService(x => new KafkaConsumerDecreaseStockEvent(x, kafkaBootstrapServers));

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
