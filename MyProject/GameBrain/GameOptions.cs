using System.Collections.Generic;
using Domain.Enums;

namespace GameBrain
{
    public class GameOptions
    {
        private int BoardWidth { get; set; }
        private int BoardHeight { get; set; }
        private EShipsCanTouch ShipsCanTouch { get; set; }
        private ENextMoveAfterHit NextMoveAfterHit { get; set; }
        private bool NextMoveByPlayerA { get; set; }
        private List<Ship> Ships { get; set; } = new List<Ship>
        {
            new(1),
            new(2),
            new(3),
            new(4),
            new(5)
        };

        public GameOptions()
        {
            BoardWidth = 10;
            BoardHeight = 10;
            ShipsCanTouch = EShipsCanTouch.No;
            NextMoveAfterHit = ENextMoveAfterHit.OtherPlayer;
            NextMoveByPlayerA = true;
        }

        public GameOptions(int boardWidth, int boardHeight, EShipsCanTouch shipsCanTouch, ENextMoveAfterHit nextMoveAfterHit, List<Ship> ships)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            ShipsCanTouch = shipsCanTouch;
            NextMoveAfterHit = nextMoveAfterHit;
            NextMoveByPlayerA = true;
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

        public bool GetNextMoveByPlayerA()
        {
            return NextMoveByPlayerA;
        }
    }
}