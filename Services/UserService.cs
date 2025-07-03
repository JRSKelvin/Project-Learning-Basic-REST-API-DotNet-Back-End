using Microsoft.EntityFrameworkCore;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Data;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Domain;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Services
{
    public class UserService : IUserService
    {
        private readonly AppDatabaseContext _context;
        public UserService(AppDatabaseContext context)
        {
            _context = context;
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> UpdateAsync(Guid id, User updated)
        {
            var user = await _context.Users.FindAsync(id);
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
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
