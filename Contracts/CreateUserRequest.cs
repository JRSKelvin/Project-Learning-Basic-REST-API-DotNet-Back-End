using System.ComponentModel.DataAnnotations;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts
{
    public class CreateUserRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
