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

        public EPlayerType EPlayerType { get; set; }

        public int? GameAId { get; set; }
        public Game GameA { get; set; } = null!;
        public int? GameBId { get; set; }
        public Game GameB { get; set; } = null!;
        
        
        
        
        public ICollection<GameShip> GameBoats { get; set; } = null!;

        public ICollection<PlayerBoardState> PlayerBoardStates { get; set; } = null!;


        public override string ToString()
        {
            return PlayerId + ": " + Name;
        }
    }
}