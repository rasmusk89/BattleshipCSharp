namespace GameBrain
{
    public class GameState
    {
        public bool NextMoveByPlayerA { get; set; }
        
        public CellState[][] BoardA { get; set; } = null!;
        
        public CellState[][] BoardB { get; set; } = null!;

        public int Width { get; set; }

        public int Height { get; set; }

    }
}