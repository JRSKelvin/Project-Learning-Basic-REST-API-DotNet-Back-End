using System.Security.Claims;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Domain;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces
{
    public interface IAuthService
    {
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);
        Task<SignInResponse> SignInAsync(SignInRequest request);
        Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken);
        Task<GetCurrentUserMeResponse> GetCurrentUserMeAsync(ClaimsPrincipal user);
    }
}
