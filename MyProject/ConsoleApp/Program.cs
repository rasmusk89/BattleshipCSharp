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
            menu.AddMenuItem(new MenuItem("Battleship", "B", Battleship));

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
                    var game = new Battleship(height, width);

                    do
                    {
                        var nextMoveByA = game.GetNextMove();

                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine($"...Player {(nextMoveByA ? "A" : "B")} turn...");
                        Console.WriteLine();
                        Console.WriteLine($"PLAYER {(nextMoveByA ? "A" : "B")} BOARD");
                        BattleshipConsoleUI.DrawBoard(nextMoveByA
                            ? GameBrain.Battleship.GetBoardA()
                            : GameBrain.Battleship.GetBoardB());
                        Console.WriteLine();
                        Console.WriteLine($"PLAYER {(nextMoveByA ? "B" : "A")} BOARD");
                        BattleshipConsoleUI.DrawBoard(nextMoveByA
                            ? GameBrain.Battleship.GetBoardB()
                            : GameBrain.Battleship.GetBoardA());

                        var (x, y) = GameBrain.Battleship.GetMoveCoordinates();
                        GameBrain.Battleship.MakeAMove(y, x);

                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine($"...Player {(nextMoveByA ? "A" : "B")} turn...");
                        Console.WriteLine();
                        Console.WriteLine($"PLAYER {(nextMoveByA ? "A" : "B")} BOARD");
                        BattleshipConsoleUI.DrawBoard(nextMoveByA
                            ? GameBrain.Battleship.GetBoardA()
                            : GameBrain.Battleship.GetBoardB());
                        Console.WriteLine();
                        Console.WriteLine($"PLAYER {(nextMoveByA ? "B" : "A")} BOARD");
                        BattleshipConsoleUI.DrawBoard(nextMoveByA
                            ? GameBrain.Battleship.GetBoardB()
                            : GameBrain.Battleship.GetBoardA());
                        Console.Write("Press any key to continue...");
                        Console.ReadLine();
                        Console.Clear();
                        Console.Write($"Player {(nextMoveByA ? "B" : "A")}, press any key to make a move...");
                        Console.ReadLine();

                    } while (true);
                }
            ));

            menu.AddMenuItem(new MenuItem(
                $"Options",
                userChoice: "O",
                () =>
                {
                    Console.WriteLine("Set the game board height: ");
                    Console.Write(">");

                    var insertedHeight = Console.ReadLine();

                    Console.WriteLine("Set the game board width: ");
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


        private static string PlayBattleship()
        {
            
            // var game = new Battleship(width, height);
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            

            // do
            // {
            //     var nextMoveByA = GameBrain.Battleship.NextMoveByPlayerA;
            //
            //     Console.WriteLine();
            //     Console.WriteLine($"Player {(nextMoveByA ? "A" : "B")} turn");
            //     BattleshipConsoleUI.DrawBoard(nextMoveByA
            //         ? GameBrain.Battleship.GetBoardA()
            //         : GameBrain.Battleship.GetBoardB());
            //
            //     var menu = new Menu(MenuLevel.Level2Plus);
            //
            //     menu.AddMenuItem(new MenuItem(
            //         $"Make a move",
            //         userChoice: "S",
            //         () =>
            //         {
            //             Console.Write("Give bomb coordinates separated with comma (example - A,1): ");
            //             var input = Console.ReadLine();
            //             Console.WriteLine($"Coordinates: {input}");
            //             return "";
            //         }
            //     ));
            //
            //
            // } while (true);
            

            // BattleshipConsoleUI.DrawBoard(nextMoveByA
            //     ? GameBrain.Battleship.GetBoardA()
            //     : GameBrain.Battleship.GetBoardB());
            //
            // Console.WriteLine();
            // Console.WriteLine($"Player {(nextMoveByA ? "A" : "B")} turn");

            // Console.Write("Give bomb coordinates separated with comma (example - A,1): ");
            // var input = Console.ReadLine();


            // MAKE CORRECT GETMOVECOORDINATES AND OTHER METHODS TO GET THE CORRECT COORDINATE!
            // var (x, y) = GameBrain.Battleship.GetMoveCoordinates(input);
            // GameBrain.Battleship.MakeAMove(x, y);
            // Console.WriteLine();
            // var userChoice = menu.RunMenu();

            return "";
        }
        
        

        //  playGame.AddMenuItem(new MenuItem(
        //     $"Save game",
        //     userChoice: "s",
        //     () => GameBrain.Battleship.SaveGameAction(game))
        // );
        //
        // menu.AddMenuItem(new MenuItem(
        //     $"Load game",
        //     userChoice: "l",
        //     () => GameBrain.Battleship.LoadGameAction(game))
        // );

        private static string MakeAMove()
        {
            
            return "";
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