using System;
using System.Collections;
using Domain.Enums;

namespace GameBrain
{
    public class Ship
    {
        public string Name { get; set; } = null!;

        public int Width { get; set; }

        public int Hits { get; set; }

        public ECellState CellState;

        public bool IsSunk => Hits >= Width;

        public Ship()
        {}
        public Ship(int width)
        {
            Width = width;

            CellState = width switch
            {
                1 => Domain.Enums.ECellState.Patrol,
                2 => Domain.Enums.ECellState.Cruiser,
                3 => Domain.Enums.ECellState.Submarine,
                4 => Domain.Enums.ECellState.Battleship,
                5 => Domain.Enums.ECellState.Carrier,
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