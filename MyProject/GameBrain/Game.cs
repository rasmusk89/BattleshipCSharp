using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ENextMoveAfterHit _nextMoveAfterHit;
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
                Name = "Player 1",
                ShipsCanTouch = options.ShipsCanTouch
            };
            // Need to order ships by descending because placing random ships should start with longest ship.
            PlayerA.SetShips(Ships.OrderByDescending(x => x.Width));
            PlayerB = new Player()
            {
                Name = "Player 2",
                ShipsCanTouch = options.ShipsCanTouch
            };
            // Need to order ships by descending because placing random ships should start with longest ship.
            PlayerB.SetShips(Ships.OrderByDescending(x => x.Width));
            PlayerA.SetBoard(_boardWidth, _boardHeight);
            PlayerB.SetBoard(_boardWidth, _boardHeight);
            _shipsCanTouch = options.GetShipsCanTouch();
            _gameOptions = options;
            _nextMoveAfterHit = options.NextMoveAfterHit;
        }

        public void StartGame()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+----------------------------+\n" +
                              "| < - - - BATTLESHIP - - - > |\n" +
                              "+----------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;

            PlayerA.SetName(PlayerA.GetPlayerType() == EPlayerType.Ai ? "AI One" : AskPlayerName());

            PlayerB.SetName(PlayerB.GetPlayerType() == EPlayerType.Ai ? "AI Two" : AskPlayerName());

            if (PlayerA.GetPlayerType() == EPlayerType.Ai)
            {
                PlayerA.PlaceRandomShips();
            }

            if (PlayerA.GetPlayerType() == EPlayerType.Human)
            {
                Console.Write("Press ENTER to random ships or type \"A\" to place ships: ");
                string input = Console.ReadLine() ?? "";

                if (input.ToLower() == "a")
                {
                    Console.Clear();
                    PlaceShips(PlayerA);
                    Console.Clear();
                }
                else
                {
                    PlayerA.PlaceRandomShips();
                }
            }


            if (PlayerB.GetPlayerType() == EPlayerType.Ai)
            {
                PlayerB.PlaceRandomShips();
            }

            if (PlayerB.GetPlayerType() == EPlayerType.Human)
            {
                Console.Write("Press ENTER to random ships or type \"A\" to place ships: ");
                string input = Console.ReadLine() ?? "";

                if (input.ToLower() == "a")
                {
                    Console.Clear();
                    PlaceShips(PlayerB);
                    Console.Clear();
                }
                else
                {
                    PlayerB.PlaceRandomShips();
                }
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
                GameSaving.SaveGameState(GetGameState());
            }

            Console.WriteLine();
            Console.WriteLine("GAME OVER!");
            if (PlayerA.HasLost)
            {
                Console.WriteLine(PlayerB.Name + " WON!");
            }

            if (PlayerB.HasLost)
            {
                Console.WriteLine(PlayerA.Name + " WON!");
            }
        }

        private static string AskPlayerName()
        {
            Console.Write("Please enter your name: ");
            string playerName = Console.ReadLine() ?? "";
            if (playerName == "")
            {
                playerName = "Player";
            }

            return playerName;
        }

        // Return true (game over) if one of the player has lost, else false.
        private bool PlaceBombs(Player playerA, Player playerB)
        {
            Console.WriteLine("Start");
            Console.Clear();
            Console.WriteLine();
            if (_nextMoveByPlayerA)
            {
                if (PlayerA.GetPlayerType() == EPlayerType.Ai)
                {
                    switch (_nextMoveAfterHit)
                    {
                        case ENextMoveAfterHit.SamePlayer:
                        {
                            while (PlayerA.PlaceRandomBomb(PlayerB))
                            {
                                Console.WriteLine();
                                Console.WriteLine("AI A HIT");
                                Console.WriteLine();
                                GameBoardUI.DrawBoards(PlayerA, PlayerB);
                                Console.WriteLine();
                                Console.Write("Continue..");
                                Console.ReadLine();
                                Console.Clear();
                            }

                            Console.WriteLine();
                            Console.WriteLine("AI A MISSED!");
                            Console.WriteLine();
                            GameBoardUI.DrawBoards(PlayerA, PlayerB);
                            Console.WriteLine();
                            Console.Write("Continue..");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        }
                        case ENextMoveAfterHit.OtherPlayer:
                            Console.WriteLine(PlayerA.PlaceRandomBomb(PlayerB) ? "AI HIT!" : "AI MISSED!");
                            Console.WriteLine();
                            GameBoardUI.DrawPlayerBoard(PlayerA);
                            Console.WriteLine();
                            Console.Write("Continue..");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _nextMoveByPlayerA = !_nextMoveByPlayerA;
                }
                else
                {
                    Console.Write($"{playerA.Name}, press ENTER to place bombs!");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        return true;
                    }

                    Console.Clear();
                    GameBoardUI.DrawBoards(playerA, playerB);
                    Console.WriteLine();
                    Console.WriteLine($"{playerA.Name}, place bomb!");
                    var (column, row) = AskCoordinates();
                    while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, playerB))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bomb already placed there!");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        (column, row) = AskCoordinates();
                    }

                    var isHit = playerB.GetPlayerBoard()[column, row] != ECellState.Empty;
                    playerA.PlaceBomb(column, row, playerB);
                    if (_nextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHit)
                        {
                            Console.Clear();
                            GameBoardUI.DrawBoards(playerA, playerB);
                            if (playerB.HasLost)
                            {
                                Console.WriteLine("HIT");
                                break;
                            }

                            Console.WriteLine(isHit ? "HIT! Move again" : "MISS! Press enter to continue..");
                            Console.WriteLine($"{playerA.Name}, place bomb!");
                            (column, row) = AskCoordinates();
                            while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, playerB))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Bomb already placed there!");
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                (column, row) = AskCoordinates();
                            }

                            isHit = playerB.GetPlayerBoard()[column, row] != ECellState.Empty;
                            playerA.PlaceBomb(column, row, playerB);
                        }

                        if (playerB.HasLost)
                        {
                            return true;
                        }

                        Console.Clear();
                        GameBoardUI.DrawBoards(playerA, playerB);
                        Console.WriteLine();
                        Console.Write(isHit ? "HIT! Press enter to continue.." : "MISS! Press enter to continue..");
                        if (Console.ReadKey().Key == ConsoleKey.Escape)
                        {
                            return true;
                        }

                        _nextMoveByPlayerA = !_nextMoveByPlayerA;
                        Console.Clear();
                    }
                    else
                    {
                        Console.Clear();
                        GameBoardUI.DrawBoards(playerA, playerB);
                        Console.WriteLine();
                        Console.Write(isHit ? "HIT! Press enter to continue.." : "MISS! Press enter to continue..");
                        if (Console.ReadKey().Key == ConsoleKey.Escape)
                        {
                            return true;
                        }

                        _nextMoveByPlayerA = !_nextMoveByPlayerA;
                        Console.Clear();
                    }
                }
            }
            else
            {
                if (PlayerB.GetPlayerType() == EPlayerType.Ai)
                {
                    switch (_nextMoveAfterHit)
                    {
                        case ENextMoveAfterHit.SamePlayer:
                        {
                            while (PlayerB.PlaceRandomBomb(PlayerA))
                            {
                                Console.WriteLine();
                                Console.WriteLine("AI B HIT");
                                Console.WriteLine();
                                GameBoardUI.DrawBoards(PlayerB, PlayerA);
                                Console.WriteLine();
                                Console.Write("Continue..");
                                Console.ReadLine();
                                Console.Clear();
                            }

                            Console.WriteLine();
                            Console.WriteLine("AI B MISSED!");
                            Console.WriteLine();
                            GameBoardUI.DrawBoards(PlayerB, PlayerA);
                            Console.WriteLine();
                            Console.Write("Continue..");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        }
                        case ENextMoveAfterHit.OtherPlayer:
                            Console.WriteLine(PlayerB.PlaceRandomBomb(PlayerA) ? "AI B HIT!" : "AI B MISSED!");
                            Console.WriteLine();
                            GameBoardUI.DrawPlayerBoard(PlayerB);
                            Console.WriteLine();
                            Console.Write("Continue..");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _nextMoveByPlayerA = true;
                }
                else
                {
                    Console.Write($"{playerB.Name}, press ENTER to place bombs!");
                    if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        return true;
                    }

                    Console.Clear();
                    GameBoardUI.DrawBoards(playerB, playerA);
                    Console.WriteLine();
                    Console.WriteLine($"{playerB.Name}, place bomb!");
                    var (column, row) = AskCoordinates();
                    while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, playerA))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bomb already placed there!");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        (column, row) = AskCoordinates();
                    }

                    var isHit = playerA.GetPlayerBoard()[column, row] != ECellState.Empty;
                    playerB.PlaceBomb(column, row, playerA);
                    if (_nextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHit)
                        {
                            Console.Clear();
                            GameBoardUI.DrawBoards(playerB, playerA);
                            if (playerA.HasLost)
                            {
                                Console.WriteLine("HIT");
                                break;
                            }

                            Console.WriteLine(isHit ? "HIT! Move again" : "MISS! Press enter to continue..");
                            Console.WriteLine($"{playerB.Name}, place bomb!");
                            (column, row) = AskCoordinates();
                            while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, playerA))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Bomb already placed there!");
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                (column, row) = AskCoordinates();
                            }

                            isHit = playerA.GetPlayerBoard()[column, row] != ECellState.Empty;
                            playerB.PlaceBomb(column, row, playerA);
                        }

                        if (playerA.HasLost)
                        {
                            return true;
                        }

                        Console.Clear();
                        GameBoardUI.DrawBoards(playerB, playerA);
                        Console.WriteLine();
                        Console.Write(isHit ? "HIT! Press enter to continue.." : "MISS! Press enter to continue..");
                        if (Console.ReadKey().Key == ConsoleKey.Escape)
                        {
                            return true;
                        }

                        _nextMoveByPlayerA = true;
                        Console.Clear();
                    }
                    else
                    {
                        Console.Clear();
                        GameBoardUI.DrawBoards(playerB, playerA);
                        Console.WriteLine();
                        Console.Write(isHit ? "HIT! Press enter to continue.." : "MISS! Press enter to continue..");
                        if (Console.ReadKey().Key == ConsoleKey.Escape)
                        {
                            return true;
                        }

                        _nextMoveByPlayerA = true;
                        Console.Clear();
                    }
                }
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

                while (!Validator.ShipCoordinatesAreValid(column, row, _boardWidth, _boardHeight, ship, orientation))
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

                while (!player.ShipAreaFree(column, row, player.GameBoard, ship, orientation, _shipsCanTouch))
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

        private GameState GetGameState()
        {
            var state = new GameState()
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

        private bool PlaceBombsKeyboard(Player playerA, Player playerB)
        {
            Console.Clear();
            Console.WriteLine();
            if (_nextMoveByPlayerA)
            {
                var x = 0;
                var y = 0;

                Console.Write("Enter any Key: ");
                var name = Console.ReadKey();
                if (name.Key == ConsoleKey.Escape)
                {
                    return true;
                }

                Console.WriteLine($"You pressed {name.KeyChar}");


                GameBoardUI.DrawBoards(playerA, playerB);
            }
            else
            {
                GameBoardUI.DrawBoards(playerB, playerA);
            }


            return false;
        }
    }
}