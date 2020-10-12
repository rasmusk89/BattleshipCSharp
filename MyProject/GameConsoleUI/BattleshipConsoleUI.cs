using System;
using GameBrain;

namespace GameConsoleUI
{
    public static class BattleshipConsoleUI
    {
        private static readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static int Numbers = 1;

        public static void DrawBoard(CellState[,] board)
        {
            var width = board.GetUpperBound(0) + 1; // x
            var height = board.GetUpperBound(1) + 1; // y

            Console.Write("   ");
            
            // Need to figure out what to do if board size is bigger than number of letters.
            for (var i = 0; i < width; i++)
            {
                Console.Write($"{Letters[i]} ");
            }

            Console.WriteLine();

            for (var i = 0; i < height; i++)
            {
                Console.Write(i < 9 ? $" {(i + 1).ToString()} " : $"{(i + 1).ToString()} ");
                for (var j = 0; j < width; j++)
                {
                    Console.Write($"{CellString(board[i,j])} ");
                }
                Console.Write($"{(i + 1).ToString()}");
                Console.WriteLine();
            }

            Console.Write("   ");
            for (var i = 0; i < width; i++)
            {
                Console.Write($"{Letters[i]} ");
                
            }

            Console.WriteLine();
        }

        private static string CellString(CellState cellState)
        {
            return cellState switch
            {
                CellState.Empty => "~",
                CellState.X => "X",
                _ => "-"
            };
        }
    }
}