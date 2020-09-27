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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=====> RASKIL GAME <=====");
            Console.ForegroundColor = ConsoleColor.Cyan;


            var menuE = new Menu(MenuLevel.Level2Plus);
            menuE.AddMenuItem(new MenuItem("Default", "1", DefaultMenuAction));
            menuE.AddMenuItem(new MenuItem("Go to Level5", "A", DefaultMenuAction));

            var menuD = new Menu(MenuLevel.Level2Plus);
            menuD.AddMenuItem(new MenuItem("Default", "1", DefaultMenuAction));
            menuD.AddMenuItem(new MenuItem("Go to Level4", "A", menuE.RunMenu));

            var menuC = new Menu(MenuLevel.Level2Plus);
            menuC.AddMenuItem(new MenuItem("Default", "1", DefaultMenuAction));
            menuC.AddMenuItem(new MenuItem("Go to Level3", "A", menuD.RunMenu));

            var menuB = new Menu(MenuLevel.Level1);
            menuB.AddMenuItem(new MenuItem("Default", "1", DefaultMenuAction));
            menuB.AddMenuItem(new MenuItem("Go to Level2", "A", menuC.RunMenu));

            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Go to Level1", "A", menuB.RunMenu));
            menu.AddMenuItem(new MenuItem("New Game TicTacToe", "1", TicTacToe));
            menu.AddMenuItem(new MenuItem("New Game Human vs AI", "2", DefaultMenuAction));
            menu.AddMenuItem(new MenuItem("New Game AI vs AI", "3", DefaultMenuAction));

            menu.RunMenu();
        }

        private static string DefaultMenuAction()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNot implemented yet!");
            Console.ForegroundColor = ConsoleColor.Cyan;

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