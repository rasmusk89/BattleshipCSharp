using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain
{
    public class Player
    {
        public int PlayerId { get; set; }

        [MaxLength(128)]
        public string Name { get; set; } = null!;

        public EPlayerType PlayerType { get; set; }
        
        public ICollection<GameShip>? GameShips { get; set; } = null!;

        public ICollection<PlayerBoardState>? PlayerBoardStates { get; set; } = null!;
    }
}