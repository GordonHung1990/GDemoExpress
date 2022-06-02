using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using GDemoExpress.DependencyInjection;
using GDemoExpress;
using GDemoExpress.DataBase;
using GDemoExpress.DataBase.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddPlayerServer(
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("Mongo");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("Redis");
    })
     .AddPlayerServerCore()
    .AddPlayerServerRepositories(
    (sp, dbBuilder) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        _ = dbBuilder.UseNpgsql(
            config.GetConnectionString("Postgres"),
            serverOption =>
            {
                _ = serverOption.SetPostgresVersion(12, 5);
                _ = serverOption.CommandTimeout(180);
            });
    },
    (sp) => sp.GetRequiredService<RedisConnectionManager>().Database,
    (sp) => sp.GetRequiredService<MongoDBConnectionManager>().Database);

//builder.Services.AddTransient<IStartupFilter, MigrationStartupFilter<DboContext>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

_ = app.UseHttpsRedirection();

_ = app.UseAuthorization();

_ = app.MapControllers();

_= app.MigrateDatabase();

app.Run();
