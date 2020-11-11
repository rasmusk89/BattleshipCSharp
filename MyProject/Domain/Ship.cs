namespace AnotherDomain
{
    public class Ship
    {
        public int ShipId { get; set; }

        public string? Name { get; set; }

        public int Width { get; set; }

        public int Hits { get; set; }

        public bool IsSunk { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

    }
}