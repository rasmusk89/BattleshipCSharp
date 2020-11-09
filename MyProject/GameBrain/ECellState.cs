using System.ComponentModel;

namespace GameBrain
{
    public enum ECellState
    {
        [Description("~")]
        Empty,
        [Description("O")]
        Bomb,
        [Description("X")]
        Hit,
        [Description("B")]
        Boat,
        [Description("P")]
        Patrol,
        [Description("C")]
        Cruiser,
        [Description("S")]
        Submarine,
        [Description("B")]
        Battleship,
        [Description("A")]
        Carrier,
    }
}