using System.Collections.Generic;
using Domain.Enums;

namespace GameBrain
{
    public class GameState
    {
        public int BoardWidthState { get; set; }

        public int BoardHeightState { get; set; }

        public Player PlayerAState { get; set; } = null!;

        public Player PlayerBState { get; set; } = null!;

        public bool NextMoveByPlayerAState { get; set; }

        public List<Ship> ShipsState { get; set; } = null!;

        public EShipsCanTouch ShipsCanTouchState { get; set; }

        public GameOptions GameOptions { get; set; } = null!;

        public ECellState[][] PlayerABoardState { get; set; } = null!;

        public ECellState[][] PlayerBBoardState { get; set; } = null!;
    }
}