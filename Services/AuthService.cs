using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Data;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Config;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Domain;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppModeUserService _appModeUserService;
        private readonly AppDatabaseContext? _context;
        private readonly IJwtService _jwtService;
        private readonly JwtSetting _jwtSetting;
        public AuthService(AppModeUserService appModeUserService, AppDatabaseContext? context, IJwtService jwtService, IOptions<JwtSetting> jwtOptions)
        {
            _appModeUserService = appModeUserService;
            _context = context;
            _jwtService = jwtService;
            _jwtSetting = jwtOptions.Value;
        }
        private static UserResponse MapUser(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
        };
        public async Task<SignUpResponse> SignUpAsync(SignUpRequest request)
        {
            if (_appModeUserService.UseDummyMode)
            {
                var dummyUser = _appModeUserService.Users.First();
                return new SignUpResponse { User = MapUser(dummyUser) };
            }
            if (await _context!.Users.AnyAsync(u => u.Email == request.Email)) throw new Exception("User Already Exists");
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new SignUpResponse { User = MapUser(user) };
        }
        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            if (_appModeUserService.UseDummyMode)
            {
                var dummyUser = _appModeUserService.Users.FirstOrDefault((u) => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && BCrypt.Net.BCrypt.Verify(request.Password, u.PasswordHash));
                if (dummyUser == null) throw new UnauthorizedAccessException("Invalid Username Or Password");
                var access = _jwtService.GenerateAccessToken(dummyUser);
                return new SignInResponse
                {
                    User = MapUser(dummyUser),
                    AccessToken = access,
                    RefreshToken = dummyUser.RefreshToken,
                };
            }
            var user = await _context!.Users.FirstOrDefaultAsync((u) => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid Username Or Password");
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationDays);
            await _context.SaveChangesAsync();
            return new SignInResponse
            {
                User = MapUser(user),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }
        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            if (_appModeUserService.UseDummyMode)
            {
                var dummyUser = _appModeUserService.Users.FirstOrDefault((u) => u.RefreshToken == refreshToken);
                if (dummyUser == null) throw new UnauthorizedAccessException("Invalid Refresh Token");
                var newAccess = _jwtService.GenerateAccessToken(dummyUser);
                return new RefreshTokenResponse
                {
                    User = MapUser(dummyUser),
                    AccessToken = newAccess,
                    RefreshToken = dummyUser.RefreshToken,
                };
            }
            var user = await _context!.Users.FirstOrDefaultAsync((u) => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow) throw new UnauthorizedAccessException("Invalid Refresh Token");
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user);
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationDays);
            await _context.SaveChangesAsync();
            return new RefreshTokenResponse
            {
                User = MapUser(user),
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }
        public async Task<GetCurrentUserMeResponse> GetCurrentUserMeAsync(ClaimsPrincipal principal)
        {
            var email = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) throw new UnauthorizedAccessException("No Email Claim Found");
            if (_appModeUserService.UseDummyMode)
            {
                var dummyUser = _appModeUserService.Users.FirstOrDefault((u) => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                if (dummyUser != null) return new GetCurrentUserMeResponse { User = MapUser(dummyUser) };
                throw new KeyNotFoundException("User Not Found");
            }
            var user = await _context!.Users.FirstOrDefaultAsync((u) => u.Email == email);
            if (user == null) throw new KeyNotFoundException("User Not Found");
            return new GetCurrentUserMeResponse { User = MapUser(user) };
        }
    }
}
