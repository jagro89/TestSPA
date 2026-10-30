using BGK.TestSPA.Server.Keycloak;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddKeycloak(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseDefaultFiles();
app.UseKeycloak();
app.MapStaticAssets().RequireAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/auth/user", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated != true)
        return Results.Unauthorized();

    return Results.Ok(new
    {
        name = context.User.Identity.Name,
        username = context.User.FindFirst("preferred_username")?.Value,
    });
}).AllowAnonymous();

app.MapGet("/auth/login", (string? returnUrl) =>
{
    var redirectUri = !string.IsNullOrWhiteSpace(returnUrl)
        && returnUrl.StartsWith('/')
        && !returnUrl.StartsWith("//")
        && !returnUrl.StartsWith("/\\")
        ? returnUrl
        : "/";
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = redirectUri },
        [OpenIdConnectDefaults.AuthenticationScheme]);
}).AllowAnonymous();

app.MapPost("/auth/logout", () => Results.SignOut(
    new AuthenticationProperties { RedirectUri = "/" },
    [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

app.MapFallbackToFile("/index.html").RequireAuthorization();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
