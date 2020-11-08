namespace GameBrain
{
    public class Coordinates
    {
        public int Row { get; set; }
        public int Column { get; set; }
        
        public Coordinates(int column, int row)
        {
            Column = column;
            Row = row;
        }
        
    }
}