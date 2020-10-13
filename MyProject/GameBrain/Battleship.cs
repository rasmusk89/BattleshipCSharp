using System;
using System.Linq;

namespace GameBrain
{
    public class Battleship
    {
        private static readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private const int Size = 10;
        private static readonly CellState[,] BoardA = new CellState[Size, Size];
        private static readonly CellState[,] BoardB = new CellState[Size, Size];

        public bool NextMoveByPlayerA = true;

        public static CellState[,] GetBoardA()
        {
            var board = new CellState[Size, Size];
            Array.Copy(BoardA, board, BoardA.Length);
            return board;
        }

        public static CellState[,] GetBoardB()
        {
            var board = new CellState[Size, Size];
            Array.Copy(BoardB, board, BoardB.Length);
            return board;
        }

        public void MakeAMove(int x, int y)
        {
            if (NextMoveByPlayerA)
            {
                if (BoardA[x, y] != CellState.Empty)
                {
                    Console.WriteLine("Bomb has already placed there. Choose another coordinate!");
                    return;
                }

                BoardA[x, y] = CellState.X;
                NextMoveByPlayerA = !NextMoveByPlayerA;
            }
            else
            {
                if (BoardB[x, y] != CellState.Empty)
                {
                    Console.WriteLine("Bomb has already placed there. Choose another coordinate!");
                    return;
                }

                BoardB[x, y] = CellState.X;
                NextMoveByPlayerA = true;
            }
        }

        private static bool IsNumeric(string x)
        {
            return int.TryParse(x, out _);
        }

        public static (int x, int y) GetMoveCoordinates()
        {
            while (true)
            {
                Console.Write("Give bomb coordinates separated with comma (example - A,1): ");

                var userInput = Console.ReadLine().Split(",");
                if (userInput.Length < 2)
                {
                    Console.WriteLine("Please enter correct coordinate");
                }
                else if (userInput[0].Length > 1 || userInput[0].Length < 0)
                {
                    Console.WriteLine("Please enter correct coordinate");
                }
                else if (!Letters.Contains(char.Parse(userInput[0].ToUpper())))
                {
                    Console.WriteLine("Please enter correct coordinate");
                }
                else if (!IsNumeric(userInput[1]))
                {
                    Console.WriteLine("Please enter correct coordinate");
                } else if (int.Parse(userInput[1]) < 1 || int.Parse(userInput[1]) > Size)
                {
                    Console.WriteLine("Please enter correct coordinate");
                }
                else
                {
                    var y = char.Parse(userInput[0].ToUpper()) - 65;
                    var x = int.Parse(userInput[1]) - 1;
                    return (x, y);
                }
            }
        }
    }
}