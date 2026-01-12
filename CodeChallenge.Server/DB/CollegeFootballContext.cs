using CodeChallenge.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CodeChallenge.Server.DB
{
    public class CollegeFootballContext : DbContext
    {
        public DbSet<TeamStats> TeamStats { get; set; }
        public DbSet<FileColumnHeader> FileColumns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString: "Filename=CollegeFootball.db",
                sqliteOptionsAction: op =>
                {
                    op.MigrationsAssembly(
                        Assembly.GetExecutingAssembly().FullName
                        );
                });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamStats>().ToTable("TeamStats");
            modelBuilder.Entity<FileColumnHeader>().ToTable("FileColumnHeader");
            base.OnModelCreating(modelBuilder);
        }

    }
}
