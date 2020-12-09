using System;
using System.Collections.Generic;
using Domain.Enums;
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
            
            playerType.AddMenuItem(new MenuItem(
                "Player vs AI",
                "2",
                BattleshipPlayerAI));
            
            playerType.AddMenuItem(new MenuItem(
                "AI vs AI",
                "3",
                BattleshipAIAI));
            
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

            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Battleship", "B", gameType.RunMenu));
            menu.AddMenuItem(new MenuItem("QUICK TEST GAME", "D", DefaultMenuAction));
            menu.RunMenu();
        }

        private static string Battleship()
        {
            var options = new GameOptions();
            var game = new Game(options);
            game.StartGame();
            return "";
        }
        
        private static string BattleshipPlayerAI()
        {
            var options = new GameOptions();
            var game = new Game(options) {PlayerB = {PlayerType = EPlayerType.Ai}};
            game.StartGame();
            return "";
        }

        private static string BattleshipAIAI()
        {
            var options = new GameOptions();
            var game = new Game(options)
            {
                PlayerA = {PlayerType = EPlayerType.Ai},
                PlayerB = {PlayerType = EPlayerType.Ai}
            };
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
            gameLoading.LoadLastGame().PlayRound();
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