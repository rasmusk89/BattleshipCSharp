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

            var playGame = new Menu(MenuLevel.Level1);
            playGame.AddMenuItem(new MenuItem(
                $"Player {(game.NextMoveByPlayerA  ? "A" : "B")} make a move",
                userChoice: "1",
                () =>
                {
                    var playerABoard = game.GetBoard();
                    var playerBBoard = game.GetBoard();
                    
                    var (x, y) = GetMoveCoordinates(game);
                    game.MakeAMove(x, y);
                    BattleshipConsoleUI.DrawBoard(game.GetBoard());
                    return "";
                })
            );

            var menu = new Menu(MenuLevel.Level1);
            menu.AddMenuItem(new MenuItem(
                $"Start Battleship game!",
                userChoice: "1",
                () =>
                {
                    SetGameBoardSize(game);
                    BattleshipConsoleUI.DrawBoard(game.GetBoard());
                    playGame.RunMenu();
                    return "";
                })
            );
            
            var userChoice = menu.RunMenu();

            return userChoice;
        }

        private static void SetGameBoardSize(Battleship game)
        {
            // Set the game board size...
            Console.Write("Choose game board size (5 - 26): ");
            string size = Console.ReadLine() ?? "8";
            if (size == "")
            {
                size = "8";
            }

            GameBrain.Battleship.SetSize(int.Parse(size));
        }

        private static (int x, int y) GetMoveCoordinates(Battleship game)
        {
            
            Console.Write("Give bomb coordinates separated with comma (example - A,1): ");

            var userInput = Console.ReadLine().Split(",");

            var x = char.Parse(userInput[0].ToUpper()) - 65;
            var y = int.Parse(userInput[1]) - 1;
            
            Console.WriteLine(x);
            Console.WriteLine(y);

            return (y, x);
        }

    }
    
}