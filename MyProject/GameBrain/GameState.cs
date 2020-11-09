﻿namespace GameBrain
{
    public class GameState
    {
        public bool NextMoveByPlayerA { get; set; }
        
        public int Width { get; set; }

        public int Height { get; set; }
        
        public ECellState[][] PlayerAPlayerBoard { get; set; } = null!;
        public ECellState[][] PlayerAFiringBoard { get; set; } = null!;
        
        public ECellState[][] PlayerBPlayerBoard { get; set; } = null!;
        public ECellState[][] PlayerBFiringBoard { get; set; } = null!;

        

    }
}