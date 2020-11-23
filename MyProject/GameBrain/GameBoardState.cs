using Domain.Enums;

namespace GameBrain
{
    public class GameBoardState
    {
        public ECellState[][] PlayerBoard { get; set; } = null!;
    }
}