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

        private readonly string[] reservedActions = new[] {"x", "m", "p"};

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

        public string RunMenu() // Needs to be of type Func<string>
        {
            var userChoice = "";

            do
            {
                Console.WriteLine("");

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

                if (!reservedActions.Contains(userChoice))
                {
                    if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                    {
                        userChoice = userMenuItem.MethodToExecute();
                    }
                    else
                    {
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

                if (_menuLevel != MenuLevel.Level0 && userChoice == "m")
                {
                    break;
                }

                if (_menuLevel == MenuLevel.Level2Plus && userChoice == "p")
                {
                    break;
                }
            } while (true);

            return userChoice;
        }
    }
}