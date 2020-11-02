using System;
using System.Linq;
using System.Text.Json;

namespace GameBrain
{
    public class Battleship
    {
        private static readonly char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static CellState[,]? _boardA;
        private static CellState[,]? _boardB;

        private static int _height;
        private static int _width;
        
        public bool NextMoveByPlayerA = true;

        public Battleship(int height, int width)
        {
            _height = height;
            _width = width;
        }

        public Battleship()
        {
            _height = 10;
            _width = 10;
        }

        public static CellState[,] GetBoardA()
        {
            _boardA = new CellState[_height, _width];
            
            return _boardA;
        }
        
        public static CellState[,] GetBoardB()
        {
            _boardB = new CellState[_height, _width];
            
            return _boardB;
        }

        public void MakeAMove(int x, int y)
        {
            if (NextMoveByPlayerA)
            {
                if (_boardA != null && _boardA[x, y] != CellState.Empty)
                {
                    Console.WriteLine("Bomb has already placed there. Choose another coordinate!");
                    return;
                }

                if (_boardA != null) _boardA[x, y] = CellState.X;
                NextMoveByPlayerA = !NextMoveByPlayerA;
            }
            else
            {
                if (_boardB != null && _boardB[x, y] != CellState.Empty)
                {
                    Console.WriteLine("Bomb has already placed there. Choose another coordinate!");
                    return;
                }

                if (_boardB != null) _boardB[x, y] = CellState.X;
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
                }
                else if (int.Parse(userInput[1]) < 1 || int.Parse(userInput[1]) > _height)
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

        private string GetSerializedGameState()
        {
            var state = new GameState
            {
                NextMoveByPlayerA = NextMoveByPlayerA,
                Width = _width,
                Height = _height
            };
        
            state.BoardA = new CellState[state.Width][];
        
            for (var i = 0; i < state.BoardA.Length; i++)
            {
                state.BoardA[i] = new CellState[state.Width];
            }
        
            for (var x = 0; x < state.Width; x++)
            {
                for (var y = 0; y < state.Height; y++)
                {
                    state.BoardA[x][y] = _boardA![x, y];
                }
            }
        
            state.BoardB = new CellState[state.Width][];
        
            for (var i = 0; i < state.BoardB.Length; i++)
            {
                state.BoardB[i] = new CellState[state.Width];
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
        
        private void SetGameStateFromJsonString(string jsonString)
        {
            var state = JsonSerializer.Deserialize<GameState>(jsonString);
        
            // restore actual state from deserialized state
            NextMoveByPlayerA = state.NextMoveByPlayerA;
            _boardA = new CellState[state.Height, state.Width];
            _boardB = new CellState[state.Height, state.Width];
        
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
        
        
            var serializedGame = game.GetSerializedGameState();
        
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
        
            game.SetGameStateFromJsonString(jsonString);
        
            // BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA ? GetBoardA() : GetBoardB());
        
            return "";
        }
    }
}