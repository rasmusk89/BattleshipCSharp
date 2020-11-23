using Domain.Enums;

namespace GameBrain
{
    public class FiringBoardState
    {
        public ECellState[][] OpponentBoard { get; set; } = null!;
    }
}