using System;
using System.Collections.Generic;
using System.Text.Json;
using Domain.Enums;

namespace GameBrain
{
    public class Game
    {
        private readonly int _boardHeight;
        private readonly int _boardWidth;
        public Player PlayerA { get; set; }
        public Player PlayerB { get; set; }
        private List<Ship> Ships { get; set; }
        private readonly EShipsCanTouch _shipsCanTouch;
        private bool _nextMoveByPlayerA = true;
        private readonly GameOptions _gameOptions;
        private readonly Validator _validator = new();

        public Game(GameOptions options)
        {
            _boardHeight = options.GetBoardHeight();
            _boardWidth = options.GetBoardWidth();
            Ships = options.GetShips();
            PlayerA = new Player()
            {
                Name = "Player 1"
            };
            PlayerA.SetShips(Ships);
            PlayerB = new Player()
            {
                Name = "Player 2"
            };
            PlayerB.SetShips(Ships);
            PlayerA.SetBoard(_boardWidth, _boardHeight);
            PlayerB.SetBoard(_boardWidth, _boardHeight);
            _shipsCanTouch = options.GetShipsCanTouch();
            _gameOptions = options;
        }

        public void StartGame()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+----------------------------+\n" +
                              "| < - - - BATTLESHIP - - - > |\n" +
                              "+----------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;

            var (playerAName, playerBName) = GetPlayerNames();
            PlayerA.SetName(playerAName);
            PlayerB.SetName(playerBName);
            Console.Write("Press ENTER to place ships or type \"R\" for random ships: ");
            string input = Console.ReadLine() ?? "";
            if (input.ToLower() != "r")
            {
                Console.Clear();
                PlaceShips(PlayerA);
                Console.Clear();
                PlaceShips(PlayerB);
            }
            else
            {
                Console.Clear();
                PlaceRandomShips(PlayerA);
                PlaceRandomShips(PlayerB);
            }
            GameSaving.InitialSave(GetGameState());
            PlayRound();
        }

        public void PlayRound()
        {
            var gameOver = false;
            while (!gameOver)
            {
                gameOver = PlaceBombs(PlayerA, PlayerB);
            }

            Console.WriteLine("GAME OVER!");
            Console.ReadLine();
        }

        public void Test()
        {
            GameBoardUI.DrawPlayerBoard(PlayerA);
            GameBoardUI.DrawPlayerBoard(PlayerB);
            Console.ReadLine();
        }

        private static (string playerAName, string playerBName) GetPlayerNames()
        {
            string playerAName;
            Console.Write("Player ONE, please enter your name: ");
            var inputA = Console.ReadLine();
            if (inputA == "")
            {
                playerAName = "PLayer One";
            }
            playerAName = inputA ?? "PLayer One";

            string playerBName;
            Console.Write("Player TWO, please enter your name: ");
            var inputB = Console.ReadLine();
            if (inputB == "")
            {
                playerBName = "PLayer Two";
            }
            playerBName = inputB ?? "PLayer Two";

            return (playerAName, playerBName);
        }

