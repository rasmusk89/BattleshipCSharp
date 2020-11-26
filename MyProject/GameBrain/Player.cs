using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Domain.Enums;

namespace GameBrain
{
    public class Player
    {
        public string Name { get; set; }

        public EPlayerType PlayerType { get; set; }

        public List<Ship> Ships { get; set; } = new();

        public GameBoard GameBoard { get; set; } = null!;

        public bool HasLost
        {
            get { return Ships.All(x => x.IsSunk); }
        }

        public Player(string name)
        {
            Name = name;
        }

        public void SetBoard(int width, int height)
        {
            GameBoard = new GameBoard(width, height);
        }

        public ECellState[,] GetPlayerBoard()
        {
            return GameBoard.Board;
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

        public bool IsHit(int column, int row, Player opponent)
        {
            return opponent.GetPlayerBoard()[column, row] != ECellState.Empty;
        }

        private static void RegisterHit(int column, int row, Player opponent)
        {
            var shipState = opponent.GetPlayerBoard()[column, row];
            var ship = new Ship();

            foreach (var opponentShip in opponent.Ships.Where(opponentShip => opponentShip.CellState == shipState && !opponentShip.IsSunk))
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
        
    }
}