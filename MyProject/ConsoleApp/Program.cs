﻿using System;
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
            
            var menu = new Menu(MenuLevel.Level1);
            menu.AddMenuItem(new MenuItem(
                $"Start Battleship game!",
                userChoice: "1",
                () =>
                {
                    BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA ? GameBrain.Battleship.GetBoardA() : GameBrain.Battleship.GetBoardB());
                    var playGame = new Menu(MenuLevel.Level1);
                    playGame.AddMenuItem(new MenuItem(
                        $"Player {(game.NextMoveByPlayerA ? "A" : "B")} make a move",
                        userChoice: "1",
                        () =>
                        {
                            var (x, y) = GameBrain.Battleship.GetMoveCoordinates();
                            game.MakeAMove(x, y);
                            BattleshipConsoleUI.DrawBoard(game.NextMoveByPlayerA ? GameBrain.Battleship.GetBoardA() : GameBrain.Battleship.GetBoardB());
                            return "";
                        })
                    );
                    
                    playGame.RunMenu();
                    return "";
                })
            );

            var userChoice = menu.RunMenu();

            return userChoice;
        }
    }
}