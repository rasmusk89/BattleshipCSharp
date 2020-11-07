using System;
using System.Collections;

namespace GameBrain
{
    public class Ship
    {
        public string Name { get; set; }

        public int Width { get; set; }

        private int Hits { get; set; }

        public bool IsSunk => Hits >= Width;

        public Ship(int width)
        {
            Width = width;
            Name = CreateName(width);

        }

        private static string CreateName(int width)
        {
            switch (width)
            {
                case 1:
                    return "Patrol";
;                case 2:
                    return "Cruiser";
                case 3:
                    return "Submarine";
                case 4:
                    return "Battleship";
                case 5:
                    return "Carrier";
                default:
                    throw new Exception("Incorrect ship width!");
            }
        }
    }
    
}