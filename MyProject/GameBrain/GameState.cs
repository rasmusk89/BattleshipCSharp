using System.Collections.Generic;

namespace GameBrain
{
    public class GameState
    {
        public bool NextMoveByPlayerA { get; set; }
        
        public int Width { get; set; }

        public int Height { get; set; }

        public string? PlayerAName { get; set; }
        
        public string? PlayerBName { get; set; }

        public List<Ship>? PlayerAShips { get; set; }
        
        public List<Ship>? PlayerBShips { get; set; }
        public ECellState[][] PlayerAPlayerBoard { get; set; } = null!;
        public ECellState[][] PlayerAFiringBoard { get; set; } = null!;
        
        public ECellState[][] PlayerBPlayerBoard { get; set; } = null!;
        public ECellState[][] PlayerBFiringBoard { get; set; } = null!;

        

    }
}