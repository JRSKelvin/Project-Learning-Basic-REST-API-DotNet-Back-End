namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts
{
    public class RefreshTokenResponse
    {
        public UserResponse User { get; set; } = new();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
