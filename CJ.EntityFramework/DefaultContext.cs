using CJ.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CJ.EntityFramework
{
    public class DefaultDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DefaultDbContext()
        {

        }

        public DefaultDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //注入Sql链接字符串
            optionsBuilder.UseMySql(@"Server=132.232.165.166;Database=xcjDB;User=root;Password=xucanjie;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<User>().HasIndex(u => u.Aaccount).IsUnique();
        }

    }
}
