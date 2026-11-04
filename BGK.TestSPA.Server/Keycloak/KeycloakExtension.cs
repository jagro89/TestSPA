using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace BGK.TestSPA.Server.Keycloak
{
    public static class KeycloakExtension
    {
        public static IServiceCollection AddKeycloak(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

            var keycloak = configuration.GetSection("Keycloak").Get<KeycloakSettings>()                ?? throw new InvalidOperationException("Keycloak section not found.");

            if (string.IsNullOrWhiteSpace(keycloak.Server) || string.IsNullOrWhiteSpace(keycloak.Realm) || string.IsNullOrWhiteSpace(keycloak.ClientId))
                throw new InvalidOperationException("Keycloak:Server, Keycloak:Realm and Keycloak:ClientId is empty.");

            services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));

            var authority = $"{keycloak.Server.TrimEnd('/')}/realms/{keycloak.Realm}";
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   options.Cookie.Name = "BGK.TestSPA.Auth";
                   options.Cookie.HttpOnly = true;
                   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                   options.Cookie.SameSite = SameSiteMode.Lax;
                   options.SlidingExpiration = true;
               });
            services.AddAuthentication()
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authority;
                    options.ClientId = keycloak.ClientId;
                    options.ClientSecret = keycloak.ClientSecret;
                    options.RequireHttpsMetadata = !environment.IsDevelopment();
                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SignedOutCallbackPath = "/signout-callback-oidc";
                    if (!string.IsNullOrWhiteSpace(keycloak.RedirectUri))
                    {
                        if (!Uri.TryCreate(keycloak.RedirectUri, UriKind.Absolute, out var redirectUri) || redirectUri.AbsolutePath != "/signin-oidc")
                        {
                            throw new InvalidOperationException("Keycloak:RedirectUri have to end with /signin-oidc.");
                        }

                        options.Events.OnRedirectToIdentityProvider = context =>
                        {
                            var redirectUri = new UriBuilder(keycloak.RedirectUri);
                            if (context.Request.PathBase.HasValue)
                            {
                                redirectUri.Path = $"{context.Request.PathBase}{options.CallbackPath}";
                            }

                            context.ProtocolMessage.RedirectUri = redirectUri.Uri.AbsoluteUri;
                            return Task.CompletedTask;
                        };
                    }
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "preferred_username",
                        RoleClaimType = "role",
                    };
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }

        public static void UseKeycloak(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
