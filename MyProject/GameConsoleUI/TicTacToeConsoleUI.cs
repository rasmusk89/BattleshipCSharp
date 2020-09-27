using System;
using GameBrain;

namespace GameConsoleUI
{
    public static class TicTacToeConsoleUi
    {

        public static void DrawBoard(CellState[,] board)
        {
            // Add +1, since this is 0 based
            var width = board.GetUpperBound(0) + 1; // x
            var height = board.GetUpperBound(1) + 1; // y

            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"| {CellString(board[colIndex, rowIndex])} |");
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();

            }
            
        }

        private static string CellString(CellState cellState)
        {
            return cellState switch
            {
                CellState.Empty => " ",
                CellState.O => "O",
                CellState.X => "X",
                _ => "-"
            };
        }
        
    }
}