namespace AuthenticationService
{
    public class ValidateResourceOwnerGrantRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
    }
}
