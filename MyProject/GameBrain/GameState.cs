namespace GameBrain
{
    public class GameState
    {
        public bool NextMoveByPlayerA { get; set; }
        
        public ECellState[][] BoardA { get; set; } = null!;
        
        public ECellState[][] BoardB { get; set; } = null!;

        public int Width { get; set; }

        public int Height { get; set; }

    }
}