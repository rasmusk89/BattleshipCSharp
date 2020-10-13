using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public class Menu
    {
        private readonly MenuLevel _menuLevel;

        private readonly string[] _reservedActions = {"x", "m", "p"};

        private static string _inputWarning = "";
        
        


        private Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }

        public void AddMenuItem(MenuItem item)
        {
            if (item.UserChoice == "")
            {
                throw new ArgumentException("UserChoice can not be empty.");
            }
            if (_reservedActions.Contains(item.UserChoice))
            {
                throw new Exception($"Cannot add \"{item.UserChoice}\" as user choice!");
            }

            if (MenuItems.ContainsKey(item.UserChoice))
            {
                throw new Exception($"User choice \"{item.UserChoice}\" already in use!");
            }

            MenuItems.Add(item.UserChoice, item);
        }

        public string RunMenu()
        {
            string? userChoice;
            do
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(_inputWarning);
                Console.ForegroundColor = ConsoleColor.Cyan;
                _inputWarning = "";
                
                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem.Value);
                }

                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine($"{FixedMenuChoices.ExitUserChoice.ToUpper()}) {FixedMenuChoices.ExitLabel}");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine($"{FixedMenuChoices.MainUserChoice.ToUpper()}) {FixedMenuChoices.MainLabel}");
                        Console.WriteLine($"{FixedMenuChoices.ExitUserChoice.ToUpper()}) {FixedMenuChoices.ExitLabel}");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine($"{FixedMenuChoices.PreviousUserChoice.ToUpper()}) {FixedMenuChoices.PreviousLabel}");
                        Console.WriteLine($"{FixedMenuChoices.MainUserChoice.ToUpper()}) {FixedMenuChoices.MainLabel}");
                        Console.WriteLine($"{FixedMenuChoices.ExitUserChoice.ToUpper()}) {FixedMenuChoices.ExitLabel}");

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
                        userChoice = userMenuItem.MethodToExecute?.Invoke();
                    }
                    else
                    {
                        _inputWarning = "\nNo such option, please try again!";
                    }
                }

                if (userChoice == "x")
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Closing...");
                    }

                    userChoice = "x";
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