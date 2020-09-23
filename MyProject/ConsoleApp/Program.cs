using System;
using MenuSystem;

namespace ConsoleApp
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=====> RASKIL GAME <=====");
            var menuB = new Menu(MenuLevel.Level2Plus);
            menuB.MenuItems.Add(new MenuItem("Sub 2.", "1", DefaultMenuAction));
            
            var menuA = new Menu(MenuLevel.Level1);
            menuA.MenuItems.Add(new MenuItem("Go to submenu 2", "1", menuB.RunMenu));

            var menu = new Menu(MenuLevel.Level0);
            menu.MenuItems.Add(new MenuItem("Go to submenu 1", "s", menuA.RunMenu));
            menu.MenuItems.Add(new MenuItem("New game human vs human", "1", DefaultMenuAction));
            menu.MenuItems.Add(new MenuItem("New game human vs AI", "2", DefaultMenuAction));
            menu.MenuItems.Add(new MenuItem("New game AI vs AI", "3", DefaultMenuAction));;
            menu.RunMenu();
        }
        
        public static void DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
        }
    }
}