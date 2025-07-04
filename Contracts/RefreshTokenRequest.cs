using System.ComponentModel.DataAnnotations;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
