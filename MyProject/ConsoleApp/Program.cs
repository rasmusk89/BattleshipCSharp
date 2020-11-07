using System;
using System.Linq;
using MenuSystem;
using GameBrain;
using GameConsoleUI;

namespace ConsoleApp
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+---------------------------+\n" +
                              "|        RASMUS GAME        |\n" +
                              "+---------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;

            var menu = new Menu(MenuLevel.Level0);
            // menu.AddMenuItem(new MenuItem("Battleship", "B", Battleship));
            menu.AddMenuItem(new MenuItem("Battleship", "B",
                () =>
                {
                    Console.Write("Player A, enter your name: ");
                    string playerAName = Console.ReadLine() ?? "PlayerA";

                    Console.Write("Player B, enter your name: ");
                    string playerBName = Console.ReadLine() ?? "PlayerB";

                    Console.WriteLine("Set game board height: ");
                    var height = int.Parse(Console.ReadLine() ?? "10");
                    
                    Console.WriteLine("Set game board width: ");
                    var width = int.Parse(Console.ReadLine() ?? "10");
                    
                    Player playerA = new Player(playerAName, width, height);
                    Player playerB = new Player(playerBName, width, height);
                    var game = new Game(playerA, playerB);
                    game.PlayRound();
                    return "";
                }));

            menu.RunMenu();
        }

        private static string Battleship()
        {
            var height = 10;
            var width = 10;
            
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("<<<-*-*- BATTLESHIP -*-*->>>");
            Console.ForegroundColor = ConsoleColor.Cyan;

            var menu = new Menu(MenuLevel.Level1);

            menu.AddMenuItem(new MenuItem(
                $"Start Game",
                userChoice: "S",
                () =>
                {
                    var game = new Battleship(width, height);

                    do
                    {
                        var nextMoveByA = game.GetNextMove();
                        DrawBattleField(nextMoveByA);

                        Console.Write("Give bomb coordinates separated with comma\n(for example - A,1): ");
                        string userInput = Console.ReadLine()?? "";
                        if (userInput.ToLower() == "m")
                        {
                           break;
                        }

                        while (!game.CoordinatesAreValid(userInput))
                        {
                            Console.Write("Please enter correct coordinates separated with comma\n(for example - A,1): ");
                            userInput = Console.ReadLine() ?? "";
                        }

                        while (game.CellHasBomb(userInput, nextMoveByA))
                        {
                            Console.WriteLine("Bomb has already placed there!");
                            Console.Write("Please enter another coordinates separated with comma\n(for example - A,1): ");
                            userInput = Console.ReadLine() ?? "";
                        }

                        var (x, y) = game.GetMoveCoordinates(userInput);
                        game.MakeAMove(x, y);

                        DrawBattleField(nextMoveByA);
                        Console.Write("Press ENTER to continue...");
                        Console.ReadLine();
                        
                        Console.Clear();
                        Console.Write($"...Player {(nextMoveByA ? "B" : "A")} press ENTER to make your move.");
                        Console.ReadLine();



                        // DrawBattleField(nextMoveByA);
                        // Console.Write("Press ENTER to accept move or \"C\" to move again: ");
                        // userInput = Console.ReadLine() ?? "";
                        //
                        // if (userInput.ToLower() == "c")
                        // {
                        //     game.RemoveAMove(x,y);
                        //     DrawBattleField(nextMoveByA);
                        //     Console.Write("Give bomb coordinates separated with comma\n(for example - A,1): ");
                        // }

                    } while (true);

                    return "";
                }
            ));

            menu.AddMenuItem(new MenuItem(
                $"Options",
                userChoice: "O",
                () =>
                {
                    Console.WriteLine("Set the game board width: ");
                    Console.Write(">");

                    var insertedHeight = Console.ReadLine();

                    Console.WriteLine("Set the game board height: ");
                    Console.Write(">");

                    var insertedWidth = Console.ReadLine();

                    height = int.Parse(insertedHeight!);
                    width = int.Parse(insertedWidth!);
                    return "";
                })
            );

            var userChoice = menu.RunMenu();

            return userChoice;
        }

        private static void DrawBattleField(bool nextMoveByA)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"...Player {(nextMoveByA ? "A" : "B")} turn...");
            Console.WriteLine();
            Console.WriteLine("YOUR BOARD");
            BattleshipConsoleUI.DrawBoard(nextMoveByA
                ? GameBrain.Battleship.GetBoardA()
                : GameBrain.Battleship.GetBoardB());
            Console.WriteLine();
            Console.WriteLine("OPPONENT BOARD");
            BattleshipConsoleUI.DrawBoard(nextMoveByA
                ? GameBrain.Battleship.GetBoardB()
                : GameBrain.Battleship.GetBoardA());
        }
        
        private static string DefaultMenuAction()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nNot implemented yet!");
            Console.ForegroundColor = ConsoleColor.Cyan;

            return "";
        }

    }
    
    
}