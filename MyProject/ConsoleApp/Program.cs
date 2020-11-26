using System;
using GameBrain;
using MenuSystem;

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

            var playerType = new Menu(MenuLevel.Level2Plus);
            playerType.AddMenuItem(new MenuItem(
                "Player VS Player",
                "1",
                Battleship));

            var gameType = new Menu(MenuLevel.Level1);
            gameType.AddMenuItem(new MenuItem(
                "Start Basic Game",
                "S",
                playerType.RunMenu
            ));

            gameType.AddMenuItem(new MenuItem(
                "Start Custom Game",
                "C",
                CustomBattleship
            ));

            gameType.AddMenuItem(new MenuItem(
                "Load Last Game",
                "L",
                LoadGame
            ));
            
            // gameType.AddMenuItem(new MenuItem(
            //     "Load Game from DB",
            //     "D",
            //     LoadGameDb
            // ));


            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Battleship", "B", gameType.RunMenu));
            menu.AddMenuItem(new MenuItem("Default", "D", DefaultMenuAction));
            menu.RunMenu();
        }

        private static string Battleship()
        {
            var options = new GameOptions();
            var game = new Game(options);
            game.StartGame();
            return "";
        }

        private static string CustomBattleship()
        {
            var gameOptions = new CustomOptions();
            var options = gameOptions.GetCustomOptions();

            var game = new Game(options);
            game.StartGame();
            return "";
        }

        private static string LoadGame()
        {
            var gameLoading = new GameLoading();
            gameLoading.LoadLastGame().StartGame();
            return "";
        }

        

        // private static string LoadGameDb()
        // {
        //     var gameLoading = new GameLoading();
        //     var game = new Game(GameLoading.LoadLastGameOptions());
        //     game.PlayRound();
        //     return "";
        // }
        
        private static string DefaultMenuAction()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nNot implemented yet!");
            Console.ForegroundColor = ConsoleColor.Cyan;

            return "";
        }
    }
}