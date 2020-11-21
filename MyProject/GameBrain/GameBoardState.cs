namespace GameBrain
{
    public class GameBoardState
    {
        public ECellState[][] PlayerBoard { get; set; } = null!;
        public ECellState[][] OpponentBoard { get; set; } = null!;

    }
}