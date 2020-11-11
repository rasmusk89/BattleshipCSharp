using System;
using System.Collections.Generic;

namespace GameBrain
{
    public class GameOptions
    {

        public int BoardWidth { get; set; } = 10;
        public int BoardHeight { get; set; } = 10;
        public bool BoatsCanTouch { get; set; } = true;
        public string PlayerAName { get; set; } = "Player A";
        public string PlayerBName { get; set; } = "Player B";
        public List<Ship> Ships { get; set; } = new List<Ship>
        {
            new Ship(1),
            new Ship(2),
            new Ship(3),
            new Ship(4),
            new Ship(5)
        };


    }
}