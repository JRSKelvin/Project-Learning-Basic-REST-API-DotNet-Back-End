using System.ComponentModel.DataAnnotations;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts
{
    public class SignInRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
