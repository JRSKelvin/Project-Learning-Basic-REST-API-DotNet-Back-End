using Microsoft.EntityFrameworkCore;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Data;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Domain;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Services
{
    public class UserService : IUserService
    {
        private readonly AppDatabaseContext? _context;
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
        public UserService(IServiceProvider serviceProvider)
        {
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
        public async Task<List<User>> GetAllAsync()
        {
            if (_useDummy) return _dummyData;
            return await _context!.Users.ToListAsync();
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (_useDummy) return _dummyData.FirstOrDefault((u) => u.Id == id);
            return await _context!.Users.FindAsync(id);
        }
        public async Task<User> CreateAsync(User user)
        {
            if (_useDummy) return user;
            _context!.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> UpdateAsync(Guid id, User updated)
        {
            if (_useDummy)
            {
                var original = _dummyData.FirstOrDefault((u) => u.Id == id);
                if (original == null) return null;
                return new User
                {
                    Id = original.Id,
                    Name = updated.Name ?? original.Name,
                    Email = updated.Email ?? original.Email,
                    PhoneNumber = updated.PhoneNumber ?? original.PhoneNumber,
                };
            }
            var user = await _context!.Users.FindAsync(id);
            if (user == null) return null;
            if (updated.Name != null)
            {
                user.Name = updated.Name;
            }
            if (updated.Email != null)
            {
                user.Email = updated.Email;
            }
            if (updated.PhoneNumber != null)
            {
                user.PhoneNumber = updated.PhoneNumber;
            }
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> DeleteAsync(Guid id)
        {
            if (_useDummy)
            {
                var original = _dummyData.FirstOrDefault((u) => u.Id == id);
                return original;
            }
            var user = await _context!.Users.FindAsync(id);
            if (user == null) return null;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
