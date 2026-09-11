using API.Extensions;
using Application.Common.Abstractions;
using Infrastructure.Identity;
using Scalar.AspNetCore;

const string CORS_POLICY = "Cors-Policy";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, builder =>
    {
        builder.WithOrigins("http://localhost:4200")
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
    
    // NSwag generates the OpenAPI document
    app.UseOpenApi(options =>
    {
        options.Path = "/openapi/{documentName}.json";
    });

    // Scalar UI
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("API Documentation")
            .WithTheme(ScalarTheme.Default)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithOpenApiRoutePattern("/openapi/{documentName}.json");;
    });
}
else 
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(CORS_POLICY);
app.UseStaticFiles();

app.UseAuthentication();
//app.UseRouting();
app.UseAuthorization();

app.MapFallbackToFile("index.html");
app.UseExceptionHandler(options => { });

//need to check
app.Map("/", () => Results.Redirect("/scalar"));

app.MapEndpoints();

app.Run();

