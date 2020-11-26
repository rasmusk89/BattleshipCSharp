using System;
using Domain.Enums;

namespace GameBrain
{
    public class Validator
    {

        public bool OrientationIsValid(string input)
        {
            return input.ToLower() == "h" || input.ToLower() == "v";
        }

        public bool ColumnInputIsValid(string input, int boardWidth)
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

        public bool RowInputIsValid(string input, int boardHeight)
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
        
        public bool ShipCoordinatesAreValid(int column, int row, int boardWidth, int boardHeight, Ship ship,
            EOrientation orientation)
        {
            var x = column;
            var y = row;
            if (orientation == EOrientation.Horizontal)
            {
                return x + ship.Width <= boardWidth;
            }

            return y + ship.Width <= boardHeight;
        }
        
        public bool BombCoordinatesAreValid(int column, int row, int boardWidth, int boardHeight, Player opponent)
        {
            var sizeValid = column >= 0 && column <= boardWidth && row >= 0 && row <= boardHeight;
            var noBomb = opponent.GetPlayerBoard()[column, row] == ECellState.Empty 
                || opponent.GetPlayerBoard()[column, row] != ECellState.Bomb
                && opponent.GetPlayerBoard()[column, row] != ECellState.Hit;

            return sizeValid && noBomb;
        }

