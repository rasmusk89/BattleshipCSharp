using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public enum MenuLevel
    {
        Level0, // 0
        Level1, // 1
        Level2Plus // 2
    }

    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private readonly MenuLevel _menuLevel;

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }

        public void RunMenu()
        {
            var userChoice = "";

            do
            {
                Console.WriteLine("");

                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem);
                }

                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine("X) Exit");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine("R) Return to main");
                        Console.WriteLine("X) Exit");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine("P) Return to previous");
                        Console.WriteLine("R) Return to main");
                        Console.WriteLine("X) Exit");
                        break;
                    default:
                        throw new Exception("Unknown menu depth!");
                }

                Console.Write(">");

                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";

                if (userChoice == "x")
                {
                    Console.WriteLine("Closing...");
                    break;
                }

                var userMenuItem = MenuItems.FirstOrDefault(t => t.UserChoice == userChoice);
                if (userMenuItem != null)
                {
                    userMenuItem.MethodToExecute();
                }
                else
                {
                    Console.WriteLine("No such option.");
                }
            } while (userChoice != "x");
        }
    }
}