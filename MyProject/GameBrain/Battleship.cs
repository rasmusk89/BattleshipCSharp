using System;

namespace GameBrain
{
    public class Battleship
    {
        private int _width;
        private int _height;

        public bool NextMoveByPlayerA = true;

        public Battleship(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}