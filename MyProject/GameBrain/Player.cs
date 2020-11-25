using System;
using System.Collections.Generic;
using System.Text.Json;
using Domain.Enums;

namespace GameBrain
{
    public class Player
    {
        public string Name { get; set; } = "Player";

        public EPlayerType PlayerType { get; set; }

        public List<Ship> Ships { get; set; } = new();

        public GameBoard PlayerBoard { get; set; } = null!;

        public GameBoard FiringBoard { get; set; } = null!;

        public bool HasLost { get; set; } = false;

        private readonly Validator _validator = new();

        // To game options
        public EShipsCanTouch ShipsCanTouch { get; set; }
        // public FiringBoard OpponentBoard { get; set; }

        // public bool HasLost
        // {
        //     get { return Ships.All(x => x.IsSunk); }
        // }

        public Player()
        {
        }

        public Player(string name)
        {
            Name = name;
        }

        public void SetBoards(int width, int height)
        {
            PlayerBoard = new GameBoard(width, height);
            FiringBoard = new GameBoard(width, height);
        }

        public ECellState[,] GetPlayerBoard()
        {
            return PlayerBoard.Board;
        }

        public ECellState[,] GetFiringBoard()
        {
            return FiringBoard.Board;
        }

        public List<Ship> GetShips()
        {
            return Ships;
        }

        public void SetShips(List<Ship> ships)
        {
            Ships = ships;
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void PlaceBomb(Player opponent, int x, int y)
        {
            opponent.GetPlayerBoard()[x, y] = ECellState.Bomb;


            // // var opponentShips = opponent.Ships;
            // // var opponentBoard = opponent.GetPlayerBoard();
            // // var playerFiringBoard = FiringBoard.Board;
            // //
            // // DrawGameUI();
            // // Console.WriteLine($"{Name}, place Bomb!");
            // // Coordinates coordinates = AskCoordinates();
            // // while (!_validator.BombCoordinateFree(coordinates, opponentBoard))
            // // {
            // //     Console.ForegroundColor = ConsoleColor.Red;
            // //     Console.WriteLine("Bomb has already placed there!");
            // //     Console.ForegroundColor = ConsoleColor.Cyan;
            // //     coordinates = AskCoordinates();
            // // }
            // //
            // // var column = coordinates.Column - 1;
            // // var row = coordinates.Row - 1;
            // //
            // // if (opponentBoard[column, row] != ECellState.Empty)
            // // {
            // //     var cellState = opponentBoard[column, row];
            // //
            // //     foreach (var opponentShip in opponent.Ships.Where(opponentShip => opponentShip.CellState == cellState))
            // //     {
            // //         opponentShip.Hits++;
            // //         if (opponentShip.IsSunk)
            // //         {
            // //             Console.WriteLine($"You sunk opponents {opponentShip.Name}");
            // //         }
            // //         break;
            // //     }
            // //     playerFiringBoard[column, row] = opponentBoard[column, row] = ECellState.Hit;
            // //     DrawGameUI();
            // //     Console.WriteLine("HIT!");
            // //     
            // //
            // //     Console.Write("Continue...");
            // //     Console.ReadLine();
            // }
            //
            // if (opponentBoard[column, row] != ECellState.Empty) return;
            // playerFiringBoard[column, row] = ECellState.Bomb;
            // opponentBoard[column, row] = ECellState.Bomb;
            // DrawGameUI();
            // Console.WriteLine("MISS!");
            // Console.Write("Continue...");
            // Console.ReadLine();
        }

        // public void PlaceShips()
        // {
        //     foreach (var ship in Ships)
        //     {
        //         DrawGameUI();
        //
        //         Console.WriteLine($"Ship: {ship.Name}, Size: {ship.Width}x1");
        //
        //         Coordinates coordinates = AskCoordinates();
        //         var orientation = EOrientation.Horizontal;
        //         if (ship.Width != 1)
        //         {
        //             orientation = AskOrientation();
        //         }
        //
        //         while (!_validator.CoordinatesAreValid(coordinates, orientation, PlayerBoard, ship.Width))
        //         {
        //             Console.ForegroundColor = ConsoleColor.Red;
        //             Console.WriteLine("Out of bounds!");
        //             Console.ForegroundColor = ConsoleColor.Cyan;
        //             coordinates = AskCoordinates();
        //             orientation = AskOrientation();
        //         }
        //
        //         while (!_validator.AreaFree(coordinates, orientation, PlayerBoard, ship.Width))
        //         {
        //             Console.ForegroundColor = ConsoleColor.Red;
        //             Console.WriteLine("Ship already placed there!");
        //             Console.ForegroundColor = ConsoleColor.Cyan;
        //             coordinates = AskCoordinates();
        //             orientation = AskOrientation();
        //         }
        //
        //         // PlaceShip(coordinates, ship, orientation);
        //     }
        //
        //     DrawGameUI();
        //     Console.Write("Continue...");
        //     Console.ReadLine();
        // }

        public void PlaceShip(int x, int y, Ship ship, EOrientation orientation)
        {
            var shipSize = ship.Width;
            var board = PlayerBoard.Board;
            var state = ship.CellState;

            if (orientation == EOrientation.Horizontal)
            {
                for (var i = x; i < x + shipSize; i++)
                {
                    board[i - 1, y - 1] = state;
                }
            }

            if (orientation != EOrientation.Vertical) return;
            {
                for (var i = y; i < y + shipSize; i++)
                {
                    board[x - 1, i - 1] = state;
                }
            }
        }

        private (int x, int y) RandomCoordinates(int width, int height)
        {
            Random rand = new(Guid.NewGuid().GetHashCode());
            var column = rand.Next(1, width + 1);
            var row = rand.Next(1, height + 2);

            return (rand.Next(1, column), rand.Next(1, row));
        }

        public string GetSerializedGameBoardState()
        {
            var state = new GameBoardState();

            var width = PlayerBoard.Board.GetUpperBound(0) + 1;
            var height = PlayerBoard.Board.GetUpperBound(1) + 1;

            state.Board = new ECellState[width][];

            for (var i = 0; i < state.Board.Length; i++)
            {
                state.Board[i] = new ECellState[height];
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    state.Board[x][y] = PlayerBoard.Board[x, y];
                }
            }


            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(state, jsonOptions);
        }

        public string GetSerializedFiringBoardState()
        {
            var state = new GameBoardState();

            var width = FiringBoard.Board.GetUpperBound(0) + 1;
            var height = FiringBoard.Board.GetUpperBound(1) + 1;

            state.Board = new ECellState[width][];

            for (var i = 0; i < state.Board.Length; i++)
            {
                state.Board[i] = new ECellState[height];
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    state.Board[x][y] = FiringBoard.Board[x, y];
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(state, jsonOptions);
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

        private void DrawGameUI()
        {
            Console.Clear();
            Console.WriteLine($"- - - {Name.ToUpper()} BOARD - - -");
            // GameBoardUI.DrawBoardWithShips(PlayerBoard);
            Console.WriteLine();
            Console.WriteLine("- - - OPPONENT BOARD - - -");
            GameBoardUI.DrawBoardWithoutShips(PlayerBoard);
            Console.WriteLine();
        }
    }
}