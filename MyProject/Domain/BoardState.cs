using Domain.Enums;

namespace Domain
{
    public class BoardState
    {
        public ECellState[,] Board { get; set; } = null!;
    }
}