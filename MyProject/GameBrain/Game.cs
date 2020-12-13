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
            PlayerA.PlayerType = options.PlayerAType;
            PlayerB.PlayerType = options.PlayerBType;
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
            if (PlayerA.GetName() == "Player")
            {
                PlayerA.Name += " One";
            }

            PlayerB.SetName(PlayerB.GetPlayerType() == EPlayerType.Ai ? "AI Two" : AskPlayerName());
            if (PlayerB.GetName() == "Player")
            {
                PlayerB.Name += " Two";
            }

            if (PlayerA.GetPlayerType() == EPlayerType.Ai)
            {
                if (!PlayerA.PlaceRandomShips(_shipsCanTouch))
                {
                    return;
                }
            }

            if (PlayerA.GetPlayerType() == EPlayerType.Human)
            {
                Console.Write(
                    $"{PlayerA.GetName()}, press ENTER to random ships, type A to place ships or Q to quit: ");
                var input = Console.ReadKey();
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                // Don't need all cases.
                switch (input.Key)
                {
                    case ConsoleKey.Q:
                        Console.Clear();
                        return;
                    case ConsoleKey.A:
                        Console.Clear();
                        PlaceShips(PlayerA);
                        Console.Clear();
                        break;
                    default:
                        if (!PlayerA.PlaceRandomShips(_shipsCanTouch))
                        {
                            return;
                        }
                        break;
                }
            }

            if (PlayerB.GetPlayerType() == EPlayerType.Ai)
            {
                if (!PlayerB.PlaceRandomShips(_shipsCanTouch))
                {
                    return;
                }
               
            }

            if (PlayerB.GetPlayerType() == EPlayerType.Human)
            {
                Console.Write(
                    $"{PlayerB.GetName()}, press ENTER to random ships, type A to place ships or Q to quit: ");
                var input = Console.ReadKey();
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                // Don't need all cases.
                switch (input.Key)
                {
                    case ConsoleKey.Q:
                        Console.Clear();
                        return;
                    case ConsoleKey.A:
                        Console.Clear();
                        PlaceShips(PlayerB);
                        Console.Clear();
                        break;
                    default:
                        if (!PlayerB.PlaceRandomShips(_shipsCanTouch))
                        {
                            return;
                        }
                        break;
                }
            }

            // GameSaving.InitialSave(GetGameState());
            PlayRound();
        }

        public void PlayRound()
        {
            var gameOver = false;
            while (!gameOver)
            {
                gameOver = PlaceBombs();
            }

            if (PlayerA.HasLost)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("GAME OVER");
                Console.WriteLine(PlayerB.Name + " WON!");
            }

            else if (PlayerB.HasLost)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("GAME OVER");
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
        private bool PlaceBombs()
        {
            Console.Clear();
            Console.WriteLine();

            if (PlayerA.GetPlayerType() == EPlayerType.Human && PlayerB.GetPlayerType() == EPlayerType.Ai)
            {
                bool exit;
                if (_nextMoveByPlayerA)
                {
                    exit = HumanMakeAMove(PlayerA, PlayerB);
                    if (exit)
                    {
                        return true;
                    }
                }
                else
                {
                    exit = AiVsHumanPlaceBomb(PlayerB, PlayerA);
                    if (exit)
                    {
                        return true;
                    }

                    _nextMoveByPlayerA = true;
                }
            }
            else
            {
                // Player A Move
                bool exit;
                if (_nextMoveByPlayerA)
                {
                    if (PlayerA.GetPlayerType() == EPlayerType.Ai)
                    {
                        switch (_nextMoveAfterHit)
                        {
                            case ENextMoveAfterHit.SamePlayer:
                            {
                                exit = AiSamePLayerPlaceBomb(PlayerA, PlayerB);
                                if (exit)
                                {
                                    return true;
                                }

                                break;
                            }
                            case ENextMoveAfterHit.OtherPlayer:
                                exit = AiOtherPlayerPlaceBomb(PlayerA, PlayerB);
                                if (exit)
                                {
                                    return true;
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _nextMoveByPlayerA = !_nextMoveByPlayerA;
                    }

                    else
                    {
                        if (HumanMakeAMove(PlayerA, PlayerB)) return true;
                    }
                }
                // Player B Move
                else
                {
                    if (PlayerB.GetPlayerType() == EPlayerType.Ai)
                    {
                        // bool exit;
                        switch (_nextMoveAfterHit)
                        {
                            case ENextMoveAfterHit.SamePlayer:
                            {
                                exit = AiSamePLayerPlaceBomb(PlayerB, PlayerA);
                                if (exit)
                                {
                                    return true;
                                }

                                break;
                            }
                            case ENextMoveAfterHit.OtherPlayer:
                                exit = AiOtherPlayerPlaceBomb(PlayerB, PlayerA);
                                if (exit)
                                {
                                    return true;
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _nextMoveByPlayerA = true;
                    }
                    else
                    {
                        if (HumanMakeAMove(PlayerB, PlayerA)) return true;
                    }
                }
            }

            return PlayerA.HasLost || PlayerB.HasLost;
        }

        private bool HumanMakeAMove(Player player, Player opponent)
        {
            Console.Write($"{player.Name}, press ENTER to place bombs or Q to quit: ");
            if (Console.ReadKey().Key == ConsoleKey.Q)
            {
                Console.Clear();
                return true;
            }

            Console.Clear();

            GameBoardUI.DrawBoards(player, opponent);
            Console.WriteLine();
            Console.WriteLine($"{player.Name}, place bomb!");
            var (column, row) = AskCoordinates();
            while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, opponent))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bomb already placed there!");
                Console.ForegroundColor = ConsoleColor.Cyan;
                (column, row) = AskCoordinates();
            }

            var isHit = opponent.GetPlayerBoard()[column, row] != ECellState.Empty;
            player.PlaceBomb(column, row, opponent);
            // GameSaving.SaveGameState(GetGameState());
            if (_nextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
            {
                while (isHit)
                {
                    Console.Clear();
                    GameBoardUI.DrawBoards(player, opponent);
                    if (opponent.HasLost)
                    {
                        Console.WriteLine("HIT");
                        break;
                    }

                    Console.WriteLine(isHit ? "HIT! Move again" : "MISS! Press enter to continue..");
                    Console.WriteLine($"{player.Name}, place bomb!");
                    (column, row) = AskCoordinates();
                    while (!_validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, opponent))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bomb already placed there!");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        (column, row) = AskCoordinates();
                    }

                    isHit = opponent.GetPlayerBoard()[column, row] != ECellState.Empty;
                    player.PlaceBomb(column, row, opponent);
                    // GameSaving.SaveGameState(GetGameState());
                }

                if (opponent.HasLost)
                {
                    return true;
                }

                Console.Clear();
                GameBoardUI.DrawBoards(player, opponent);
                Console.WriteLine();
                Console.Write(isHit
                    ? "HIT! Press enter to continue or Q to quit: "
                    : "MISS! Press enter to continue or Q to quit: ");
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    Console.Clear();
                    return true;
                }

                _nextMoveByPlayerA = !_nextMoveByPlayerA;
                Console.Clear();
            }
            else
            {
                Console.Clear();
                GameBoardUI.DrawBoards(player, opponent);
                Console.WriteLine();
                Console.Write(isHit
                    ? "HIT! Press enter to continue or Q to quit: "
                    : "MISS! Press enter to continue or Q to quit: ");
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    Console.Clear();
                    return true;
                }

                _nextMoveByPlayerA = !_nextMoveByPlayerA;
                Console.Clear();
            }

            return false;
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
            var state = new GameState
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


        private static bool AiOtherPlayerPlaceBomb(Player player, Player opponent)
        {
            Console.WriteLine(player.PlaceRandomBomb(opponent)
                ? $"{player.GetName()} HIT!"
                : $"{player.GetName()} MISSED!");
            // GameSaving.SaveGameState(GetGameState());
            Console.WriteLine();
            GameBoardUI.DrawBoards(player, opponent);
            Console.WriteLine();
            Console.Write("Press ENTER to continue or Q to quit: ");
            if (Console.ReadKey().Key == ConsoleKey.Q)
            {
                Console.Clear();
                return true;
            }

            Console.Clear();
            return false;
        }

        private static bool AiSamePLayerPlaceBomb(Player player, Player opponent)
        {
            var playerHit = player.PlaceRandomBomb(opponent);
            // GameSaving.SaveGameState(GetGameState());
            while (playerHit)
            {
                Console.WriteLine();
                Console.WriteLine($"{player.GetName()} HIT!");
                Console.WriteLine();
                GameBoardUI.DrawBoards(player, opponent);
                Console.WriteLine();
                Console.Write("Press ENTER to continue or Q to quit: ");
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    Console.Clear();
                    return true;
                }

                Console.Clear();
                playerHit = player.PlaceRandomBomb(opponent);
                // GameSaving.SaveGameState(GetGameState());
            }

            Console.WriteLine();
            Console.WriteLine($"{player.GetName()} MISSED!");
            Console.WriteLine();
            GameBoardUI.DrawBoards(player, opponent);
            Console.WriteLine();
            Console.Write("Press ENTER to continue or Q to quit: ");
            if (Console.ReadKey().Key == ConsoleKey.Q)
            {
                Console.Clear();
                return true;
            }

            Console.Clear();
            return false;
        }

        private bool AiVsHumanPlaceBomb(Player ai, Player player)
        {
            var isHit = ai.PlaceRandomBomb(player);
            if (_nextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
            {
                while (isHit)
                {
                    isHit = ai.PlaceRandomBomb(player);
                }
            }

            Console.WriteLine(ai.PlaceRandomBomb(player) ? "AI HIT!" : "AI MISSED!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            GameBoardUI.DrawPlayerBoard(player);
            Console.WriteLine();
            Console.Write("Press ENTER to continue or Q to quit: ");
            if (Console.ReadKey().Key != ConsoleKey.Q) return false;
            Console.Clear();
            return true;

        }
    }
}