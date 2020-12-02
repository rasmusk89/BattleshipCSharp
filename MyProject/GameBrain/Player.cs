using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Domain.Enums;

namespace GameBrain
{
    public class Player
    {
        public string Name { get; set; } = "Player";

        public EPlayerType PlayerType { get; set; }

        public List<Ship> Ships { get; set; } = new();

        public GameBoard GameBoard { get; set; } = null!;

        public Validator Validator { get; set; } = new Validator();

        public EShipsCanTouch ShipsCanTouch { get; set; }

        public bool HasLost
        {
            get { return Ships.All(x => x.IsSunk); }
        }

        public void SetBoard(int width, int height)
        {
            GameBoard = new GameBoard(width, height);
        }

        public ECellState[,] GetPlayerBoard()
        {
            return GameBoard.Board;
        }
        
        public ECellState GetCell(int x, int y)
        {
            return GameBoard.Board[x ,y];
        }


        public IEnumerable<Ship> GetShips()
        {
            return Ships;
        }

        public void SetShips(IEnumerable<Ship> ships)
        {
            foreach (var ship in ships)
            {
                Ships.Add(new Ship
                {
                    CellState = ship.CellState,
                    Hits = ship.Hits,
                    Name = ship.Name,
                    Width = ship.Width
                });
            }
        }

        public string GetName()
        {
            return Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public EPlayerType GetPlayerType()
        {
            return PlayerType;
        }

        private static bool IsHit(int column, int row, Player opponent)
        {
            return opponent.GetPlayerBoard()[column, row] != ECellState.Empty;
        }

        private static void RegisterHit(int column, int row, Player opponent)
        {
            var shipState = opponent.GetPlayerBoard()[column, row];
            var ship = new Ship();

            foreach (var opponentShip in opponent.Ships.Where(opponentShip =>
                opponentShip.CellState == shipState && !opponentShip.IsSunk))
            {
                ship = opponentShip;
            }

            ship.Hits++;
        }

        public void PlaceBomb(int column, int row, Player opponent)
        {
            if (IsHit(column, row, opponent))
            {
                RegisterHit(column, row, opponent);
                opponent.GetPlayerBoard()[column, row] = ECellState.Hit;
            }
            else
            {
                opponent.GetPlayerBoard()[column, row] = ECellState.Bomb;
            }
        }

        public void PlaceShip(int column, int row, Ship ship, EOrientation orientation)
        {
            var shipSize = ship.Width;
            var board = GameBoard.Board;
            var state = ship.CellState;
            switch (orientation)
            {
                case EOrientation.Horizontal:
                {
                    for (var i = 0; i < shipSize; i++)
                    {
                        board[column + i, row] = state;
                    }

                    break;
                }
                case EOrientation.Vertical:
                {
                    for (var i = 0; i < shipSize; i++)
                    {
                        board[column, row + i] = state;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        public bool ShipAreaFree(int column, int row, GameBoard board, Ship ship, EOrientation orientation,
            EShipsCanTouch shipsCanTouch)
        {
            var boardWidth = board.Board.GetUpperBound(0);
            var boardHeight = board.Board.GetUpperBound(1);
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
                    return board.Board[column, row] == ECellState.Empty;
                }

                for (var i = startColumn; i <= endColumn; i++)
                {
                    for (var j = startRow; j <= endRow; j++)
                    {
                        if (board.Board[i, j] == ECellState.Empty)
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
                        if (board.Board[i, j] == ECellState.Empty)
                        {
                            continue;
                        }

                        occupiedCells++;
                    }
                }
            }
            return occupiedCells == 0;
        }

        public void PlaceRandomShips()
        {
            var random = new Random();
            for (var i = Ships.Count - 1; i >= 0; i--)
            {
                var ship = Ships[i];
                var width = GameBoard.Board.GetUpperBound(0) + 1;
                var height = GameBoard.Board.GetUpperBound(1) + 1;
                var x = random.Next(1, width);
                var y = random.Next(1, height);
                var orientation = EOrientation.Vertical;
                var orientationIndex = random.Next(1, 101) % 2;

                orientation = orientationIndex switch
                {
                    0 => EOrientation.Horizontal,
                    1 => EOrientation.Vertical,
                    _ => orientation
                };
                while (!Validator.ShipCoordinatesAreValid(x, y, width, height, ship, orientation)
                       || !ShipAreaFree(x, y, GameBoard, ship, orientation, ShipsCanTouch))
                {
                    x = random.Next(1, width);
                    y = random.Next(1, height);
                    orientationIndex = random.Next(1, 101) % 2;
                }
                PlaceShip(x, y, ship, orientation);
            }
        }

        public string GetSerializedGameBoardState()
        {
            var state = new GameBoardState();

            var width = GameBoard.Board.GetUpperBound(0) + 1;
            var height = GameBoard.Board.GetUpperBound(1) + 1;

            state.Board = new ECellState[width][];

            for (var i = 0; i < state.Board.Length; i++)
            {
                state.Board[i] = new ECellState[height];
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    state.Board[x][y] = GameBoard.Board[x, y];
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(state, jsonOptions);
        }

        public void SetSerializedBoardState(string board)
        {
            var width = GameBoard.Board.GetUpperBound(0) + 1;
            var height = GameBoard.Board.GetUpperBound(1) + 1;
            var gameBoardState = JsonSerializer.Deserialize<GameBoardState>(board);
            GameBoard = new GameBoard(width, height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    GameBoard.Board[x, y] = gameBoardState!.Board[x][y];
                }
            }
        }
    }
}