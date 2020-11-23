using Domain.Enums;

namespace Domain
{
    public class GameBoard
    {
        public ECellState[,] Board { get; set; }  = null!;
    }
}