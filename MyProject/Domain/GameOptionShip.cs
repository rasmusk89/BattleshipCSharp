﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameOptionShip
    {
        public int GameOptionShipId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }

        public int ShipId { get; set; }
        public Ship Ship { get; set; }  = null!;

        public int GameOptionId { get; set; }
        public GameOption GameOption { get; set; } = null!;

        // public ICollection<Game>? Games { get; set; }
        
    }
}