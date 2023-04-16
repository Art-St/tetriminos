namespace Tetriminos.MainFunctions
{
    public class PlayfieldState
    {
        public Blocks _currentBlock;
        public Blocks currentBlock
        {
            get => _currentBlock;
            private set
            {
                _currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);
                    if (!BlockFits())
                        currentBlock.Move(-1, 0);
                }
            }
        }
        public Playfield _playfield { get; }
        public BlockQueue _blockQueue { get; }
        public bool _gameOver { get; private set; }
        public int _score { get; private set; }
        public Blocks _heldBlock { get; private set; }
        public bool _canHold { get; private set; }
        public PlayfieldState()
        {
            _playfield = new Playfield(22, 10);
            _blockQueue = new BlockQueue();
            currentBlock = _blockQueue.RerollBlock();
            _canHold = true;
        }
        private bool BlockFits()
        {
            foreach (Position position in currentBlock.TilePositions())
            {
                if (!_playfield.IsEmpty(position._row, position._column))
                    return false;
            }
            return true;
        }
        public void HoldBlock()
        {
            if (!_canHold)
                return;
            if(_heldBlock == null)
            {
                _heldBlock = currentBlock;
                currentBlock = _blockQueue.RerollBlock();
            }
            else
            {
                Blocks temporaryBlock = currentBlock;
                currentBlock = _heldBlock;
                _heldBlock = temporaryBlock;
            }
            _canHold = false;
        }
        public void RotateBlockRight()
        {
            currentBlock.RotateRight();
            if (BlockFits())
                currentBlock.RotateLeft();

        }
        public void RotateBlockLeft()
        {
            currentBlock.RotateLeft();
            if (!BlockFits())
                currentBlock.RotateRight();
        }
        public void MoveBlockRight()
        {
            currentBlock.Move(0, 1);
            if (!BlockFits())
                currentBlock.Move(0, -1);
        }
        public void MoveBlockLeft()
        {
            currentBlock.Move(0, -1);
            if (!BlockFits())
                currentBlock.Move(0, 1);
        }
        private bool IsGameOver()
        {
            return !(_playfield.IsEmptyRow(0) && _playfield.IsEmptyRow(1));
        }
        private void PlaceBlock()
        {
            foreach (Position position in currentBlock.TilePositions())
                _playfield[position._row, position._column] = currentBlock._id;
            _score += _playfield.ClearFullRows();
            if (IsGameOver())
                _gameOver = true;
            else
            {
                currentBlock = _blockQueue.RerollBlock();
                _canHold = true;
            }
        }
        public void MoveBlockDown()
        {
            currentBlock.Move(1, 0);
            if (!BlockFits())
            {
                currentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
        private int TileDropDistance(Position position)
        {
            int drop = 0;

            while (_playfield.IsEmpty(position._row + drop + 1, position._column))
            {
                drop++;
            }

            return drop;
        }
        public int BlockDropDistance()
        {
            int drop = _playfield._rows;

            foreach (Position position in currentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(position));
            }
            return drop;
        }
        public void DropBlock()
        {
            currentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }
}
