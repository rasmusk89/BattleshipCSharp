using System;

namespace GameBrain
{
    public class Battleship
    {
        private static int _width = 8;
        private static int _height = 8;
        
        private CellState[,] _board = new CellState[_width, _height];
        private bool _nextMoveByPlayerA = true;

        public CellState[,] GetBoard()
        {
            var res = new CellState[_width,_height];
            Array.Copy(_board, res, _board.Length );
            return res;
        }
        
    }
}