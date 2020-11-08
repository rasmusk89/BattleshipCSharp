using System.Collections.Generic;
using System.Linq;

namespace GameBrain
{
    public class GameBoard
    {
        private int Width { get; set; }
        private int Height { get; set; }

        public ECellState[,] Board { get; set; }

        public GameBoard(int width, int height)
        {
            Width = width;
            Height = height;
            Board = new ECellState[width, height];
        }

        public static List<Cell> Range(IEnumerable<Cell> cells,
            int startRow,
            int startColumn,
            int endRow,
            int endColumn)
        {
            return cells.Where(x => x.Coordinates.Row >= startRow
                                     && x.Coordinates.Column >= startColumn
                                     && x.Coordinates.Row <= endRow
                                     && x.Coordinates.Column <= endColumn).ToList();
        }
    }
}