using System.Collections.Generic;

namespace Tetriminos.MainFunctions
{
    public abstract class Blocks
    {
        protected abstract Position[][] _tiles { get; }
        protected abstract Position _startOffset { get; }
        public abstract int _id { get; }
        private int _rotationState;
        private Position _offset;
        public Blocks()
        {
            _offset = new Position(_startOffset._row, _startOffset._column);
        }
        /// <summary>
        /// Iterates through all the possible positions of a player object
        /// </summary>
        /// <returns>Value of a given position in terms of Position array</returns>
        public IEnumerable<Position> TilePositions()
        {
            foreach (Position position in _tiles[_rotationState])
                yield return new Position(position._row + _offset._row, position._column + _offset._column);
        }
        /// <summary>
        /// Rotates the player object 90° to the right.
        /// </summary>
        public void RotateRight()
        {
            _rotationState = (_rotationState + 1) % _tiles.Length;
        }
        /// <summary>
        /// Rotates the plater object 90° to the left
        /// </summary>
        public void RotateLeft()
        {
            if (_rotationState == 0)
                _rotationState = _tiles.Length - 1;
            else
                _rotationState--;
        }
        /// <summary>
        /// Moves the player object one space Left, Right or Down.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public void Move(int rows, int columns)
        {
            _offset._row += rows;
            _offset._column += columns;
        }
        /// <summary>
        /// Resets the player object's position to its origin.
        /// </summary>
        public void Reset()
        {
            _rotationState = 0;
            _offset._row = _startOffset._row;
            _offset._column = _startOffset._column;
        }
    }
}
