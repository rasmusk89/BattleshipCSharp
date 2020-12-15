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
        [Description("0")] Custom0,
        [Description("1")] Custom1,
        [Description("2")] Custom2,
        [Description("3")] Custom3,
        [Description("4")] Custom4,
        [Description("5")] Custom5,
        [Description("6")] Custom6,
        [Description("7")] Custom7,
        [Description("8")] Custom8,
        [Description("9")] Custom9,
        [Description("X")] Custom,
    }
}