namespace AnotherDomain
{
    public class CellState
    {
        // public int? ShipId { get; set; }
        //
        // public int? Empty { get; set; }
        //
        // public int Bomb { get; set; }
        //
        // public int? Hit { get; set; }
        //
        // public int? Boat { get; set; }
        //
        // public int? Patrol { get; set; }
        //
        // public int? Cruiser { get; set; }
        //
        // public int? Submarine { get; set; }
        //
        // public int? Battleship { get; set; }
        //
        // public int? Carrier { get; set; }

        // this is a value from GameBoat.GameBoatId
        public int? ShipId { get; set; }

        // 0 - no bomb yet here, 1..X - bomb placements in numbered order
        public int Bomb { get; set; }
    }
}