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
        private Player PlayerA { get; set; }
        private Player PlayerB { get; set; }
        private bool NextMoveByPlayerA { get; set; }

        // private List<Ship> Ships { get; set; } = new ();
        // // {
        // //     new Ship(1),
        // //     new Ship(2),
        // //     new Ship(3),
        // //     new Ship(4),
        // //     new Ship(5)
        // // };

        public GameOptions()
        {
            BoardWidth = 10;
            BoardHeight = 10;
            
            ShipsCanTouch = EShipsCanTouch.No;
            NextMoveAfterHit = ENextMoveAfterHit.OtherPlayer;
            
            PlayerA = new Player("Player 1")
            {
                GameBoard = new GameBoard(BoardWidth, BoardHeight),
                Ships = new List<Ship>
                {
                    new Ship(1),
                    new Ship(2),
                    new Ship(3),
                    new Ship(4),
                    new Ship(5)
                }
            };
            PlayerB = new Player("Player 2")
            {
                GameBoard = new GameBoard(BoardWidth, BoardHeight),
                Ships = new List<Ship>
                {
                    new Ship(1),
                    new Ship(2),
                    new Ship(3),
                    new Ship(4),
                    new Ship(5)
                }
            };
            
            NextMoveByPlayerA = true;
        }

        public GameOptions(int boardWidth, int boardHeight, Player playerA, Player playerB,
            EShipsCanTouch shipsCanTouch, /*List<Ship> ships,*/ ENextMoveAfterHit nextMoveAfterHit)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            PlayerA = playerA;
            PlayerB = playerB;
            ShipsCanTouch = shipsCanTouch;
            // Ships = ships;
            NextMoveAfterHit = nextMoveAfterHit;
            NextMoveByPlayerA = true;
        }

        // public List<Ship> GetShips()
        // {
        //     return Ships;
        // }

        public Player GetPlayerA()
        {
            return PlayerA;
        }

        public Player GetPlayerB()
        {
            return PlayerB;
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
    }
}