using System;

namespace GameBrain
{
    public class Battleship
    {
        private static int _width = 8;
        private static int _height = 8;
        
        private readonly CellState[,] _board = new CellState[_width, _height];
        public bool NextMoveByPlayerA = true;

        public CellState[,] GetBoard()
        {
            var res = new CellState[_width,_height];
            Array.Copy(_board, res, _board.Length );
            return res;
        }
        
        public bool MakeAMove(int x, int y)
        {
            if (_board[x, y] != CellState.Empty) return false;
            _board[x, y] = CellState.X;
            NextMoveByPlayerA = !NextMoveByPlayerA;
            return true;
        }
      
    }
}