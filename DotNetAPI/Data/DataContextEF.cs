using System.Data;
using Dapper;
using DotnetAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data;

class DataContextEF : DbContext
{
    private readonly IConfiguration _config;

    public DataContextEF(IConfiguration config)
    {
        _config = config;
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserSalary> UserSalary { get; set; }
    public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
            optionsBuilder => optionsBuilder.EnableRetryOnFailure()); //SE fallisce riprova
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("TutorialAppSchema"); //Facciamo un ovverride del metodo per impostare come default schema il nostro
        modelBuilder.Entity<User>()
                 .ToTable("Users", "TutorialAppSchema")
                 .HasKey(u => u.UserId); //Questo serve per fare il match tra il modello User e la tabella Users visto che hanno nomi diversi e setta la chiave univoca

        modelBuilder.Entity<UserSalary>()
               .HasKey(u => u.UserId);

        modelBuilder.Entity<UserJobInfo>()
               .HasKey(u => u.UserId);
    }
}