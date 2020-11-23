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
        {
        }

        public Ship(int width)
        {
            Width = width;
            CellState = GetCellState(width);
            Name = CreateName(width);
        }

        private static string CreateName(int width)
        {
            return width switch
            {
                1 => "Patrol",
                2 => "Cruiser",
                3 => "Submarine",
                4 => "Battleship",
                5 => "Carrier",
                _ => "Custom"
            };
        }

        private static ECellState GetCellState(int width)
        {
            return width switch
            {
                1 => ECellState.Patrol,
                2 => ECellState.Cruiser,
                3 => ECellState.Submarine,
                4 => ECellState.Battleship,
                5 => ECellState.Carrier,
                _ => ECellState.Custom
            };
        }
    }
}