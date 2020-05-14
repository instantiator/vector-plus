using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VectorPlus.Lib;

namespace VectorPlus.Web.Service.Db
{
    public class VectorPlusBackgroundServiceDbContext : DbContext
    {
        public DbSet<ModuleConfig> Modules { get; set; }
        public DbSet<RoboConfig> RoboConfig { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=vectorplus.db");
    }

}
