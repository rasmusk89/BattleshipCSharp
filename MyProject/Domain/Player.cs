using System.Collections.Generic;

namespace AnotherDomain
{
    public class Player
    {
        public int PlayerId { get; set; }

        public string Name { get; set; } = null!;

        public int PlayerBoardId { get; set; }
        public GameBoard PlayerBoard { get; set; } = null!;

        public int OpponentBoardId { get; set; }
        public GameBoard OpponentBoard { get; set; } = null!;
        
        public int? GameAId { get; set; }
        public Game GameA { get; set; } = null!;
        
        public int? GameBId { get; set; }
        public Game GameB { get; set; } = null!;
        
        public ICollection<Ship> GameShips { get; set; } = null!;

        public ICollection<PlayerBoardState> PlayerBoardStates { get; set; } = null!;

        public override string ToString()
        {
            return "NewPLayer!";
        }
    }
}