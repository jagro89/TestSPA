using BGK.TestSPA.Server.Keycloak;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddKeycloak(builder.Configuration, builder.Environment);

var app = builder.Build();

var pathBase = builder.Configuration["PathBase"]?.Trim();
if (!string.IsNullOrEmpty(pathBase))
{
    pathBase = pathBase.TrimEnd('/');
    if (pathBase.Length > 0)
    {
        if (!pathBase.StartsWith('/') || pathBase.Contains("//") || pathBase.Contains('?') || pathBase.Contains('#') || pathBase.Contains('\\'))
        {
            throw new InvalidOperationException("PathBase must be a valid absolute URL path, for example /test.");
        }

        app.UsePathBase(new PathString(pathBase));
    }
}

app.UseDefaultFiles();
app.UseKeycloak();
app.MapStaticAssets().RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/test", () =>
{
    return new string[] { "Wartość 1", "Wartość 2" };
})
.WithName("Test");

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

app.MapGet("/auth/login", (HttpContext context, string? returnUrl) =>
{
    var redirectUri = !string.IsNullOrWhiteSpace(returnUrl) && returnUrl.StartsWith('/') && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")
    ? returnUrl
    : context.Request.PathBase.Add("/").Value;

    return Results.Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, [OpenIdConnectDefaults.AuthenticationScheme]);
}).AllowAnonymous();

app.MapPost("/auth/logout", (HttpContext context) => Results.SignOut(
    new AuthenticationProperties { RedirectUri = context.Request.PathBase.Add("/").Value },
    [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

app.MapFallbackToFile("/index.html").RequireAuthorization();

app.Run();