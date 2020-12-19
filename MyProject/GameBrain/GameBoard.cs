using Domain.Enums;

namespace GameBrain
{
    public class GameBoard
    {
        public int Width { get; }
        public int Height { get; }

        public ECellState[,] Board { get; }

        public GameBoard(int width, int height)
        {
            Width = width;
            Height = height;
            Board = new ECellState[width, height];
        }
    }
}