using System;
using System.Runtime.CompilerServices;
using GameBrain;
using GameConsoleUI;
using MenuSystem;

namespace ConsoleApp
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=====> RASKIL GAME <=====");

            var menuE = new Menu(MenuLevel.Level2Plus);
            menuE.AddMenuItem(new MenuItem("Go to Level5", "A", DefaultMenuAction));

            var menuD = new Menu(MenuLevel.Level2Plus);
            menuD.AddMenuItem(new MenuItem("Go to Level4", "A", menuE.RunMenu));

            var menuC = new Menu(MenuLevel.Level2Plus);
            menuC.AddMenuItem(new MenuItem("Go to Level3", "A", menuD.RunMenu));

            var menuB = new Menu(MenuLevel.Level1);
            menuB.AddMenuItem(new MenuItem("Go to Level2", "A", menuC.RunMenu));
            menuB.AddMenuItem(new MenuItem("Do something", "1", DefaultMenuAction));

            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Go to Level1", "A", menuB.RunMenu));
            menu.AddMenuItem(new MenuItem("New Game TicTacToe", "1", TicTacToe));
            menu.AddMenuItem(new MenuItem("New Game Human vs AI", "2", DefaultMenuAction));
            menu.AddMenuItem(new MenuItem("New Game AI vs AI", "3", DefaultMenuAction));

            menu.RunMenu();
        }

        private static string DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
            return "";
        }
        
        private static string TicTacToe()
        {
            var game = new TicTacToe();
            TicTacToeConsoleUi.DrawBoard(game.GetBoard());
            return "";
        }
        
    }
}