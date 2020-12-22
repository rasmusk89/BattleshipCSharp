using System;
using System.Linq;
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
                BattleshipPlayerAi));

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
                    var allGamesMenu = new Menu(MenuLevel.Level2Plus);
                    foreach (var (id, desc) in GameLoading.GetListOfAllGames())
                    {
                        allGamesMenu.AddMenuItem(new MenuItem(
                            desc,
                            id.ToString(),
                            () => LoadGameById(id)
                        ));
                    }

                    if (!GameLoading.GetListOfAllGames().Any())
                    {
                        Console.Write("No saved games!");
                    }

                    return allGamesMenu.RunMenu();
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

        private static string BattleshipPlayerAi()
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
            GameLoading.LoadLastGame().PlayRound();
            return "";
        }

        private static string LoadGameById(int id)
        {
            GameLoading.LoadGameById(id).PlayRound();
            return "";
        }
    }
}