        public bool ShipAreaFree(int column, int row, Player player, Ship ship, EOrientation orientation,
            EShipsCanTouch shipsCanTouch)
        {
            var board = player.GetPlayerBoard();
            var boardWidth = board.GetUpperBound(0);
            var boardHeight = board.GetUpperBound(1);
            var startColumn = column;
            var startRow = row;
            var endColumn = startColumn;
            var endRow = startRow;
            var occupiedCells = 0;

            if (orientation == EOrientation.Horizontal)
            {
                for (var i = 1; i < ship.Width; i++)
                {
                    endColumn++;
                }
            }

            if (orientation == EOrientation.Vertical)
            {
                for (var i = 1; i < ship.Width; i++)
                {
                    endRow++;
                }
            }

            if (shipsCanTouch == EShipsCanTouch.Yes)
            {
                if (ship.Width == 1)
                {
                    return board[column, row] == ECellState.Empty;
                }

                for (var i = startColumn; i <= endColumn; i++)
                {
                    for (var j = startRow; j <= endRow; j++)
                    {
                        if (board[i, j] == ECellState.Empty)
                        {
                            continue;
                        }

                        occupiedCells++;
                    }
                }
            }

            if (shipsCanTouch == EShipsCanTouch.No)
            {
                if (startColumn < 1)
                {
                    ++startColumn;
                    ++endColumn;
                }
            
                if (startRow < 1)
                {
                    ++startRow;
                    ++endRow;
                }

                if (startColumn >= boardWidth)
                {
                    --startColumn;
                    endColumn = startColumn;
                }

                if (startRow >= boardWidth)
                {
                    --endRow;
                    endRow = startRow;
                }
            
                if (endColumn >= boardWidth)
                {
                    --endColumn;
                }
                
                if (endRow >= boardHeight)
                {
                    --endRow;
                }

                for (var i = startColumn - 1; i <= endColumn + 1; i++)
                {
                    for (var j = startRow - 1; j <= endRow + 1; j++)
                    {
                        if (board[i, j] == ECellState.Empty)
                        {
                            continue;
                        }

                        occupiedCells++;
                    }
                }
            }

            Console.WriteLine();

            // if (shipsCanTouch == EShipsCanTouch.Yes)
            // {
            //     if (orientation == EOrientation.Horizontal)
            //     {
            //         for (var i = 0; i < ship.Width; i++)
            //         {
            //             if (player.GetPlayerBoard()[x + i, y] == ECellState.Empty)
            //             {
            //                 continue;
            //             }
            //
            //             occupiedCells++;
            //         }
            //     }
            //
            //     if (orientation == EOrientation.Vertical)
            //     {
            //         for (var i = 0; i < ship.Width; i++)
            //         {
            //             if (player.GetPlayerBoard()[x, y + i] == ECellState.Empty)
            //             {
            //                 continue;
            //             }
            //
            //             occupiedCells++;
            //         }
            //     }
            // }
            //
            // if (shipsCanTouch == EShipsCanTouch.No)
            // {
            //     if (orientation == EOrientation.Horizontal)
            //     {
            //         for (var i = 0; i < 4; i++)
            //         {
            //             for (var j = 0; j < ship.Width + 2; j++)
            //             {
            //                 if (player.GetPlayerBoard()[x - 1 + j, y - 1 + i] == ECellState.Empty)
            //                 {
            //                     continue;
            //                 }
            //
            //                 occupiedCells++;
            //             }
            //         }
            //     }
            //
            //     if (orientation == EOrientation.Vertical)
            //     {
            //         for (var i = 0; i < 4; i++)
            //         {
            //             for (var j = 0; j < ship.Width + 2; j++)
            //             {
            //                 if (player.GetPlayerBoard()[x - 1 + i, y - 1 + j] == ECellState.Empty)
            //                 {
            //                     continue;
            //                 }
            //
            //                 occupiedCells++;
            //             }
            //         }
            //     }
            //
            //     if (shipsCanTouch == EShipsCanTouch.Corner)
            //     {
            //         if (orientation == EOrientation.Horizontal)
            //         {
            //             if (player.GetPlayerBoard()[x - 1, y] != ECellState.Empty ||
            //                 player.GetPlayerBoard()[x + ship.Width + 1, y] != ECellState.Empty)
            //             {
            //                 occupiedCells++;
            //             }
            //
            //             for (var i = 0; i < ship.Width; i++)
            //             {
            //                 for (var j = 0; j < 3; j++)
            //                 {
            //                     if (player.GetPlayerBoard()[x + i, y - 1 + j] != ECellState.Empty)
            //                     {
            //                         occupiedCells++;
            //                     }
            //                         
            //                 }
            //             }
            //         }
            //     }
            // }

            return occupiedCells == 0;

            // switch (shipsCanTouch)
            // {
            //     case EShipsCanTouch.Yes:
            //     {
            //         if (orientation == EOrientation.Horizontal)
            //         {
            //             for (var i = column; i < ship.Width; i++)
            //             {
            //                 if (player.GetPlayerBoard()[i, row] == ECellState.Empty)
            //                 {
            //                     continue;
            //                 }
            //
            //                 occupiedCells++;
            //             }
            //         }
            //         else
            //         {
            //             for (var i = row; i < ship.Width; i++)
            //             {
            //                 if (player.GetPlayerBoard()[column, i] == ECellState.Empty)
            //                 {
            //                     continue;
            //                 }
            //
            //                 occupiedCells++;
            //             }
            //         }
            //
            //         return occupiedCells == 0;
            //     }
            //     case EShipsCanTouch.No:
            //     {
            //         if (orientation == EOrientation.Horizontal)
            //         {
            //             for (var i = column - 1; i < ship.Width + 1; i++)
            //             {
            //                 for (var j = row - 1; j < row + 1; j++)
            //                 {
            //                     if (player.GetPlayerBoard()[i, j] == ECellState.Empty)
            //                     {
            //                         continue;
            //                     }
            //
            //                     occupiedCells++;
            //                 }
            //             }
            //         }
            //         else
            //         {
            //             for (var i = 0; i < ship.Width; i++)
            //             {
            //                 if (player.GetPlayerBoard()[column, row + i] == ECellState.Empty)
            //                 {
            //                     continue;
            //                 }
            //
            //                 occupiedCells++;
            //             }
            //         }
            //
            //         return occupiedCells == 0;
            //     }
            //     default:
            //         return true;
            // }
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