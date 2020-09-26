using System;

namespace GameBrain
{
    // _, X, O, /X, /O
    public class TicTacToe
    {
        private readonly CellState[,] _board = new CellState[3, 3];

        public CellState[,] GetBoard()
        {
            var res = new CellState[3, 3];
            Array.Copy(_board, res, _board.Length);
            return res;
        }
    }
}