using Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class BTaskContext : DbContext
    {
        public BTaskContext(DbContextOptions<BTaskContext> options) : base(options)
        {
        }

        public DbSet<BTask> BTasks { get; set; }
    }
}
