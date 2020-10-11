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


            var menuC = new Menu(MenuLevel.Level2Plus);

            var menuB = new Menu(MenuLevel.Level1);

            menuB.AddMenuItem(new MenuItem("Go to Level2", "A", menuC.RunMenu));

            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Battleship", "B", Battleship));
            menu.AddMenuItem(new MenuItem("Go to Level1", "A", menuB.RunMenu));

            menu.RunMenu();
        }

        private static string DefaultMenuAction()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nNot implemented yet!");
            Console.ForegroundColor = ConsoleColor.Cyan;

            return "";
        }

        private static string Battleship()
        {
            
            var game = new Battleship();
            
            BattleshipConsoleUI.DrawBoard(game.GetBoard());
            
            var menu = new Menu(MenuLevel.Level1);
            menu.AddMenuItem(new MenuItem(
                $"Set game board size or default(8x8)",
                userChoice: "1",
                () =>
                {
                    var (x, y) = SetGameBoardSize(game);
                    game.MakeAMove(x, y);
                    BattleshipConsoleUI.DrawBoard(game.GetBoard());
                    return "";
                })
            );
            menu.AddMenuItem(new MenuItem(
                $"Player {(game.NextMoveByPlayerA ? "A" : "B")} make a move",
                userChoice: "2",
                () =>
                {
                    var (x, y) = GetMoveCoordinates(game);
                    game.MakeAMove(x, y);
                    BattleshipConsoleUI.DrawBoard(game.GetBoard());
                    return "";
                })
            );

            var userChoice = menu.RunMenu();


            return userChoice;

        }

        private static (int x, int y) SetGameBoardSize(Battleship game)
        {
            // Set the game board size...
            
            return (1, 2);
        }
        
        private static (int x, int y) GetMoveCoordinates(Battleship game)
        {
            
            Console.Write("Give bomb coordinates: ");

            string userInput = Console.ReadLine();
            
            var userValues = userInput!.ToCharArray();

            var x = (int)char.GetNumericValue(userValues[0]) - 1;
            var y = (int)char.GetNumericValue(userValues[1]) - 1;
            
            Console.WriteLine(x);
            Console.WriteLine(y);

            return (x, y);
        }

        private static bool ValidateCoordinates(string coordinates, int width, int height)
        {
            char[] arrayOfCoordinates = coordinates.ToCharArray();

            return arrayOfCoordinates.Length == 2;
        }
    }
}