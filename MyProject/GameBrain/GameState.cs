namespace GameBrain
{
    public class GameState
    {
        public bool NextMoveByPlayerA { get; set; }
        
        public CellState[][] BoardA { get; set; } = null!;
        
        public CellState[][] BoardB { get; set; } = null!;

        public int Size { get; set; }

    }
}