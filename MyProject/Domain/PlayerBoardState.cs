using System;

namespace Domain
{
    public class PlayerBoardState
    {
        public int PlayerBoardStateId { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        //Serialized to JSON
        public string BoardStatePlayer { get; set; } = null!;
    }
}