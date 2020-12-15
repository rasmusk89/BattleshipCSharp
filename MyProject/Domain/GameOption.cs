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

        [Range(2, 60)]
        [Display(Name="Board Width")]
        public int BoardWidth { get; set; }

        [Range(2, 60)]
        [Display(Name="Board Height")]
        public int BoardHeight { get; set; }
        
        public int NumberOfShips { get; set; }

        [Display(Name="Ships Can Touch")]
        public EShipsCanTouch EShipsCanTouch { get; set; }

        [Display(Name="Next Move After Hit")]
        public ENextMoveAfterHit NextMoveAfterHit { get; set; }

    }
}