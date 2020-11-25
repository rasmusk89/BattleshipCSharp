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

        private List<Ship> Ships { get; set; } = new()
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
            NextMoveAfterHit = ENextMoveAfterHit.OtherPlayer;
            PlayerA = new Player("Player 1")
            {
                PlayerBoard = new GameBoard(BoardWidth, BoardHeight),
                FiringBoard = new GameBoard(BoardWidth, BoardHeight)
            };
            PlayerB = new Player("Player 2")
            {
                PlayerBoard = new GameBoard(BoardWidth, BoardHeight),
                FiringBoard = new GameBoard(BoardWidth, BoardHeight)
            };
            NextMoveByPlayerA = true;
        }

        public GameOptions(int boardWidth, int boardHeight, Player playerA, Player playerB,
            EShipsCanTouch shipsCanTouch, List<Ship> ships, ENextMoveAfterHit nextMoveAfterHit)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            PlayerA = playerA;
            PlayerB = playerB;
            ShipsCanTouch = shipsCanTouch;
            Ships = ships;
            NextMoveAfterHit = nextMoveAfterHit;
            NextMoveByPlayerA = true;
        }

        public List<Ship> GetShips()
        {
            return Ships;
        }

        public Player GetPlayerA()
        {
            return PlayerA;
        }

        public Player GetPlayerB()
        {
            return PlayerB;
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

        // public Player GetPlayerA()
        // {
        //     return PlayerA;
        // }

        public void SetPlayerAName(string name)
        {
            PlayerA.SetName(name);
        }

        // public Player GetPlayerB()
        // {
        //     return PlayerB;
        // }

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