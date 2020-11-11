using System;

namespace AnotherDomain
{
    public class Game
    {

        public int GameId { get; set; }

        public bool NextMoveByPlayerA { get; set; }

        public int PlayerAId { get; set; }
        public Player PlayerA { get; set; } = null!;

        public int PlayerBId { get; set; }
        public Player PlayerB { get; set; } = null!;
    }
}