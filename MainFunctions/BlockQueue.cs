using System;

namespace Tetriminos.MainFunctions
{
    public class BlockQueue
    {
        private readonly Blocks[] blocks = new Blocks[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };
        private readonly Random random = new Random();
        public Blocks _nextBlock { get; private set; }
        public BlockQueue()
        {
            _nextBlock = RandomBlock();
        }
        /// <summary>
        /// Calculates a random block from the blocks array.
        /// </summary>
        /// <returns>The ID of a random block within the array.</returns>
        private Blocks RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }
        /// <summary>
        /// If the block to be returned is identical to the previous one, calls the RandomBlock function until a new block is choosen.
        /// </summary>
        /// <returns>A new Block.</returns>
        public Blocks RerollBlock()
        {
            Blocks block = _nextBlock;
            do
            {
                _nextBlock = RandomBlock();
            }
            while (block._id == _nextBlock._id);
            return block;
        }
    }
}
