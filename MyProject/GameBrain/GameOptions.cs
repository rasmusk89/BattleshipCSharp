using System;

namespace GameBrain
{
    public static class GameOptions
    {
        public static string GetAName()
        {
            Console.WriteLine("Player A Name: ");
            return Console.ReadLine() ?? "PlayerA";
        }
        
        public static string GetBName()
        {
            Console.WriteLine("Player B Name: ");
            return Console.ReadLine() ?? "PlayerB";
        }
        
        public static int GetBoardWidth()
        {
            Console.WriteLine("Board width: ");
            return int.Parse(Console.ReadLine() ?? "10");
        }
        
        public static int GetBoardHeight()
        {
            Console.WriteLine("Board height: ");
            return int.Parse(Console.ReadLine() ?? "10");
        }
        
        
    }
}