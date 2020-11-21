﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DAL;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameBrain
{
    public class Game
    {
        private static int _boardWidth;
        private static int _boardHeight;
        private Player PlayerA { get; set; }
        private Player PlayerB { get; set; }

        private static bool _nextMoveByPlayerA = true;

        private EShipsCanTouch _shipsCanTouch;

        public Game(GameOptions options)
        {
            _boardWidth = options.GetBoardWidth();
            _boardHeight = options.GetBoardHeight();
            PlayerA = options.PlayerA;
            PlayerB = options.PlayerB;
            _shipsCanTouch = options.ShipsCanTouch;
        }

        public void PlayRound()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+----------------------------+\n" +
                              "| < - - - BATTLESHIP - - - > |\n" +
                              "+----------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Press ENTER to place ships or type \"R\" for random ships: ");
            string input = Console.ReadLine() ?? "";
            if (input.ToLower() != "r")
            {
                PlaceShips();
            }
            else
            {
                PlaceRandomShips();
            }

            // Save options to database.
            SaveToDataBase();

            SaveGameAction();
            while (true)
            {
                if (_nextMoveByPlayerA)
                {
                    PlayerA.PlaceBomb(PlayerB);
                    if (PlayerB.HasLost)
                    {
                        Console.WriteLine("PLayer A WON!");
                        Console.ReadLine();
                        return;
                    }

                    _nextMoveByPlayerA = !_nextMoveByPlayerA;
                    SaveGameAction();
                }

                PlayerB.PlaceBomb(PlayerA);
                if (PlayerA.HasLost)
                {
                    _nextMoveByPlayerA = true;
                    Console.WriteLine("PLayer B WON!");
                    Console.ReadLine();
                    return;
                }

                _nextMoveByPlayerA = true;
                SaveGameAction();
            }
        }

        private void PlaceShips()
        {
            Console.Clear();
            Console.Write($"Player {PlayerA.GetName()}, press ENTER to place ships...");
            Console.ReadLine();
            PlayerA.PlaceShips();
            SaveGameAction();
            Console.Clear();
            Console.Write($"Player {PlayerB.GetName()}, press ENTER to place ships...");
            Console.ReadLine();
            PlayerB.PlaceShips();
            SaveGameAction();
            Console.Clear();
            Console.Write("Continue...");
        }

        private void PlaceRandomShips()
        {
            PlayerA.PlaceRandomShips();
            PlayerB.PlaceRandomShips();
            SaveGameAction();
        }

        private void PlaceBombs()
        {
            if (_nextMoveByPlayerA)
            {
                PlayerA.PlaceBomb(PlayerB);
                _nextMoveByPlayerA = !_nextMoveByPlayerA;
                SaveGameAction();
            }

            PlayerB.PlaceBomb(PlayerA);

            _nextMoveByPlayerA = true;
            SaveGameAction();
        }

        private string GetSerializedGameState()
        {
            var state = new GameState
            {
                NextMoveByPlayerA = _nextMoveByPlayerA,
                Width = _boardWidth,
                Height = _boardHeight,
                PlayerAName = PlayerA.GetName(),
                PlayerBName = PlayerB.GetName(),
                PlayerAShips = PlayerA.GetShips(),
                PlayerBShips = PlayerB.GetShips()
            };

            state.PlayerAPlayerBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerAPlayerBoard.Length; i++)
            {
                state.PlayerAPlayerBoard[i] = new ECellState[state.Height];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.PlayerAPlayerBoard[x][y] = PlayerA.PlayerBoard.Board[x, y];
                }
            }

            state.PlayerAFiringBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerAFiringBoard.Length; i++)
            {
                state.PlayerAFiringBoard[i] = new ECellState[state.Height];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.PlayerAFiringBoard[x][y] = PlayerA.OpponentBoard.Board[x, y];
                }
            }

            state.PlayerBPlayerBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerBPlayerBoard.Length; i++)
            {
                state.PlayerBPlayerBoard[i] = new ECellState[state.Height];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.PlayerBPlayerBoard[x][y] = PlayerB.PlayerBoard.Board[x, y];
                }
            }

            state.PlayerBFiringBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerBFiringBoard.Length; i++)
            {
                state.PlayerBFiringBoard[i] = new ECellState[state.Height];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.PlayerBFiringBoard[x][y] = PlayerB.OpponentBoard.Board[x, y];
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(state, jsonOptions);
        }

        private void SaveGameAction()
        {
            // // var defaultName = "Battleship_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            // var defaultName = "Battleship.json";
            // Console.Write($"File name ({defaultName}):");
            // var fileName = Console.ReadLine();
            // if (string.IsNullOrWhiteSpace(fileName))
            // {
            //     fileName = defaultName;
            // }

            const string? name = "Battleship.json";

            var serializedGame = GetSerializedGameState();

            File.WriteAllText(name, serializedGame);
        }


        private void SetGameStateFromJsonString(string jsonString)
        {
            var state = JsonSerializer.Deserialize<GameState>(jsonString);

            // restore actual state from deserialized state
            _nextMoveByPlayerA = state!.NextMoveByPlayerA;
            _boardWidth = state.Width;
            _boardHeight = state.Height;
            PlayerA.Name = state.PlayerAName!;
            PlayerB.Name = state.PlayerBName!;
            PlayerA.PlayerBoard.Board = new ECellState[state.Width, state.Height];
            PlayerA.OpponentBoard.Board = new ECellState[state.Width, state.Height];
            PlayerB.PlayerBoard.Board = new ECellState[state.Width, state.Height];
            PlayerB.OpponentBoard.Board = new ECellState[state.Width, state.Height];

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    PlayerA.PlayerBoard.Board[x, y] = state.PlayerAPlayerBoard[x][y];
                }
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    PlayerA.OpponentBoard.Board[x, y] = state.PlayerAFiringBoard[x][y];
                }
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    PlayerB.PlayerBoard.Board[x, y] = state.PlayerBPlayerBoard[x][y];
                }
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    PlayerB.OpponentBoard.Board[x, y] = state.PlayerBFiringBoard[x][y];
                }
            }
        }

        public void LoadGameAction()
        {
            // var files = System.IO.Directory.EnumerateFiles(".", "*").ToList();
            // for (var i = 0; i < files.Count; i++)
            // {
            //     Console.WriteLine($"{i} - {files[i]}");
            // }
            //
            // var fileNo = Console.ReadLine();
            // var fileName = files[int.Parse(fileNo!.Trim())];

            // var jsonString = System.IO.File.ReadAllText(fileName);
            var jsonString = File.ReadAllText("Battleship.json");

            SetGameStateFromJsonString(jsonString);

            while (true)
            {
                PlaceBombs();
                if (!PlayerA.HasLost || !PlayerB.HasLost) continue;
                Console.WriteLine("GAME OVER");
                Console.ReadLine();
                return;
            }
        }

        public void SaveToDataBase()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(
                @"
                Server=barrel.itcollege.ee,1533;
                User Id=student;
                Password=Student.Bad.password.0;
                Database=raskil_db;
                MultipleActiveResultSets=true;
                ").Options;
            using var dbCtx = new AppDbContext(dbOptions);

            Console.WriteLine("Deleting database..");
            dbCtx.Database.EnsureDeleted();
            Console.WriteLine("Migrating database..");
            dbCtx.Database.Migrate();
            Console.WriteLine("Adding data to database..");

            var game = new Domain.Game
            {
                Description = PlayerA.Name + "_vs_" + PlayerB.Name + "@" + DateTime.Now.TimeOfDay
            };

            game.PlayerA = new Domain.Player
            {
                Name = PlayerA.Name,
                EPlayerType = PlayerA.PlayerType,
                PlayerBoardStates = new List<PlayerBoardState>()
            };

            game.PlayerB = new Domain.Player
            {
                Name = PlayerB.Name,
                EPlayerType = PlayerB.PlayerType,
                PlayerBoardStates = new List<PlayerBoardState>()
            };

            var gameOptions = new GameOption
            {
                Name = PlayerA.Name + "_vs_" + PlayerB.Name + "@" + DateTime.Now.TimeOfDay,
                BoardWidth = _boardWidth,
                BoardHeight = _boardHeight,
                EShipsCanTouch = Domain.Enums.EShipsCanTouch.Yes,
                NextMoveAfterHit = ENextMoveAfterHit.OtherPlayer
            };

            game.GameOption = gameOptions;

            var playerABoardState = new PlayerBoardState
            {
                Player = game.PlayerA,
                BoardStatePlayer = PlayerA.GetSerializedBoardState()
            };

            var playerBBoardState = new PlayerBoardState
            {
                Player = game.PlayerB,
                BoardStatePlayer = PlayerB.GetSerializedBoardState()
            };

            game.PlayerA.PlayerBoardStates.Add(playerABoardState);
            game.PlayerB.PlayerBoardStates.Add(playerBBoardState);

            dbCtx.Games.Add(game);
            dbCtx.SaveChanges();
        }
    }
}