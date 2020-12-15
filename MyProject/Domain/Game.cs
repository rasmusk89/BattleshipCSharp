using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }

        [MaxLength(512)]
        public string Description { get; set; } =
            DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString();

        public int? GameOptionId { get; set; }
        public GameOption GameOption { get; set; } = default!;

        public int? PlayerAId { get; set; }
        public Player PlayerA { get; set; } = default!;

        public int? PlayerBId { get; set; }
        public Player PlayerB { get; set; } = default!;

        public ICollection<GameState>? GameStates { get; set; }
    }
}