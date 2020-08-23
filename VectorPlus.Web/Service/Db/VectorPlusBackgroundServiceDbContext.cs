using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VectorPlus.Web.Service.Db
{
    public class VectorPlusBackgroundServiceDbContext : DbContext
    {
        public DbSet<ModuleConfig> Modules { get; set; }
        public DbSet<RoboConfig> RoboConfig { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=vectorplus.db");
        }

        public async Task InitAsync()
        {
            await Database.MigrateAsync();
        }
    }

}
