using Domain.Enums;

namespace GameBrain
{
    public class Ship
    {
        public string Name { get; set; }

        public int Width { get; set; }

        public int Hits { get; set; }

        public ECellState CellState;

        public bool IsSunk => Hits >= Width;

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
                6 => "Custom0",
                7 => "Custom1",
                8 => "Custom2",
                9 => "Custom3",
                10 => "Custom4",
                11 => "Custom5",
                12 => "Custom6",
                13 => "Custom7",
                14 => "Custom8",
                15 => "Custom9",
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
                6 => ECellState.Custom0,
                7 => ECellState.Custom1,
                8 => ECellState.Custom2,
                9 => ECellState.Custom3,
                10 => ECellState.Custom4,
                11 => ECellState.Custom5,
                12 => ECellState.Custom6,
                13 => ECellState.Custom7,
                14 => ECellState.Custom8,
                15 => ECellState.Custom9,
                _ => ECellState.Custom
            };
        }
    }
}