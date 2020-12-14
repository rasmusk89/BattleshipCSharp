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
                              "|         MAIN MENU         |\n" +
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
                BattleshipAiAi));
            
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
            
            gameType.AddMenuItem(new MenuItem(
                "Load All Games",
                "O",
                () =>
                {
                    var allGames = new Menu(MenuLevel.Level2Plus);
                    var listOfAllGames = new GameLoading().GetListOfAllGames();
                    var label = 1;
                    foreach (var (id, desc) in listOfAllGames)
                    {
                        allGames.AddMenuItem(new MenuItem(
                            desc,
                            label.ToString(),
                            () => LoadGameById(id)
                        ));
                        label++;
                    }

                    allGames.RunMenu();
                    return "";
                }
            ));
            
            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Battleship", "B", gameType.RunMenu));
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

        private static string BattleshipAiAi()
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

        private static string LoadGameById(int id)
        {
            var gameLoading = new GameLoading();
            gameLoading.LoadGameById(id).PlayRound();
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