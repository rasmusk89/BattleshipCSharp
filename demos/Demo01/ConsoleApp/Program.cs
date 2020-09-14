using System;
using MenuSystem;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("======> TIC-TAC-TOE RASKIL <======");
            var menu = new Menu();
            menu.RunMenu();
        }
    }
}