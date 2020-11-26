using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain
{
    public class GameShip
    {
        public int GameShipId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Width { get; set; }

        [MaxLength(128)]
        public string Name { get; set; } = null!;

        public int Hits { get; set; }

        public bool IsSunk { get; set; }

        public ECellState ECellState { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;
    }
}