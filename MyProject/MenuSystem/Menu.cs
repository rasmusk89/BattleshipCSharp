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
            Console.CursorVisible = false;
            var userChoice = "";
            Console.ForegroundColor = ConsoleColor.Cyan;

            do
            {
                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n==> MAIN MENU <==");
                        Console.WriteLine("----------------------");
                        Console.ForegroundColor = ConsoleColor.Cyan;

                        break;
                    case MenuLevel.Level1:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n==> MENU 1 <==");
                        Console.WriteLine("----------------------");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case MenuLevel.Level2Plus:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n==> SUB MENU <==");
                        Console.WriteLine("----------------------");
                        Console.ForegroundColor = ConsoleColor.Cyan;
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nNo such option, please try again!");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                }
                
                if (userChoice == "x")
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
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