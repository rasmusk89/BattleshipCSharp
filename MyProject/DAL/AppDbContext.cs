using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<GameOption> GameOptions { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<GameShip> GameShips { get; set; } = null!;
        // public DbSet<PlayerBoardState> PlayerBoardStates { get; set; } = null!;
        public DbSet<GameState> GameStates { get; set; } = null!;
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
            
        /* Use this to disable all cascade delete.
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        */
    }
}