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
        public bool NextMoveByPlayerA = true;
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
            PlayerA.SetNewBoard(_boardWidth, _boardHeight);
            PlayerB.SetNewBoard(_boardWidth, _boardHeight);
            PlayerA.SetPlayerType(options.PlayerAType);
            PlayerB.SetPlayerType(options.PlayerBType);
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
                PlayerA.PlaceRandomShips(_shipsCanTouch);
            }

            if (PlayerA.GetPlayerType() == EPlayerType.Human)
            {
                // Return true, if input is "Q - game over", else false.
                if (AskShipPlacementType(PlayerA, PlayerB)) return;
            }

            if (PlayerB.GetPlayerType() == EPlayerType.Ai)
            {
                PlayerB.PlaceRandomShips(_shipsCanTouch);
            }

            if (PlayerB.GetPlayerType() == EPlayerType.Human)
            {
                // Return true, if input is "Q - game over", else false.
                if (AskShipPlacementType(PlayerB, PlayerB)) return;
            }

            GameSaving.InitialSave(GetGameState());
            PlayRound();
        }

        public void PlayRound()
        {
            var gameOver = false;
            while (!gameOver)
            {
                gameOver = PlaceBombs();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GAME OVER!");
            if (OneOfPlayersHasLost())
            {
                Console.WriteLine($"Player {CheckForWinner().Name} WON!");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write("Press any key to exit...");
            Console.ReadKey();
            Console.Clear();
        }

        // Return true (game over) if one of the player has lost, else false.
        private bool PlaceBombs()
        {
            Console.Clear();
            Console.WriteLine();

            if (NextMoveByPlayerA)
            {
                if (PlayerA.GetPlayerType() == EPlayerType.Human)
                {
                    Console.Write($"{PlayerA.Name}, press ENTER to place bombs or Q to quit: ");
                    if (Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        Console.Clear();
                        return true;
                    }

                    if (HumanMakeAMove(PlayerA, PlayerB))
                    {
                        NextMoveByPlayerA = false;
                        GameSaving.SaveGameState(GetGameState());
                        return true;
                    }

                    NextMoveByPlayerA = false;
                    GameSaving.SaveGameState(GetGameState());
                }
                else
                {
                    if (AiMakeAMove(PlayerA, PlayerB))
                    {
                        NextMoveByPlayerA = false;
                        GameSaving.SaveGameState(GetGameState());
                        return true;
                    }

                    NextMoveByPlayerA = false;
                    GameSaving.SaveGameState(GetGameState());
                }
            }
            else
            {
                if (PlayerB.GetPlayerType() == EPlayerType.Human)
                {
                    Console.Write($"{PlayerB.Name}, press ENTER to place bombs or Q to quit: ");
                    if (Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        Console.Clear();
                        return true;
                    }

                    if (HumanMakeAMove(PlayerB, PlayerA))
                    {
                        NextMoveByPlayerA = true;
                        GameSaving.SaveGameState(GetGameState());
                        return true;
                    }

                    NextMoveByPlayerA = true;
                    GameSaving.SaveGameState(GetGameState());
                }
                else
                {
                    if (AiMakeAMove(PlayerB, PlayerA))
                    {
                        NextMoveByPlayerA = true;
                        GameSaving.SaveGameState(GetGameState());
                        return true;
                    }

                    NextMoveByPlayerA = true;
                    GameSaving.SaveGameState(GetGameState());
                }
            }

            return PlayerA.HasLost || PlayerB.HasLost;
        }

        // Return true, if game over or exit.
        private bool HumanMakeAMove(Player player, Player opponent)
        {
            Console.Clear();

            GameBoardUI.DrawBoards(player, opponent);
            Console.WriteLine();
            Console.WriteLine($"{player.Name}, place bomb!");
            var (column, row) = AskCoordinates(player, opponent);
            while (!Validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, opponent))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bomb already placed there!");
                Console.ForegroundColor = ConsoleColor.Cyan;
                (column, row) = AskCoordinates(player, opponent);
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
                    (column, row) = AskCoordinates(player, opponent);
                    while (!Validator.BombCoordinatesAreValid(column, row, _boardWidth, _boardHeight, opponent))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bomb already placed there!");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        (column, row) = AskCoordinates(player, opponent);
                    }

                    isHit = opponent.GetPlayerBoard()[column, row] != ECellState.Empty;
                    player.PlaceBomb(column, row, opponent);
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

                NextMoveByPlayerA = !NextMoveByPlayerA;
                GameSaving.SaveGameState(GetGameState());
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

                NextMoveByPlayerA = !NextMoveByPlayerA;
                GameSaving.SaveGameState(GetGameState());
                Console.Clear();
            }

            return false;
        }

        // Return true, if game over or exit.
        private bool AiMakeAMove(Player ai, Player player)
        {
            if (_nextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
            {
                var isHit = ai.PlaceRandomBomb(player);
                if (isHit)
                {
                    Console.WriteLine($"{ai.Name} HIT!");
                    Console.WriteLine();
                    GameBoardUI.DrawPlayerBoard(player);
                    Console.WriteLine();
                    GameSaving.SaveGameState(GetGameState());
                }
                else
                {
                    Console.WriteLine($"{ai.Name} MISSED!");
                    Console.WriteLine();
                    GameBoardUI.DrawPlayerBoard(player);
                    Console.WriteLine();
                    GameSaving.SaveGameState(GetGameState());
                }

                while (isHit)
                {
                    isHit = ai.PlaceRandomBomb(player);
                    if (isHit)
                    {
                        Console.WriteLine($"{ai.Name} HIT!");
                        Console.WriteLine();
                        GameBoardUI.DrawPlayerBoard(player);
                        Console.WriteLine();
                        GameSaving.SaveGameState(GetGameState());
                    }
                    else
                    {
                        Console.WriteLine($"{ai.Name} MISSED!");
                        Console.WriteLine();
                        GameBoardUI.DrawPlayerBoard(player);
                        Console.WriteLine();
                        GameSaving.SaveGameState(GetGameState());
                    }

                    GameSaving.SaveGameState(GetGameState());
                }

                GameSaving.SaveGameState(GetGameState());
            }
            else
            {
                var isHit = ai.PlaceRandomBomb(player);
                if (isHit)
                {
                    Console.WriteLine($"{ai.Name} HIT!");
                    Console.WriteLine();
                    GameBoardUI.DrawPlayerBoard(player);
                    Console.WriteLine();
                    GameSaving.SaveGameState(GetGameState());
                }
                else
                {
                    Console.WriteLine($"{ai.Name} MISSED!");
                    Console.WriteLine();
                    GameBoardUI.DrawPlayerBoard(player);
                    Console.WriteLine();
                    GameSaving.SaveGameState(GetGameState());
                }

                GameSaving.SaveGameState(GetGameState());
            }

            Console.Write("Press ENTER to continue or Q to quit: ");
            if (Console.ReadKey().Key != ConsoleKey.Q) return false;
            Console.Clear();
            return true;
        }

        private void PlaceShips(Player player, Player opponent)
        {
            Console.Clear();
            Console.WriteLine();
            foreach (var ship in player.GetShips())
            {
                Console.Clear();
                GameBoardUI.DrawPlayerBoard(player);
                Console.WriteLine();
                Console.WriteLine($"Player {player.GetName()} place ships.");
                Console.WriteLine($"Ship: {ship.Name}, Size: {ship.Width}x1");
                var orientation = EOrientation.Horizontal;
                var (column, row) = AskCoordinates(player, opponent);
                if (ship.Width > 1)
                {
                    orientation = AskOrientation();
                }

                var canPlaceShip = player.PlaceShip(column, row, ship, orientation, _shipsCanTouch);
                while (!canPlaceShip)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        "Ship can't be placed there. Out of bounds or another ship on the way. Please try again!");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    (column, row) = AskCoordinates(player, opponent);
                    if (ship.Width > 1)
                    {
                        orientation = AskOrientation();
                    }

                    canPlaceShip = player.PlaceShip(column, row, ship, orientation, _shipsCanTouch);
                }

                Console.Clear();
                GameBoardUI.DrawPlayerBoard(player);
            }

            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
            GameSaving.SaveGameState(GetGameState());
        }

        private static EOrientation AskOrientation()
        {
            Console.Write("Insert orientation Horizontal(H) or Vertical(V): ");
            string input = Console.ReadLine()!;

            while (!Validator.OrientationIsValid(input))
            {
                Console.Write("Please enter correct orientation Horizontal(H) or Vertical(V): ");
                input = Console.ReadLine()!;
            }

            var orientation = input.Trim().ToLower() switch
            {
                "h" => EOrientation.Horizontal,
                "v" => EOrientation.Vertical,
                _ => EOrientation.Horizontal
            };
            return orientation;
        }

        private (int x, int y) AskCoordinates(Player player, Player opponent)
        {
            Console.Write(
                $"Insert Column (A-{IntToAlphabeticValue(_boardWidth - 1)}) or press ENTER for random coordinates: ");
            string columnInput = Console.ReadLine() ?? "";
            if (columnInput == "")
            {
                return player.GetRandomBombCoordinates(opponent);
            }

            while (!Validator.ColumnInputIsValid(columnInput, _boardWidth))
            {
                Console.Write($"Please insert correct Column (A-{IntToAlphabeticValue(_boardWidth - 1)}): ");
                columnInput = Console.ReadLine()!;
            }

            var column = Validator.ConvertStringToInteger(columnInput);

            Console.Write($"Insert Row (1-{_boardHeight}): ");
            string rowInput = Console.ReadLine()!;
            while (!_validator.RowInputIsValid(rowInput, _boardHeight))
            {
                Console.Write($"Please enter correct row (1-{_boardHeight}): ");
                rowInput = Console.ReadLine()!;
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
                NextMoveByPlayerAState = NextMoveByPlayerA,
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

        private bool OneOfPlayersHasLost()
        {
            return PlayerA.HasLost || PlayerB.HasLost;
        }

        private Player CheckForWinner()
        {
            return PlayerA.HasLost ? PlayerB : PlayerA;
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

        private bool AskShipPlacementType(Player player, Player opponent)
        {
            Console.Write(
                $"{player.GetName()}, press ENTER to random ships, type A to place ships or Q to quit: ");
            var input = Console.ReadKey();

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            // Don't need all cases.
            switch (input.Key)
            {
                case ConsoleKey.Q:
                    Console.Clear();
                    return true;
                case ConsoleKey.A:
                    Console.Clear();
                    PlaceShips(player, opponent);
                    Console.Clear();
                    break;
                default:
                    player.PlaceRandomShips(_shipsCanTouch);
                    break;
            }

            return false;
        }
    }
}