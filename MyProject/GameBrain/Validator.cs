using System;
using Domain.Enums;

namespace GameBrain
{
    public class Validator
    {
        public bool BombCoordinateFree(Coordinates coordinates, FiringBoard board)
        {
            var row = coordinates.Row - 1;
            var column = coordinates.Column - 1;

            return board.Board[column, row] == ECellState.Empty;
        }


        public bool OrientationIsValid(string input)
        {
            return input.ToLower() == "h" || input.ToLower() == "v";
        }

        public bool RowIsValid(string input, GameBoard board)
        {
            var boardWidth = board.Board.GetUpperBound(0) + 1;

            if (IsNumeric(input))
            {
                return int.Parse(input) <= boardWidth && int.Parse(input) > 0;
            }

            return false;
        }

        public bool ColumnIsValid(string input, GameBoard board)
        {
            if (input.Length < 1)
            {
                return false;
            }

            if (input.Contains("-") || input.Contains("+") || input.Contains("*") || input.Contains("/"))
            {
                return false;
            }

            var boardHeight = board.Board.GetUpperBound(1) + 1;

            var number = ConvertStringToInteger(input.ToUpper());

            return number <= boardHeight && number > 0;
        }

        public bool CoordinatesAreValid(Coordinates coordinates, EOrientation orientation, GameBoard board,
            int shipSize)
        {
            var boardWidth = board.Board.GetUpperBound(0) + 1;
            var boardHeight = board.Board.GetUpperBound(1) + 1;

            var column = coordinates.Column - 1;
            var row = coordinates.Row - 1;
            if (orientation == EOrientation.Horizontal)
            {
                return column + shipSize <= boardWidth;
            }

            return row + shipSize <= boardHeight;
        }

        public bool AreaFree(Coordinates coordinates, EOrientation orientation, GameBoard board, int shipSize)
        {
            var row = coordinates.Row - 1;
            var column = coordinates.Column - 1;


            var occupiedCells = 0;

            if (orientation == EOrientation.Horizontal)
            {
                for (var i = 0; i < shipSize; i++)
                {
                    if (board.Board[column + i, row] == ECellState.Empty)
                    {
                        continue;
                    }
                    occupiedCells++;
                }
            }
            else
            {
                for (var i = 0; i < shipSize; i++)
                {
                    if (board.Board[column, row + i] == ECellState.Empty)
                    {
                        continue;
                    }
                    occupiedCells++;

                }
            }

            return occupiedCells == 0;
        }

        public bool IsNumeric(string x)
        {
            return int.TryParse(x, out _);
        }

        public int ConvertStringToInteger(string input)
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