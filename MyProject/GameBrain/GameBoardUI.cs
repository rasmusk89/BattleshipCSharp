﻿using System;
using Domain.Enums;

namespace GameBrain
{
    public static class GameBoardUI
    {
        public static void DrawPlayerBoard(Player player)
        {
            var board = player.GetPlayerBoard();
            var width = board.GetUpperBound(0) + 1;
            var height = board.GetUpperBound(1) + 1;

            Console.WriteLine($"    {player.Name} board".ToUpper());
            Console.Write("   ");

            // First line
            for (var i = 0; i < width; i++)
            {
                Console.Write(IntToAlphabeticValue(i).Length < 2
                    ? $"{IntToAlphabeticValue(i)}  "
                    : $"{IntToAlphabeticValue(i)} ");
            }

            // Middle part
            Console.WriteLine();

            for (var i = 0; i < height; i++)
            {
                Console.Write(i < 9 ? $" {(i + 1).ToString()} " : $"{(i + 1).ToString()} ");
                for (var j = 0; j < width; j++)
                {
                    if (board[j, i] == ECellState.Bomb)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{CellString(board[j, i])}  ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    if (IsBoat(board[j, i]))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{CellString(board[j, i])}  ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    if (board[j, i] == ECellState.Empty)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"{CellString(board[j, i])}  ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    if (board[j, i] == ECellState.Hit)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"{CellString(board[j, i])}  ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                }

                Console.Write($"{(i + 1).ToString()}");
                Console.WriteLine();
            }

            // Last line
            Console.Write("   ");
            for (var i = 0; i < width; i++)
            {
                Console.Write(IntToAlphabeticValue(i).Length < 2
                    ? $"{IntToAlphabeticValue(i)}  "
                    : $"{IntToAlphabeticValue(i)} ");
            }

            Console.WriteLine();
        }

        private static void DrawOpponentBoard(Player opponent)
        {
            var board = opponent.GetPlayerBoard();
            var width = board.GetUpperBound(0) + 1;
            var height = board.GetUpperBound(1) + 1;

            Console.WriteLine($"    opponent board".ToUpper());
            Console.Write("   ");

            // First line
            for (var i = 0; i < width; i++)
            {
                Console.Write(IntToAlphabeticValue(i).Length < 2
                    ? $"{IntToAlphabeticValue(i)}  "
                    : $"{IntToAlphabeticValue(i)} ");
            }

            // Middle part
            Console.WriteLine();

            for (var i = 0; i < height; i++)
            {
                Console.Write(i < 9 ? $" {(i + 1).ToString()} " : $"{(i + 1).ToString()} ");
                for (var j = 0; j < width; j++)
                {
                    if (board[j, i] == ECellState.Bomb)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("O  ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    if (board[j, i] == ECellState.Empty || IsBoat(board[j, i]))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("~  ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    if (board[j, i] != ECellState.Hit) continue;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("X  ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.Write($"{(i + 1).ToString()}");
                Console.WriteLine();
            }

            // Last line
            Console.Write("   ");
            for (var i = 0; i < width; i++)
            {
                Console.Write(IntToAlphabeticValue(i).Length < 2
                    ? $"{IntToAlphabeticValue(i)}  "
                    : $"{IntToAlphabeticValue(i)} ");
            }

            Console.WriteLine();
        }

        public static void DrawBoards(Player player, Player opponent)
        {
            DrawPlayerBoard(player);
            Console.WriteLine();
            DrawOpponentBoard(opponent);
        }

        private static string CellString(ECellState cellState)
        {
            return cellState switch
            {
                ECellState.Empty => "~",
                ECellState.Bomb => "O",
                ECellState.Hit => "X",
                ECellState.Patrol => "P",
                ECellState.Cruiser => "C",
                ECellState.Submarine => "S",
                ECellState.Battleship => "B",
                ECellState.Carrier => "A",
                ECellState.Custom0 => "0",
                ECellState.Custom1 => "1",
                ECellState.Custom2 => "2",
                ECellState.Custom3 => "3",
                ECellState.Custom4 => "4",
                ECellState.Custom5 => "5",
                ECellState.Custom6 => "6",
                ECellState.Custom7 => "7",
                ECellState.Custom8 => "8",
                ECellState.Custom9 => "9",
                _ => "-"
            };
        }

        private static bool IsBoat(ECellState state)
        {
            return state != ECellState.Empty && state != ECellState.Hit && state != ECellState.Bomb;
        }

        private static string IntToAlphabeticValue(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }
    }
}