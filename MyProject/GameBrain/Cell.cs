namespace GameBrain
{
    public class Cell
    {
        public static ECellState CellState { get; set; }
        public Coordinates Coordinates { get; set; }
        
        public Cell(int row, int column)
        {
            Coordinates = new Coordinates(row, column);
            CellState = ECellState.Empty;
        }
        
        public bool IsOccupied => CellState == ECellState.Boat;
    }
}