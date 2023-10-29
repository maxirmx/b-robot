using Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public bool Exists(int id)
        {
            return Users.Any(e => e.Id == id);
        }

        public bool Exists(string email)
        {
            return Users.Any(e => e.Email == email);
        }

        public async Task<List<UserViewItem>> UserViewItems()
        {
            return await Users.AsNoTracking().Select(x => new UserViewItem()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Patronimic = x.Patronimic,
                    Email = x.Email,
                    ApiKey = x.ApiKey
                }).ToListAsync();
        }
    }
}
