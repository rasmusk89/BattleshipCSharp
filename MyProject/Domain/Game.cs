using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }

        [MaxLength(512)]
        public string Description { get; set; } =
            DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString();
        
        public int GameOptionId { get; set; }
        public GameOption GameOption { get; set; } = null!;

        public int PlayerAId { get; set; }
        public Player PlayerA { get; set; } = null!;

        public int PlayerBId { get; set; }
        public Player PlayerB { get; set; } = null!;
        
    }
}