        private bool PlaceBombs(Player playerA, Player playerB)
        {
            Console.Clear();
            Console.WriteLine();
            if (_nextMoveByPlayerA)
            {
                Console.Write($"{playerA.Name}, press ENTER to place bombs!");
                Console.ReadLine();
                Console.Clear();
                GameBoardUI.DrawBoards(playerA, playerB);
                Console.WriteLine();
                Console.WriteLine($"{playerA.Name}, place bomb!");
                var (column, row) = AskCoordinates();
                while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, playerB))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ship already placed there!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    (column, row) = AskCoordinates();
                }

                var isHit = playerB.GetPlayerBoard()[column, row] != ECellState.Empty;
                playerA.PlaceBomb(column, row, playerB);
                Console.Clear();
                GameBoardUI.DrawBoards(playerA, playerB);
                Console.WriteLine();
                Console.Write(isHit ? "HIT! Press enter to continue.." : "MISS! Press enter to continue..");
                Console.ReadLine();
                _nextMoveByPlayerA = !_nextMoveByPlayerA;
                Console.Clear();
            }
            else
            {
                Console.Write($"{playerB.Name}, press ENTER to place bombs!");
                Console.ReadLine();
                Console.Clear();
                GameBoardUI.DrawBoards(playerB, playerA);
                Console.WriteLine();
                Console.WriteLine($"{playerB.Name}, place bomb!");
                var (column, row) = AskCoordinates();
                while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, playerA))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ship already placed there!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    (column, row) = AskCoordinates();
                }

                var isHit = playerA.GetPlayerBoard()[column, row] != ECellState.Empty;
                playerB.PlaceBomb(column, row, playerA);
                Console.Clear();
                GameBoardUI.DrawBoards(playerB, playerA);
                Console.WriteLine();
                Console.Write(isHit ? "HIT! Press enter to continue.." : "MISS! Press enter to continue..");
                Console.ReadLine();
                _nextMoveByPlayerA = true;
                Console.Clear();
            }
            return playerA.HasLost || playerB.HasLost;
        }

        private void PlaceShips(Player player)
        {
            Console.Clear();
            Console.WriteLine();
            foreach (var ship in player.GetShips())
            {
                GameBoardUI.DrawPlayerBoard(player);
                Console.WriteLine();
                Console.WriteLine($"Player {player.GetName()} place ships.");
                Console.WriteLine($"Ship: {ship.Name}, Size: {ship.Width}x1");
                var orientation = EOrientation.Horizontal;
                var (column, row) = AskCoordinates();
                if (ship.Width > 1)
                {
                    orientation = AskOrientation();
                }

                while (!_validator.ShipCoordinatesAreValid(column, row, _boardWidth, _boardHeight, ship, orientation))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Out of bounds!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    (column, row) = AskCoordinates();
                    if (ship.Width > 1)
                    {
                        orientation = AskOrientation();
                    }
                }

                while (!_validator.ShipAreaFree(column, row, player, ship, orientation, _shipsCanTouch))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ship already on path!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    (column, row) = AskCoordinates();
                    if (ship.Width > 1)
                    {
                        orientation = AskOrientation();
                    }
                }

                player.PlaceShip(column, row, ship, orientation);

                Console.Clear();
                GameBoardUI.DrawPlayerBoard(player);
            }

            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
        }

        private EOrientation AskOrientation()
        {
            Console.Write("Insert orientation Horizontal(H) or Vertical(V): ");
            string input = Console.ReadLine() ?? "";

            while (!_validator.OrientationIsValid(input))
            {
                Console.Write("Please enter correct orientation Horizontal(H) or Vertical(V): ");
                input = Console.ReadLine() ?? "";
            }

            var orientation = input.Trim().ToLower() switch
            {
                "h" => EOrientation.Horizontal,
                "v" => EOrientation.Vertical,
                _ => EOrientation.Horizontal
            };
            return orientation;
        }

        private static (int x, int y) RandomCoordinates(int width, int height)
        {
            Random rand = new(Guid.NewGuid().GetHashCode());
            var column = rand.Next(0, width);
            var row = rand.Next(0, height);
            return (column, row);
        }

        private (int x, int y) AskCoordinates()
        {
            Console.Write(
                $"Insert Column (A-{IntToAlphabeticValue(_boardWidth - 1)}) or press ENTER for random coordinates: ");
            string columnInput = Console.ReadLine() ?? "";
            if (columnInput == "")
            {
                return RandomCoordinates(_boardWidth, _boardHeight);
            }

            while (!_validator.ColumnInputIsValid(columnInput, _boardWidth))
            {
                Console.Write($"Please insert correct Column (A-{IntToAlphabeticValue(_boardWidth - 1)}): ");
                columnInput = Console.ReadLine() ?? "";
            }

            var column = Validator.ConvertStringToInteger(columnInput);

            Console.Write($"Insert Row (1-{_boardHeight}): ");
            string rowInput = Console.ReadLine() ?? "";
            while (!_validator.RowInputIsValid(rowInput, _boardHeight))
            {
                Console.Write($"Please enter correct row (1-{_boardHeight}): ");
                rowInput = Console.ReadLine() ?? "";
            }

            var row = int.Parse(rowInput);
            return (column - 1, row - 1);
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

        private void PlaceRandomShips(Player player)
        {
            var random = new Random();
            for (var i = player.Ships.Count - 1; i >= 0; i--)
            {
                var ship = player.GetShips()[i];
                var x = random.Next(1, _boardWidth);
                var y = random.Next(1, _boardHeight);
                var orientation = EOrientation.Vertical;
                var orientationIndex = random.Next(1, 101) % 2;

                orientation = orientationIndex switch
                {
                    0 => EOrientation.Horizontal,
                    1 => EOrientation.Vertical,
                    _ => orientation
                };

                while (!_validator.ShipCoordinatesAreValid(x, y, _boardWidth, _boardHeight, ship, orientation)
                       || !_validator.ShipAreaFree(x, y, player, ship, orientation, _shipsCanTouch))
                {
                    x = random.Next(1, _boardWidth);
                    y = random.Next(1, _boardHeight);
                    orientationIndex = random.Next(1, 101) % 2;
                }

                player.PlaceShip(x, y, ship, orientation);
            }
        }

        private GameState GetGameState()
        {
            var state =  new GameState()
            {
                BoardHeightState = _boardHeight,
                BoardWidthState = _boardWidth,
                NextMoveByPlayerAState = _nextMoveByPlayerA,
                PlayerAState = PlayerA,
                PlayerBState = PlayerB,
                ShipsState = Ships,
                ShipsCanTouchState = _shipsCanTouch,
                GameOptions = _gameOptions,
            };

            state.PlayerABoardState = new ECellState[state.BoardWidthState][];

            for (var i = 0; i < state.BoardWidthState; i++)
            {
                state.PlayerABoardState[i] = new ECellState[state.BoardHeightState];
            }
            for (var x = 0; x < state.BoardWidthState; x++)
            {
                for (var y = 0; y < state.BoardHeightState; y++)
                {
                    state.PlayerABoardState[x][y] = PlayerA.GetPlayerBoard()[x, y];
                }
            }
            
            state.PlayerBBoardState = new ECellState[state.BoardWidthState][];

            for (var i = 0; i < state.BoardWidthState; i++)
            {
                state.PlayerBBoardState[i] = new ECellState[state.BoardHeightState];
            }
            for (var x = 0; x < state.BoardWidthState; x++)
            {
                for (var y = 0; y < state.BoardHeightState; y++)
                {
                    state.PlayerBBoardState[x][y] = PlayerB.GetPlayerBoard()[x, y];
                }
            }
            return state;
        }
        

        /*
         private string GetSerializedGameState()
        {
            // var state = new GameState
            // {
            //     NextMoveByPlayerA = _nextMoveByPlayerA,
            //     Width = _boardWidth,
            //     Height = _boardHeight,
            //     PlayerAName = PlayerA.GetName(),
            //     PlayerBName = PlayerB.GetName(),
            //     PlayerAShips = PlayerA.GetShips(),
            //     PlayerBShips = PlayerB.GetShips()
            // };
            //
            // state.PlayerAPlayerBoard = new ECellState[state.Width][];
            //
            // for (var i = 0; i < state.PlayerAPlayerBoard.Length; i++)
            // {
            //     state.PlayerAPlayerBoard[i] = new ECellState[state.Height];
            // }
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         state.PlayerAPlayerBoard[x][y] = PlayerA.PlayerBoard.Board[x, y];
            //     }
            // }
            //
            // state.PlayerAFiringBoard = new ECellState[state.Width][];
            //
            // for (var i = 0; i < state.PlayerAFiringBoard.Length; i++)
            // {
            //     state.PlayerAFiringBoard[i] = new ECellState[state.Height];
            // }
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         state.PlayerAFiringBoard[x][y] = PlayerA.OpponentBoard.Board[x, y];
            //     }
            // }
            //
            // state.PlayerBPlayerBoard = new ECellState[state.Width][];
            //
            // for (var i = 0; i < state.PlayerBPlayerBoard.Length; i++)
            // {
            //     state.PlayerBPlayerBoard[i] = new ECellState[state.Height];
            // }
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         state.PlayerBPlayerBoard[x][y] = PlayerB.PlayerBoard.Board[x, y];
            //     }
            // }
            //
            // state.PlayerBFiringBoard = new ECellState[state.Width][];
            //
            // for (var i = 0; i < state.PlayerBFiringBoard.Length; i++)
            // {
            //     state.PlayerBFiringBoard[i] = new ECellState[state.Height];
            // }
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         state.PlayerBFiringBoard[x][y] = PlayerB.OpponentBoard.Board[x, y];
            //     }
            // }
            //
            // var jsonOptions = new JsonSerializerOptions
            // {
            //     WriteIndented = true
            // };
            // return JsonSerializer.Serialize(state, jsonOptions);
            return "";
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

            // const string? name = "Battleship.json";
            //
            // var serializedGame = GetSerializedGameState();
            //
            // File.WriteAllText(name, serializedGame);
        }


        private void SetGameStateFromJsonString(string jsonString)
        {
            // var state = JsonSerializer.Deserialize<GameState>(jsonString);
            //
            // // restore actual state from deserialized state
            // _nextMoveByPlayerA = state!.NextMoveByPlayerA;
            // _boardWidth = state.Width;
            // _boardHeight = state.Height;
            // PlayerA.Name = state.PlayerAName!;
            // PlayerB.Name = state.PlayerBName!;
            // PlayerA.PlayerBoard.Board = new ECellState[state.Width, state.Height];
            // PlayerA.OpponentBoard.Board = new ECellState[state.Width, state.Height];
            // PlayerB.PlayerBoard.Board = new ECellState[state.Width, state.Height];
            // PlayerB.OpponentBoard.Board = new ECellState[state.Width, state.Height];
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         PlayerA.PlayerBoard.Board[x, y] = state.PlayerAPlayerBoard[x][y];
            //     }
            // }
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         PlayerA.OpponentBoard.Board[x, y] = state.PlayerAFiringBoard[x][y];
            //     }
            // }
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         PlayerB.PlayerBoard.Board[x, y] = state.PlayerBPlayerBoard[x][y];
            //     }
            // }
            //
            // for (var x = 0; x < state.Width; x++)
            // {
            //     for (var y = 0; y < state.Height; y++)
            //     {
            //         PlayerB.OpponentBoard.Board[x, y] = state.PlayerBFiringBoard[x][y];
            //     }
            // }
        }

        public void LoadGameAction()
        {
            // // var files = System.IO.Directory.EnumerateFiles(".", "*").ToList();
            // // for (var i = 0; i < files.Count; i++)
            // // {
            // //     Console.WriteLine($"{i} - {files[i]}");
            // // }
            // //
            // // var fileNo = Console.ReadLine();
            // // var fileName = files[int.Parse(fileNo!.Trim())];
            //
            // // var jsonString = System.IO.File.ReadAllText(fileName);
            // var jsonString = File.ReadAllText("Battleship.json");
            //
            // SetGameStateFromJsonString(jsonString);
            //
            // while (true)
            // {
            //     PlaceBombs();
            //     if (!PlayerA.HasLost || !PlayerB.HasLost) continue;
            //     Console.WriteLine("GAME OVER");
            //     Console.ReadLine();
            //     return;
            // }
        }

        private void SaveInitialDataToDataBase()
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
            Console.Write("Saving game...");
            dbCtx.Database.Migrate();

            var game = new Domain.Game
            {
                Description = PlayerA.Name + "_vs_" + PlayerB.Name + "@" + DateTime.Now.TimeOfDay,
                PlayerA = new Domain.Player
                {
                    Name = PlayerA.Name,
                    EPlayerType = PlayerA.PlayerType,
                    PlayerBoardStates = new List<PlayerBoardState>(),
                    GameShips = new List<GameShip>()
                },
                PlayerB = new Domain.Player
                {
                    Name = PlayerB.Name,
                    EPlayerType = PlayerB.PlayerType,
                    PlayerBoardStates = new List<PlayerBoardState>(),
                    GameShips = new List<GameShip>()
                }
            };

            var gameOptions = new GameOption
            {
                Name = PlayerA.Name + "_vs_" + PlayerB.Name + "@" + DateTime.Now.TimeOfDay,
                BoardWidth = _boardWidth,
                BoardHeight = _boardHeight,
                // EShipsCanTouch = _shipsCanTouch,
                NextMoveAfterHit = ENextMoveAfterHit.OtherPlayer,
                GameOptionShips = new List<GameOptionShip>(),
                NextMoveByPlayerA = _nextMoveByPlayerA
            };

            game.GameOption = gameOptions;

            var playerABoardState = new PlayerBoardState
            {
                Player = game.PlayerA,
                GameBoardState = PlayerA.GetSerializedGameBoardState(),
                // FiringBoardState = PlayerA.GetSerializedFiringBoardState()
            };

            var playerBBoardState = new PlayerBoardState
            {
                Player = game.PlayerB,
                GameBoardState = PlayerB.GetSerializedGameBoardState(),
                // FiringBoardState = PlayerB.GetSerializedFiringBoardState()
            };

            game.PlayerA.PlayerBoardStates.Add(playerABoardState);
            game.PlayerB.PlayerBoardStates.Add(playerBBoardState);

            // foreach (var gameOptionShip in Ships.Select(ship => new GameOptionShip
            // {
            //     GameOption = game.GameOption,
            //     Ship = new Domain.Ship
            //     {
            //         Name = ship.Name,
            //         Width = ship.Width
            //     },
            //     Amount = 1
            // }))
            // {
            //     dbCtx.GameOptionShips.Add(gameOptionShip);
            // }

            foreach (var gameShip in PlayerA.GetShips().Select(ship => new GameShip
            {
                Hits = ship.Hits,
                Name = ship.Name,
                Width = ship.Width,
                IsSunk = ship.IsSunk,
                Player = game.PlayerA,
                ECellState = ship.CellState
            }))
            {
                dbCtx.GameShips.Add(gameShip);
            }

            foreach (var gameShip in PlayerB.GetShips().Select(ship => new GameShip
            {
                Hits = ship.Hits,
                Name = ship.Name,
                Width = ship.Width,
                IsSunk = ship.IsSunk,
                Player = game.PlayerB,
                ECellState = ship.CellState
            }))
            {
                dbCtx.GameShips.Add(gameShip);
            }

            dbCtx.Games.Add(game);
            dbCtx.SaveChanges();
        }

        private void SaveGameStateToDb()
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

            var game = dbCtx.Games
                .OrderByDescending(id => id.GameId)
                .FirstOrDefault();

            var playerAShips = PlayerA.Ships;
            foreach (var playerAShip in playerAShips)
            {
                dbCtx.GameShips
                    .FirstOrDefault(x => x.PlayerId == game.PlayerAId &&
                                         x.Width == playerAShip.Width).Hits = playerAShip.Hits;
            }

            var playerBShips = PlayerB.Ships;
            foreach (var playerBShip in playerBShips)
            {
                dbCtx.GameShips
                    .FirstOrDefault(x =>
                        x.PlayerId == game.PlayerBId &&
                        x.Width == playerBShip.Width)
                    .Hits = playerBShip.Hits;
            }


            dbCtx.PlayerBoardStates
                .Add(new PlayerBoardState
                {
                    GameBoardState = PlayerA.GetSerializedGameBoardState(),
                    // FiringBoardState = PlayerA.GetSerializedFiringBoardState(),
                    PlayerId = game.PlayerAId
                });

            dbCtx.PlayerBoardStates
                .Add(new PlayerBoardState
                {
                    GameBoardState = PlayerB.GetSerializedGameBoardState(),
                    // FiringBoardState = PlayerB.GetSerializedFiringBoardState(),
                    PlayerId = game.PlayerBId
                });
            dbCtx.SaveChanges();
        }
        
        */
        
    }
}