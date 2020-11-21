using System;
using System.Collections.Generic;

namespace GameBrain
{
    public class GameOptions
    {
        public static int BoardWidth { get; set; }
        public static int BoardHeight { get; set; }
        public EShipsCanTouch ShipsCanTouch { get; set; }
        public static bool NextMoveAfterHit { get; set; }
        public Player PlayerA { get; set; }
        public Player PlayerB { get; set; }
        public static List<Ship> Ships { get; set; } = new List<Ship>
        {
            new Ship(1),
            new Ship(2),
            new Ship(3),
            new Ship(4),
            new Ship(5)
        };

        public GameOptions()
        {
            BoardWidth = 10;
            BoardHeight = 10;
            ShipsCanTouch = EShipsCanTouch.Yes;
            NextMoveAfterHit = false;
            PlayerA = new Player("PlayerA", BoardWidth, BoardHeight, Ships, ShipsCanTouch);
            PlayerB = new Player("PlayerB", BoardWidth, BoardHeight, Ships,  ShipsCanTouch);
        }

        public GameOptions(int boardWidth, int boardHeight, string playerAName, string playerBName, EShipsCanTouch shipsCanTouch, List<Ship> ships, bool nextMoveAfterHit)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            PlayerA = new Player(playerAName, boardWidth, boardHeight, ships, shipsCanTouch);
            PlayerB = new Player(playerBName, boardWidth, boardHeight, ships, shipsCanTouch);
            ShipsCanTouch = shipsCanTouch;
            Ships = ships;
            NextMoveAfterHit = nextMoveAfterHit;
        }

        public int GetBoardWidth()
        {
            return BoardWidth;
        }

        public void SetBoardWidth(int width)
        {
            BoardWidth = width;
        }

        public int GetBoardHeight()
        {
            return BoardHeight;
        }

        public void SetBoardHeight(int height)
        {
            BoardHeight = height;
        }

        public EShipsCanTouch GetShipsCanTouch()
        {
            return ShipsCanTouch;
        }

        public void SetShipsCanTouch(EShipsCanTouch canTouch)
        {
            ShipsCanTouch = canTouch;
        }

        public Player GetPlayerA()
        {
            return PlayerA;
        }

        public void SetPlayerAName(string name)
        {
            PlayerA.SetName(name);
        }
        
        public Player GetPlayerB()
        {
            return PlayerB;
        }
        
        public void SetPlayerBName(string name)
        {
            PlayerB.SetName(name);
        }

        public void SetShips(List<Ship> ships)
        {
            Ships = ships;
        }

    }
}