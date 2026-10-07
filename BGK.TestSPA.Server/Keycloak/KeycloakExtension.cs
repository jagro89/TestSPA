using Microsoft.AspNetCore.Authentication;

namespace BGK.TestSPA.Server.Keycloak
{
    public static class KeycloakExtension
    {
        public static IServiceCollection AddKeycloak(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

            var keycloak = new KeycloakSettings(configuration);

            services.AddAuthentication("Bearer")
               .AddJwtBearer("Bearer", options =>
               {
                   options.Authority = $"{keycloak.Server.TrimEnd('/')}/realms/{keycloak.Realm}";
                   options.Audience = keycloak.ClientId;
                   options.RequireHttpsMetadata = !environment.IsDevelopment();
                   options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   {
                       NameClaimType = "preferred_username",
                       RoleClaimType = "role",
                   };
               });
            services.AddAuthorization();

            return services;
        }

        public static void UseKeycloak(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
