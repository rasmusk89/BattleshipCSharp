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
            var NextMoveByPlayerA = true;
            
            var menu = new Menu(MenuLevel.Level1);

            menu.AddMenuItem(new MenuItem(
                $"Start Battleship (Default 10x10 board size)!",
                userChoice: "1",
                BattleshipDefaultSize)
            );

            menu.AddMenuItem(new MenuItem(
                $"Start Battleship (Set board size)!",
                userChoice: "2",
                BattleshipSetSize)
            );
            

            /*menu.AddMenuItem(new MenuItem(
                $"Start Battleship (Set board size)!",
                userChoice: "2",
                () =>
                {
                    BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA
                        ? GameBrain.Battleship.GetBoardA()
                        : GameBrain.Battleship.GetBoardB());
                    playGame.RunMenu();
                    return "";
                })
            );
            
            

            playGame.AddMenuItem(new MenuItem(
                $"Player {(game.NextMoveByPlayerA ? "A" : "B")} make a move",
                userChoice: "1",
                () =>
                {
                    var (x, y) = GameBrain.Battleship.GetMoveCoordinates();
                    game.MakeAMove(x, y);
                    BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA
                        ? GameBrain.Battleship.GetBoardA()
                        : GameBrain.Battleship.GetBoardB());
                    return "";
                })
            );*/


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


            var userChoice = menu.RunMenu();


            return userChoice;
        }

        private static string BattleshipDefaultSize()
        {
            var game = new Battleship();

            var playGame = new Menu(MenuLevel.Level2Plus);

            playGame.AddMenuItem(new MenuItem(
                $"Player {(game.NextMoveByPlayerA ? "A" : "B")} make a move",
                userChoice: "1",
                () =>
                {
                    var (x, y) = GameBrain.Battleship.GetMoveCoordinates();
                    Console.WriteLine(x + ", " + y);
                    game.MakeAMove(x, y);
                    BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA
                        ? GameBrain.Battleship.GetBoardA()
                        : GameBrain.Battleship.GetBoardB());
                    return "";
                })
            );

            BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA
                ? GameBrain.Battleship.GetBoardA()
                : GameBrain.Battleship.GetBoardB());
            playGame.RunMenu();
            return "";
        }

        private static string BattleshipSetSize()
        {
            Console.WriteLine("Set height: ");
            Console.Write(">");

            var insertedHeight = Console.ReadLine();

            Console.WriteLine("Set width: ");
            Console.Write(">");

            var insertedWidth = Console.ReadLine();

            var height = int.Parse(insertedHeight ?? "10");
            var width = int.Parse(insertedWidth ?? "10");

            var game = new Battleship(width, height);

            var playGame = new Menu(MenuLevel.Level1);

            BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA
                ? GameBrain.Battleship.GetBoardA()
                : GameBrain.Battleship.GetBoardB());
            playGame.RunMenu();
            return "";
        }
    }
}