using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Domain.Enums;

namespace Domain
{
    public class GameOption
    {
        public int GameOptionId { get; set; }

        [MaxLength(128)] public string Name { get; set; } = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        public int BoardWidth { get; set; }

        public int BoardHeight { get; set; }
        
        public int NumberOfShips { get; set; }

        public EShipsCanTouch EShipsCanTouch { get; set; }

        public ENextMoveAfterHit NextMoveAfterHit { get; set; }

    }
}