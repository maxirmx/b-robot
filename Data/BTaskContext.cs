using b_robot_api.Models;
using Microsoft.EntityFrameworkCore;

namespace b_robot_api.Data;
public class BTaskContext : DbContext
{
    public BTaskContext(DbContextOptions<BTaskContext> options) : base(options)
    {
    }

    public DbSet<BTask> BTasks { get; set; }

    public bool Exists(int id)
    {
        return BTasks.Any(e => e.Id == id);
    }

    public async Task<List<BTask>> BTasksForUser(int userId)
    {
        return await BTasks.AsNoTracking().Where(x => x.UserId == userId).Select(x => x).ToListAsync();
    }

}
