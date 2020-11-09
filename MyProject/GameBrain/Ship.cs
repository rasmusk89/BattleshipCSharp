using System;
using System.Collections;

namespace GameBrain
{
    public class Ship
    {
        public string Name { get; set; }

        public int Width { get; set; }

        public int Hits { get; set; }

        public readonly ECellState CellState;

        public bool IsSunk => Hits >= Width;

        public Ship(int width)
        {
            Width = width;

            CellState = width switch
            {
                1 => ECellState.Patrol,
                2 => ECellState.Cruiser,
                3 => ECellState.Submarine,
                4 => ECellState.Battleship,
                5 => ECellState.Carrier,
                _ => throw new Exception("Incorrect ship width!")
            };
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