using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2Plus
    }

    public class Menu
    {
        private readonly MenuLevel _menuLevel;

        private readonly string[] _reservedActions = {"x", "m", "p"};

        private Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }

        public void AddMenuItem(MenuItem item)
        {
            if (item.UserChoice == "")
            {
                throw new ArgumentException($"UserChoice can not be empty.");
            }

            MenuItems.Add(item.UserChoice, item);
        }

        public string RunMenu()
        {
            var userChoice = "";
            Console.ForegroundColor = ConsoleColor.Cyan;

            do
            {
                Console.WriteLine("");

                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine("==> MAIN MENU <==");
                        Console.WriteLine("----------------------");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine("==> MENU 1 <==");
                        Console.WriteLine("----------------------");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine("==> SUB MENU <==");
                        Console.WriteLine("----------------------");
                        break;
                    default:
                        break;
                }

                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem.Value);
                }

                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine("X) eXit");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) eXit");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine("P) return to Previous");
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) eXit");
                        break;
                    default:
                        throw new Exception("Unknown menu depth!");
                }

                Console.Write(">");

                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";

                if (!_reservedActions.Contains(userChoice))
                {
                    if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                    {
                        userChoice = userMenuItem.MethodToExecute();
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("No such option.Please try again!");
                    }
                }

                if (userChoice == "x")
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.WriteLine("Closing...");
                    }

                    break;
                }

                if (userChoice == "x")
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.WriteLine("Closing...");
                    }

                    break;
                }

                if (_menuLevel != MenuLevel.Level0 && userChoice == "m")
                {
                    break;
                }

                if (_menuLevel != MenuLevel.Level2Plus || userChoice != "p") continue;
                userChoice = "";
                break;
            } while (true);


            return userChoice;
        }
    }
}