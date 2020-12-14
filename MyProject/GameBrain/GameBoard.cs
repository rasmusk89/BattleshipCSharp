using Domain.Enums;

namespace GameBrain
{
    public class GameBoard
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public ECellState[,] Board { get; set; }

        public GameBoard(int width, int height)
        {
            Width = width;
            Height = height;
            Board = new ECellState[width, height];
        }
    }
}