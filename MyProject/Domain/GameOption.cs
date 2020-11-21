using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain
{
    public class GameOption
    {
        public int GameOptionId { get; set; }

        [MaxLength(128)]
        public string Name { get; set; } = null!;

        public int BoardWidth { get; set; }

        public int BoardHeight { get; set; }

        public EShipsCanTouch EShipsCanTouch { get; set; }

        public ENextMoveAfterHit NextMoveAfterHit { get; set; }

        public ICollection<GameOptionShip> GameOptionShips { get; set; } = null!;

        public ICollection<Game> Games { get; set; } = null!;

    }
}