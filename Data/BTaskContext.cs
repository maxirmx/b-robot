using b_robot_api.Models;
using Microsoft.EntityFrameworkCore;

namespace b_robot_api.Data;
public class BTaskContext : DbContext
{
    public BTaskContext(DbContextOptions<BTaskContext> options) : base(options)
    {
    }

    public DbSet<BTask> BTasks { get; set; }
    public DbSet<User> Users { get; set; }
}
