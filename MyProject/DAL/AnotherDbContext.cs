﻿using System.Linq;
using AnotherDomain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AnotherDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        public DbSet<Ship> Ships { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(@"Data Source=C:\Users\Rasmus\RiderProjects\icd0008-2020f\MyProject\ConsoleApp\app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder
                .Entity<Player>()
                .HasOne<Game>()
                .WithOne(x => x.PlayerA)
                .HasForeignKey<Game>(x => x.PlayerAId);
            
            modelBuilder
                .Entity<Player>()
                .HasOne<Game>()
                .WithOne(x => x.PlayerB)
                .HasForeignKey<Game>(x => x.PlayerBId);
            
            modelBuilder
                .Entity<Game>()
                .HasOne<Player>()
                .WithOne(x => x.GameA)
                .HasForeignKey<Player>(x => x.GameAId);
            modelBuilder
                .Entity<Game>()
                .HasOne<Player>()
                .WithOne(x => x.GameB)
                .HasForeignKey<Player>(x => x.GameBId);
            
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}