using API.Extensions;
using Application.Common.Abstractions;
using Infrastructure.Identity;
using Microsoft.OpenApi.Models;

const string CORS_POLICY = "Cors-Policy";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, builder =>
    {
        builder.WithOrigins("http://localhost:4200/")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

var app = builder.Build();

ServiceLocator.ServiceProvider = app.Services;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.IdentityInitialiseDatabaseAsync();

    app.UseSwaggerUi(options =>
    {
        options.Path = "/api";
        options.DocumentPath = "/api/specification.json";
    });
}
else 
{
    app.UseHsts();
}

app.UseCors(CORS_POLICY);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
//app.UseRouting();
app.UseAuthorization();

app.MapFallbackToFile("index.html");
app.UseExceptionHandler(options => { });

//need to check
app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.Run();

