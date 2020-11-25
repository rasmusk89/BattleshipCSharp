using System;
using Domain.Enums;

namespace GameBrain
{
    public class Validator
    {
        public bool BombCoordinateFree(int x, int y, ECellState[,] board)
        {
            var row = x - 1;
            var column = y - 1;

            return board[column, row] == ECellState.Empty;
        }

        public static bool OrientationIsValid(string input)
        {
            return input.ToLower() == "h" || input.ToLower() == "v";
        }

        public bool RowIsValid(string input, int boardHeight)
        {
            if (input.Contains("-") || input.Contains("+") || input.Contains("*") || input.Contains("/"))
            {
                return false;
            }

            if (IsNumeric(input))
            {
                return int.Parse(input) <= boardHeight && int.Parse(input) > 0;
            }

            return false;
        }

        public bool ColumnIsValid(string input, int boardWidth)
        {
            if (input.Length < 1)
            {
                return false;
            }

            if (input.Contains("-") || input.Contains("+") || input.Contains("*") || input.Contains("/"))
            {
                return false;
            }

            var number = ConvertStringToInteger(input.ToUpper());

            return number <= boardWidth && number > 0;
        }

        public bool CoordinatesAreValid(int x, int y, int boardWidth, int boardHeight, Ship ship,
            EOrientation orientation)
        {
            var column = x - 1;
            var row = y - 1;
            if (orientation == EOrientation.Horizontal)
            {
                return column + ship.Width <= boardWidth;
            }

            return row + ship.Width <= boardHeight;
        }

        public bool ShipAreaFree(int x, int y, Player player,Ship ship, EOrientation orientation)
        {
            var column = x - 1;
            var row = y - 1;
            var occupiedCells = 0;

            if (orientation == EOrientation.Horizontal)
            {
                for (var i = 0; i < ship.Width; i++)
                {
                    if (player.GetPlayerBoard()[column + i, row] == ECellState.Empty)
                    {
                        continue;
                    }
                    occupiedCells++;
                }
            }
            else
            {
                for (var i = 0; i < ship.Width; i++)
                {
                    if (player.GetPlayerBoard()[column, row + i] == ECellState.Empty)
                    {
                        continue;
                    }
                    occupiedCells++;
                }
            }

            return occupiedCells == 0;
        }

        private bool IsNumeric(string x)
        {
            return int.TryParse(x, out _);
        }

        public static int ConvertStringToInteger(string input)
        {
            int numericValueOfString;

            if (input.Length > 1)
            {
                char[] listOfInput = input.ToCharArray();
                numericValueOfString = 26 + char.ToUpper(listOfInput[1]) - 64;
            }
            else
            {
                var c = char.Parse(input);
                numericValueOfString = char.ToUpper(c) - 64;
            }

            return numericValueOfString;
        }
    }
}