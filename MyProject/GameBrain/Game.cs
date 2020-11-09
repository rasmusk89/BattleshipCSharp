using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GameBrain
{
    public class Game
    {
        private const int BoardWidth = 10;
        private const int BoardHeight = 10;

        private Player PlayerA { get; set; }
        private Player PlayerB { get; set; }

        private static bool _nextMoveByPlayerA = true;

        public Game()
        {
            PlayerA = new Player("Player A", BoardWidth, BoardHeight);
            PlayerB = new Player("Player B", BoardWidth, BoardHeight);
        }

        public void PlayRound()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+---------------------------+\n" +
                              "|      BATTLESHIP BASIC     |\n" +
                              "+---------------------------+");
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

            while (true)
            {
                PlaceBombs();
            }
        }

        public void CustomRound()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+---------------------------+\n" +
                              "|     BATTLESHIP CUSTOM     |\n" +
                              "+---------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write("Enter player one name: ");
            string aName = Console.ReadLine() ?? "";

            Console.Write("Enter player two name: ");
            string bName = Console.ReadLine() ?? "";

            Console.Write("Enter game board width: ");
            string width = Console.ReadLine() ?? "";

            Console.Write("Enter game board height: ");
            string height = Console.ReadLine() ?? "";

            var boardWidth = int.Parse(width);
            var boardHeight = int.Parse(height);

            List<Ship> ships = new List<Ship>();

            Console.WriteLine("Enter number of Patrol (1x1) ships: ");
            string patrol = Console.ReadLine() ?? "";
            var nPatrol = int.Parse(patrol);

            Console.WriteLine("Enter number of Cruiser (2x1) ships: ");
            string cruiser = Console.ReadLine() ?? "";
            var nCruiser = int.Parse(cruiser);

            Console.WriteLine("Enter number of Submarine (3x1) ships: ");
            string submarine = Console.ReadLine() ?? "";
            var nSubmarine = int.Parse(submarine);

            Console.WriteLine("Enter number of Battleship (4x1) ships: ");
            string battleship = Console.ReadLine() ?? "";
            var nBattleship = int.Parse(battleship);

            Console.WriteLine("Enter number of Carrier (5x1) ships: ");
            string carrier = Console.ReadLine() ?? "";
            var nCarrier = int.Parse(carrier);

            for (var i = 0; i < nPatrol; i++)
            {
                ships.Add(new Ship(1));
            }

            for (var i = 0; i < nCruiser; i++)
            {
                ships.Add(new Ship(2));
            }

            for (var i = 0; i < nSubmarine; i++)
            {
                ships.Add(new Ship(3));
            }

            for (var i = 0; i < nBattleship; i++)
            {
                ships.Add(new Ship(4));
            }

            for (var i = 0; i < nCarrier; i++)
            {
                ships.Add(new Ship(5));
            }

            Console.WriteLine("Can ships touch? (Y/N): ");
            string canTouch = Console.ReadLine() ?? "";

            var shipsCanTouch = true;
            if (canTouch.ToLower() == "y")
            {
                shipsCanTouch = true;
            }

            if (canTouch.ToLower() == "n")
            {
                shipsCanTouch = false;
            }


            PlayerA = new Player(aName, boardWidth, boardHeight, shipsCanTouch);
            PlayerB = new Player(bName, boardWidth, boardHeight, shipsCanTouch);

            PlayerA.SetShips(ships);
            PlayerB.SetShips(ships);

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

            while (true)
            {
                PlaceBombs();
            }
        }

        private void PlaceShips()
        {
            Console.Clear();
            Console.Write($"Player {PlayerA.GetName()}, press ENTER to place ships...");
            Console.ReadLine();
            PlayerA.PlaceShips();
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
                Width = BoardWidth,
                Height = BoardHeight
            };

            state.PlayerAPlayerBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerAPlayerBoard.Length; i++)
            {
                state.PlayerAPlayerBoard[i] = new ECellState[state.Width];
            }

            for (var x = 0; x < state.Height; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.PlayerAPlayerBoard[x][y] = PlayerA.PlayerBoard.Board[x, y];
                }
            }
            
            state.PlayerAFiringBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerAFiringBoard.Length; i++)
            {
                state.PlayerAFiringBoard[i] = new ECellState[state.Width];
            }

            for (var x = 0; x < state.Height; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.PlayerAFiringBoard[x][y] = PlayerA.OpponentBoard.Board[x, y];
                }
            }
            
            state.PlayerBPlayerBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerBPlayerBoard.Length; i++)
            {
                state.PlayerBPlayerBoard[i] = new ECellState[state.Width];
            }

            for (var x = 0; x < state.Height; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.PlayerBPlayerBoard[x][y] = PlayerB.PlayerBoard.Board[x, y];
                }
            }
            
            state.PlayerBFiringBoard = new ECellState[state.Width][];

            for (var i = 0; i < state.PlayerBFiringBoard.Length; i++)
            {
                state.PlayerBFiringBoard[i] = new ECellState[state.Width];
            }

            for (var x = 0; x < state.Height; x++)
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

            System.IO.File.WriteAllText(name, serializedGame);
        }


        private void SetGameStateFromJsonString(string jsonString)
        {
            var state = JsonSerializer.Deserialize<GameState>(jsonString);
        
            // restore actual state from deserialized state
            _nextMoveByPlayerA = state.NextMoveByPlayerA;
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
            var jsonString = System.IO.File.ReadAllText("Battleship.json");

            SetGameStateFromJsonString(jsonString);

            
            while (true)
            {
                PlaceBombs();
            }

        }

        
    }
}