// using System;
// using System.Linq;
// using Domain;
// using Microsoft.EntityFrameworkCore;
//
// namespace DAL
// {
//     public class ApplicationDbContext : DbContext
//     {
//         // public DbSet<BoardCellState>? BoardCellStates { get; set; }
//         //
//         // public DbSet<BoardState>? BoardStates { get; set; }
//         //
//         // public DbSet<Game>? Games { get; set; }
//         //
//         // public DbSet<GameOption>? GameOptions { get; set; }
//         //
//         // public DbSet<GameOptionShip>? GameOptionShips { get; set; }
//         //
//         // public DbSet<GameShip>? GameShips { get; set; }
//         //
//         // public DbSet<Player>? Players { get; set; }
//         //
//         // public PlayerBoardState? PlayerBoardStates { get; set; }
//         //
//         // public DbSet<Ship>? Ships { get; set; }
//
//         public DbSet<Game> Games { get; set; } = null!;
//         public DbSet<GameOption> GameOptions { get; set; } = null!;
//         public DbSet<Player> Players { get; set; } = null!;
//         public DbSet<GameOptionShip> GameOptionBoats { get; set; } = null!;
//         public DbSet<Ship> Boats { get; set; } = null!;
//         public DbSet<GameShip> GameBoats { get; set; } = null!;
//
//
//         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         {
//             base.OnConfiguring(optionsBuilder);
//             // optionsBuilder.UseSqlServer(@"
//             //     Server=barrel.itcollege.ee,1533;
//             //     User Id=student;
//             //     Password=Student.Bad.password.0;
//             //     Database=raskil_BattleshipGameDb;
//             //     MultipleActiveResultSets=true;
//             //     "
//             // );
//             // optionsBuilder.UseSqlServer(
//             //     @"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;"
//             // );
//             optionsBuilder.UseSqlite(@"Data Source=C:\Users\Rasmus\RiderProjects\icd0008-2020f\MyProject\ConsoleApp\app.db");
//         }
//
//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);
//
//             modelBuilder
//                 .Entity<Player>()
//                 .HasOne<Game>()
//                 .WithOne(x => x.PlayerA)
//                 .HasForeignKey<Game>(x => x.PlayerAId);
//
//             modelBuilder
//                 .Entity<Player>()
//                 .HasOne<Game>()
//                 .WithOne(x => x.PlayerB)
//                 .HasForeignKey<Game>(x => x.PlayerBId);
//
//             modelBuilder
//                 .Entity<Game>()
//                 .HasOne<Player>()
//                 .WithOne(x => x.GameA)
//                 .HasForeignKey<Player>(x => x.GameAId);
//             modelBuilder
//                 .Entity<Game>()
//                 .HasOne<Player>()
//                 .WithOne(x => x.GameB)
//                 .HasForeignKey<Player>(x => x.GameBId);
//
//
//             // remove the cascade delete
//             foreach (var relationship in modelBuilder.Model
//                 .GetEntityTypes()
//                 .Where(e => !e.IsOwned())
//                 .SelectMany(e => e.GetForeignKeys()))
//             {
//                 relationship.DeleteBehavior = DeleteBehavior.Restrict;
//             }
//         }
//     }
// }