namespace Tetriminos
{
    public class Position
    {
        public int _row { get; set; }
        public int _column { get; set; }
        public Position(int row, int column)
        {
            _row = row;
            _column = column;
        }
    }
}
