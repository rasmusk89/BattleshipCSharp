namespace GameBrain
{
    public class Cell
    {
        private ECellState CellState { get; set; }
        private Coordinates Coordinates { get; set; }
        
        public Cell(int row, int column)
        {
            Coordinates = new Coordinates(row, column);
            CellState = ECellState.Empty;
        }

        public string State => CellState.ToString();

        public bool IsOccupied => CellState == ECellState.Boat;
    }
}