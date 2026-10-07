namespace BGK.TestSPA.Server.Keycloak
{
    public class KeycloakSettings
    {
        public KeycloakSettings(IConfiguration configuration) { }

        public string Server { get; set; } = null!;
        public string Realm { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string? ClientSecret { get; set; }
    }
}