using System;
using System.Drawing;

namespace GameBrain
{
    public class Battleship
    {

        private static int _size = 5;

        private readonly CellState[,] _board = new CellState[_size, _size];
        public bool NextMoveByPlayerA = true;

        public CellState[,] GetBoard()
        {
            var res = new CellState[_size, _size];
            Array.Copy(_board, res, _board.Length );
            return res;
        }
        
        public void MakeAMove(int x, int y)
        {
            if (_board[x, y] != CellState.Empty)
            {
                Console.WriteLine("Bomb has already placed there. Choose another coordinate!");
                return;
            }
            _board[x, y] = CellState.X;
            NextMoveByPlayerA = !NextMoveByPlayerA;
        }

        public static void SetSize(int size)
        {
            _size = size;
        }
        
    }
}