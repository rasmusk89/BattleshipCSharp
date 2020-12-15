using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain
{
    public class Player
    {
        public int PlayerId { get; set; }

        [MaxLength(128)]
        [Display(Name="Player Name")]
        public string Name { get; set; } = null!;

        [Display(Name="Player Type")]
        public EPlayerType PlayerType { get; set; }
        
        public ICollection<GameShip>? GameShips { get; set; }

        // public ICollection<PlayerBoardState>? PlayerBoardStates { get; set; }
    }
}