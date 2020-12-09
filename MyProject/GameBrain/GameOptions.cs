using System.Collections.Generic;
using Domain.Enums;

namespace GameBrain
{
    public class GameOptions
    {
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public EShipsCanTouch ShipsCanTouch { get; set; }
        public ENextMoveAfterHit NextMoveAfterHit { get; set; }

        public List<Ship> Ships { get; set; } = new()
        {
            new Ship(1),
            // new Ship(2),
            // new Ship(3),
            // new Ship(4),
            // new Ship(5)
        };

        public GameOptions()
        {
            BoardWidth = 10;
            BoardHeight = 10;
            ShipsCanTouch = EShipsCanTouch.No;
            NextMoveAfterHit = ENextMoveAfterHit.SamePlayer;
        }

        public GameOptions(int boardWidth, int boardHeight, EShipsCanTouch shipsCanTouch,
            ENextMoveAfterHit nextMoveAfterHit, List<Ship> ships)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            ShipsCanTouch = shipsCanTouch;
            NextMoveAfterHit = nextMoveAfterHit;
            Ships = ships;
        }

        public List<Ship> GetShips()
        {
            return Ships;
        }

        public int GetBoardHeight()
        {
            return BoardHeight;
        }

        public int GetBoardWidth()
        {
            return BoardWidth;
        }

        public EShipsCanTouch GetShipsCanTouch()
        {
            return ShipsCanTouch;
        }

        public ENextMoveAfterHit GetNextMoveAfterHit()
        {
            return NextMoveAfterHit;
        }
    }
}