using CJ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CJ.Services
{
    public class DefaultDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        public DefaultDbContext()
        {

        }

        public DefaultDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            var configurationRoot = builder.Build();

            var nameSection = configurationRoot.GetSection("AllowedHosts");

            //注入MySql链接字符串
            optionsBuilder.UseMySql(
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetSection("DBConnectionString").Value);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<User>().HasIndex(u => u.Aaccount).IsUnique();
        }

    }
}
