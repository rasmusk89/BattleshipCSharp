using System;
using MenuSystem;

namespace ConsoleApp
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=====> RASKIL GAME <=====");
            
            var menu = new Menu();
            menu.RunMenu();
        }
    }
}