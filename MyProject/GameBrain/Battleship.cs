using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.Json;

namespace GameBrain
{
    public class Battleship
    {
        private static readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static int _height { get; set; } = 10;
        private static int _width { get; set; } = 10;

        private static ECellState[,]? _boardA = new ECellState[_height, _width];
        private static ECellState[,]? _boardB = new ECellState[_height, _width];

        private static bool _nextMoveByPlayerA = true;

        public Battleship(int width, int height)
        {
            _height = height;
            _width = width;
            _boardA = new ECellState[height, width];
            _boardB = new ECellState[height, width];
        }

        public static ECellState[,] GetBoardA()
        {
            var board = new ECellState[_height, _width];
            if (_boardA != null) Array.Copy(_boardA, board, _boardA.Length);

            return board;
        }

        public static ECellState[,] GetBoardB()
        {
            var board = new ECellState[_height, _width];
            if (_boardB != null) Array.Copy(_boardB, board, _boardB.Length);

            return board;
        }

        public void MakeAMove(int x, int y)
        {
            if (_nextMoveByPlayerA)
            {
                if (_boardB != null && _boardB[y, x] != ECellState.Empty)
                {
                    Console.WriteLine("Bomb has already placed there. Choose another coordinate!");
                    return;
                }

                if (_boardB != null) _boardB[y, x] = ECellState.Bomb;
                _nextMoveByPlayerA = !_nextMoveByPlayerA;
            }
            else
            {
                if (_boardA != null && _boardA[y, x] != ECellState.Empty)
                {
                    Console.WriteLine("Bomb has already placed there. Choose another coordinate!");
                    return;
                }

                if (_boardA != null) _boardA[y, x] = ECellState.Bomb;
                _nextMoveByPlayerA = true;
            }
        }

        public void RemoveAMove(int x, int y)
        {
            if (!_nextMoveByPlayerA)
            {
                _boardB![x, y] = ECellState.Empty;
                _nextMoveByPlayerA = true;
            }
            else
            {
                _boardA![x, y] = ECellState.Empty;
                _nextMoveByPlayerA = !_nextMoveByPlayerA;
            }
        }

        public bool CellHasBomb(string input, bool nextMoveByA)
        {
            if (!CoordinatesAreValid(input)) return false;
            var (x, y) = GetMoveCoordinates(input);
            if (nextMoveByA)
            {
                return _boardB![y, x] == ECellState.Bomb;
            }

            return _boardA![y, x] == ECellState.Bomb;
        }

        private static bool IsNumeric(string x)
        {
            return int.TryParse(x, out _);
        }

        public bool CoordinatesAreValid(string input)
        {
            var userInput = input.Split(",");
            if (userInput[0].Length < 1 || userInput[1].Length < 1)
            {
                Console.WriteLine("null?");
                return false;
            }
            
            if (input.Length < 3)
            {
                Console.WriteLine("0");
                return false;
            }

            if (userInput.Length != 2)
            {
                Console.WriteLine("1");
                return false;
            }

            if (userInput[0].Length > 1 || userInput[0].Length < 0)
            {
                Console.WriteLine("2");

                return false;
            }

            if (!Letters.Contains(char.Parse(userInput[0].ToUpper())))
            {
                Console.WriteLine("3");

                return false;
            }

            if (char.Parse(userInput[0].ToUpper()) - 64 > _height)
            {
                Console.WriteLine("4");

                return false;
            }

            if (!IsNumeric(userInput[1]))
            {
                Console.WriteLine("5");

                return false;
            }

            if (int.Parse(userInput[1]) > _height || int.Parse(userInput[1]) < 1)
            {
                Console.WriteLine("6");
                return false;
            }

            return true;
        }

        public (int x, int y) GetMoveCoordinates(string input)
        {
            var userInput = input.Split(",");

            var y = char.Parse(userInput[0].ToUpper()) - 65;
            var x = int.Parse(userInput[1]) - 1;
            return (x, y);
        }

        private static string GetSerializedGameState()
        {
            var state = new GameState
            {
                NextMoveByPlayerA = _nextMoveByPlayerA,
                Width = _width,
                Height = _height
            };

            state.BoardA = new ECellState[state.Width][];

            for (var i = 0; i < state.BoardA.Length; i++)
            {
                state.BoardA[i] = new ECellState[state.Width];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.BoardA[x][y] = _boardA![x, y];
                }
            }

            state.BoardB = new ECellState[state.Width][];

            for (var i = 0; i < state.BoardB.Length; i++)
            {
                state.BoardB[i] = new ECellState[state.Width];
            }

            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.BoardB[x][y] = _boardB![x, y];
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(state, jsonOptions);
        }

        private static void SetGameStateFromJsonString(string jsonString)
        {
            var state = JsonSerializer.Deserialize<GameState>(jsonString);

            // restore actual state from deserialized state
            _nextMoveByPlayerA = state.NextMoveByPlayerA;
            _boardA = new ECellState[state.Height, state.Width];
            _boardB = new ECellState[state.Height, state.Width];

            for (var x = 0; x < state.Height; x++)
            {
                for (var y = 0; y < state.Width; y++)
                {
                    _boardA[x, y] = state.BoardA[x][y];
                }
            }

            for (var x = 0; x < state.Height; x++)
            {
                for (var y = 0; y < state.Width; y++)
                {
                    _boardB[x, y] = state.BoardB[x][y];
                }
            }
        }

        public static string SaveGameAction(Battleship game)
        {
            // 2020-10-12
            var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.Write($"File name ({defaultName}):");
            var fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = defaultName;
            }


            var serializedGame = GetSerializedGameState();

            System.IO.File.WriteAllText(fileName, serializedGame);

            return "";
        }

        public static string LoadGameAction(Battleship game)
        {
            var files = System.IO.Directory.EnumerateFiles(".", "*.json").ToList();
            for (var i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i} - {files[i]}");
            }

            var fileNo = Console.ReadLine();
            var fileName = files[int.Parse(fileNo!.Trim())];

            var jsonString = System.IO.File.ReadAllText(fileName);

            SetGameStateFromJsonString(jsonString);

            // BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA ? GetBoardA() : GetBoardB());

            return "";
        }

        public bool GetNextMove()
        {
            return _nextMoveByPlayerA;
        }
    }
}