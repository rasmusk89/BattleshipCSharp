using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Ship
    {
        public int ShipId { get; set; }

        [MaxLength(128)]
        public string Name { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Width { get; set; }

    }
}