using Domain.Enums;

namespace GameBrain
{
    public class GameBoardState
    {
        public ECellState[][] Board { get; set; } = null!;
    }
}