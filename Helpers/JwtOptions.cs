namespace Lpgin2.Helpers
{
    public class JwtOptions
    {
        public required string  Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SigningKey { get; set; }
        public int LifetimeInMinutes { get; set; } = 30;
    }
}
