namespace BGK.TestSPA.Server.Keycloak
{
    public class KeycloakSettings
    {
        public string Server { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string? RedirectUri { get; set; }
    }
}