using System.ComponentModel;

namespace Domain.Enums
{
    public enum ECellState
    {
        [Description("~")] Empty,
        [Description("O")] Bomb,
        [Description("X")] Hit,
        [Description("P")] Patrol,
        [Description("C")] Cruiser,
        [Description("S")] Submarine,
        [Description("B")] Battleship,
        [Description("A")] Carrier,
        [Description("B")] Boat,
        [Description("Z")] Custom,
    }
}