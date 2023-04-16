namespace Tetriminos
{
    public class Playfield
    {
        private readonly int[,] grid;
        public int _rows { get; }
        public int _columns { get; }
        public int this[int row, int column]
        {
            get => grid[row, column];
            set => grid[row, column] = value;
        }
        public Playfield(int rows, int columns)
        {
            grid = new int[rows, columns];
            _rows = rows;
            _columns = columns;
        }
        /// <summary>
        /// Checks if the player object is within bounds of the Playfield
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>True, if the player is inbounds. False, if the player is out of bounds</returns>
        public bool IsInside(int row, int column)
        {
            return row >= 0 && row < _rows && column >= 0 && column < _columns;
        }
        /// <summary>
        /// Checks if a given Cell is empty
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>True, if the cell is empty, otherwise, False</returns>
        public bool IsEmpty(int row, int column)
        {
            return IsInside(row, column) && grid[row, column] == 0;
        }
        /// <summary>
        /// Checks if a row is full of blocks.
        /// </summary>
        /// <param name="row"></param>
        /// <returns>True, if the row is full of blocks, otherwise, False</returns>
        public bool IsFullRow(int row)
        {
            for (int column = 0; column < _columns; column++)
            {
                if (grid[row, column] == 0)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Checks if a row is empty.
        /// </summary>
        /// <param name="row"></param>
        /// <returns>True, if a row has no blocks, otherwise, False</returns>
        public bool IsEmptyRow(int row)
        {
            for (int column = 0; column < _columns; column++)
            {
                if (grid[row, column] != 0)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Clears a specific row that is full of blocks.
        /// </summary>
        /// <param name="row"></param>
        private void ClearRow(int row)
        {
            for (int column = 0; column < _columns; column++)
            {
                grid[row, column] = 0;
            }
        }
        /// <summary>
        /// Collapses the rows of the playfield by the number of rows cleared.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="numberOfRows"></param>
        private void CollapseRow(int row, int numberOfRows)
        {
            for (int column = 0; column < _columns; column++)
            {
                grid[row + numberOfRows, column] = grid[row, column];
                grid[row, column] = 0;
            }
        }
        /// <summary>
        /// Clears rows that are completely full of blocks
        /// </summary>
        /// <returns>Number of Rows to Clear</returns>
        public int ClearFullRows()
        {
            int clearedRows = 0;
            for (int row = _rows - 1; row >= 0; row--)
            {
                if (IsFullRow(row))
                {
                    ClearRow(row);
                    clearedRows++;
                }
                else if (clearedRows > 0)
                    CollapseRow(row, clearedRows);
            }
            return clearedRows;
        }
    }
}
