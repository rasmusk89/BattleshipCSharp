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
            
            var inBattleshipGame = new Menu(MenuLevel.Level1);
            inBattleshipGame.AddMenuItem(new MenuItem(
                "Start Game",
                "S",
                Battleship
                ));
            
            inBattleshipGame.AddMenuItem(new MenuItem(
                "Options",
                "O",
                Options
                ));
            
            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Battleship", "B", inBattleshipGame.RunMenu));

            menu.RunMenu();
        }
        
        private static string Options()
        {
            Console.WriteLine("Set game board width: ");
            var width = int.Parse(Console.ReadLine() ?? "10");
            
            Console.WriteLine("Set game board height: ");
            var height = int.Parse(Console.ReadLine() ?? "10");
            
            var game = new Game(width, height);
            game.PlayRound();
            return "";
        }
        
        
        private static string Battleship()
        {
            var game = new Game();
            game.PlayRound();
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