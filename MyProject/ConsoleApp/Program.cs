﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DAL;
using MenuSystem;
using GameBrain;
using GameConsoleUI;
using Player = Domain.Player;

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


            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Battleship", "B", gameType.RunMenu));
            menu.AddMenuItem(new MenuItem("DB Test", "D", DbTest));
            menu.RunMenu();
        }

        private static string Battleship()
        {
            var game = new Game();
            game.PlayRound();
            return "";
        }

        private static string CustomBattleship()
        {
            var game = new Game();
            game.CustomRound();
            return "";
        }

        private static string LoadGame()
        {
            var game = new Game();
            game.LoadGameAction();
            return "";
        }

        private static string DefaultMenuAction()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nNot implemented yet!");
            Console.ForegroundColor = ConsoleColor.Cyan;

            return "";
        }

        private static string DbTest()
        {
            using var db = new ApplicationDbContext();
            foreach (var player in db.Players!)
            {
                Console.WriteLine(player);
            }

            return "";
        }
    }
}