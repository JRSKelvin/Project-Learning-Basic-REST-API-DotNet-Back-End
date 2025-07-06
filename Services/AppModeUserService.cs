using Microsoft.EntityFrameworkCore;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Data;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Domain;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Services
{
    public class AppModeUserService
    {
        public bool UseDummyMode { get; }
        public List<User> Users { get; } = new()
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
        public AppModeUserService(IServiceProvider serviceProvider)
        {
            try
            {
                var context = serviceProvider.GetService<AppDatabaseContext>();
                if (context == null)
                {
                    Console.WriteLine("⚠️ Database Context Not Registered And App Will Run In Dummy Mode");
                    UseDummyMode = true;
                    return;
                }
                context.Database.OpenConnection();
                context.Database.CloseConnection();
                UseDummyMode = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Failed To Connect To Database And App Will Run In Dummy Mode");
                Console.WriteLine(ex.Message);
                UseDummyMode = true;
            }
        }
    }
}
