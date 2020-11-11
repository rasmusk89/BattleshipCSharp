﻿using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameShip
    {
        public int GameShipId { get; set; }

        [Range(1, int.MaxValue)]
        public int Size { get; set; }

        [MaxLength(128)] public string Name { get; set; } = null!;

        public bool IsSunk { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;
    }
}