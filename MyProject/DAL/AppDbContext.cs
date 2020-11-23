using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<GameOption> GameOptions { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<GameOptionShip> GameOptionShips { get; set; } = null!;
        public DbSet<Ship> Ships { get; set; } = null!;
        public DbSet<GameShip> GameShips { get; set; } = null!;

        public DbSet<PlayerBoardState> PlayerBoardStates { get; set; } = null!;


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // modelBuilder
            //     .Entity<Player>()
            //     .HasOne<Game>()
            //     .WithOne(x => x.PlayerA)
            //     .HasForeignKey<Game>(x => x.PlayerAId);
            
            // modelBuilder
            //     .Entity<Player>()
            //     .HasOne<Game>()
            //     .WithOne(x => x.PlayerB)
            //     .HasForeignKey<Game>(x => x.PlayerBId);
            
            // modelBuilder
            //     .Entity<Game>()
            //     .HasOne<Player>()
            //     .WithOne(x => x.GameA)
            //     .HasForeignKey<Player>(x => x.GameAId);
            
            // modelBuilder
            //     .Entity<Game>()
            //     .HasOne<Player>()
            //     .WithOne(x => x.GameB)
            //     .HasForeignKey<Player>(x => x.GameBId);
            
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