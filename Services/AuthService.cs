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
        private readonly AppDatabaseContext? _context;
        private readonly IJwtService _jwtService;
        private readonly JwtSetting _jwtSetting;
        private readonly bool _useDummy;
        private readonly List<User> _dummyData = new()
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Dummy User",
                Email = "dummy-user@email.com",
                PhoneNumber = "000-0000-0000",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("DummyUser"),
                RefreshToken = "DummyRefreshToken",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30),
            },
        };
        public AuthService(IServiceProvider serviceProvider, IJwtService jwtService, IOptions<JwtSetting> jwtOptions)
        {
            _jwtService = jwtService;
            _jwtSetting = jwtOptions.Value;
            try
            {
                _context = serviceProvider.GetService<AppDatabaseContext>();
                if (_context == null)
                {
                    Console.WriteLine("⚠️ Database Context Not Registered And App Will Run In Dummy Mode");
                    _useDummy = true;
                    return;
                }
                _context.Database.OpenConnection();
                _context.Database.CloseConnection();
                _useDummy = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Failed To Connect To Database And App Will Run In Dummy Mode");
                Console.WriteLine(ex.Message);
                _useDummy = true;
            }
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
            if (_useDummy)
            {
                var dummyUser = _dummyData.First();
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
            if (_useDummy)
            {
                var dummyUser = _dummyData.FirstOrDefault((u) => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) && BCrypt.Net.BCrypt.Verify(request.Password, u.PasswordHash));
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
            if (_useDummy)
            {
                var dummyUser = _dummyData.FirstOrDefault((u) => u.RefreshToken == refreshToken);
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
            if (_useDummy)
            {
                var dummyUser = _dummyData.FirstOrDefault((u) => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                if (dummyUser != null) return new GetCurrentUserMeResponse { User = MapUser(dummyUser) };
                throw new KeyNotFoundException("User Not Found");
            }
            var user = await _context!.Users.FirstOrDefaultAsync((u) => u.Email == email);
            if (user == null) throw new KeyNotFoundException("User Not Found");
            return new GetCurrentUserMeResponse { User = MapUser(user) };
        }
    }
}
