namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Config
{
    public class JwtSetting
    {
        public string AccessSecret { get; set; } = string.Empty;
        public string RefreshSecret { get; set; } = string.Empty;
        public int AccessTokenExpirationDays { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
