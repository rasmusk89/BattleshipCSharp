using System;
using System.Collections.Generic;

namespace MenuSystem
{
    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; }

        public Menu()
        {
            MenuItems = new List<MenuItem>();
            
            MenuItems.Add(new MenuItem("New game human vs human", "1"));
            MenuItems.Add(new MenuItem("New game human vs AI", "2"));
            MenuItems.Add(new MenuItem("New game AI vs AI", "3"));
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
                
                Console.WriteLine("L) Load game");
                Console.WriteLine("X) Exit");
                Console.Write(">");

                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";

                switch (userChoice)
                {
                    case "1":
                        Console.WriteLine("Not implemented yet..");
                        break;
                    case "2":
                        Console.WriteLine("Not implemented yet..");
                        break;
                    case "3":
                        Console.WriteLine("Not implemented yet..");
                        break;
                    case "l":
                        Console.WriteLine("Not implemented yet..");
                        break;
                    case "x":
                        Console.WriteLine("Closing...");
                        break;
                    default:
                        Console.WriteLine("No such option.");
                        break;
                }
            } while (userChoice != "x");
        }
    }
}