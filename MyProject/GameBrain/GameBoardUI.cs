﻿using System;

namespace GameBrain
{
    public class GameBoardUI
    {
        
        private static readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        // public void DrawBoard(ECellState[,] board, bool includeShips=true)
        public void DrawBoard(GameBoard gameBoard, bool includeShips=true)
        {
            var board = gameBoard.Board;
            var width = board.GetUpperBound(0) + 1;
            var height = board.GetUpperBound(1) + 1;
            
            Console.Write("   ");
            
            // First line
            for (var i = 0; i < width; i++)
            {
                Console.Write($"{Letters[i]} ");
            }

            // Middle part
            Console.WriteLine();

            for (var i = 0; i < height; i++)
            {
                Console.Write(i < 9 ? $" {(i + 1).ToString()} " : $"{(i + 1).ToString()} ");
                for (var j = 0; j < width; j++)
                {
                    
                    Console.Write($"{CellString(board[j,i])} ");
                }
                Console.Write($"{(i + 1).ToString()}");
                Console.WriteLine();
            }

            // Last line
            Console.Write("   ");
            for (var i = 0; i < width; i++)
            {
                Console.Write($"{Letters[i]} ");
                
            }

            Console.WriteLine();

        }

        private static string CellString(ECellState cellState)
        {
            return cellState switch
            {
                ECellState.Empty => "~",
                ECellState.Bomb => "X",
                ECellState.Boat => "B",
                _ => "-"
            };
        }
    }
}