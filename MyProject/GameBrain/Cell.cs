namespace GameBrain
{
    public class Cell
    {
        public ECellState CellState { get; set; }
        public Coordinates Coordinates { get; set; }
        
        public Cell(int row, int column)
        {
            Coordinates = new Coordinates(row, column);
            CellState = ECellState.Empty;
        }
        
        public bool IsOccupied => CellState == ECellState.Boat;
    }
}