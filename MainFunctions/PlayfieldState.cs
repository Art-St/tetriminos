using Plugin.SimpleAudioPlayer;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Media;
using System.Reflection;

namespace Tetriminos.MainFunctions
{
    public class PlayfieldState
    {
        public Blocks _currentBlock;
        public Blocks CurrentBlock
        {
            get => _currentBlock;
            private set
            {
                _currentBlock = value;
                CurrentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    CurrentBlock.Move(1, 0);
                    if (!BlockFits())
                        CurrentBlock.Move(-1, 0);
                }
            }
        }
        public Playfield Playfield { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public int RowsCleared { get; private set; }
        public bool Combo { get; private set; }
        public Blocks HeldBlock { get; private set; }
        public bool CanHold { get; private set; }
        public PlayfieldState()
        {
            Playfield = new Playfield(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.RerollBlock();
            CanHold = true;
            Combo = false;
        }
        private bool BlockFits()
        {
            foreach (Position position in CurrentBlock.TilePositions())
            {
                if (!Playfield.IsEmpty(position._row, position._column))
                    return false;
            }
            return true;
        }
        public void HoldBlock()
        {
            if (!CanHold)
                return;
            if(HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.RerollBlock();
            }
            else
            {
                (HeldBlock, CurrentBlock) = (CurrentBlock, HeldBlock);
            }
            CanHold = false;
        }
        public void RotateBlockRight()
        {
            CurrentBlock.RotateRight();
            if (!BlockFits())
            {
                CurrentBlock.RotateLeft();
                Play(Constants.SoundEffects.BLOCKED_MOVEMENT);
            }
            else
                Play(Constants.SoundEffects.ROTATE_BLOCK);
        }
        public void RotateBlockLeft()
        {
            CurrentBlock.RotateLeft();
            if (!BlockFits()) 
            { 
                CurrentBlock.RotateRight();
                Play(Constants.SoundEffects.BLOCKED_MOVEMENT); 
            }
            else
                Play(Constants.SoundEffects.ROTATE_BLOCK);
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
                Play(Constants.SoundEffects.BLOCKED_MOVEMENT);
            }
            else
                Play(Constants.SoundEffects.MOVE_BLOCK);
        }
        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
                Play(Constants.SoundEffects.BLOCKED_MOVEMENT);
            }
            else
                Play(Constants.SoundEffects.MOVE_BLOCK);
        }
        private bool IsGameOver()
        {
            return !(Playfield.IsEmptyRow(0) && Playfield.IsEmptyRow(1));
        }
        private void PlaceBlock()
        {
            foreach (Position position in CurrentBlock.TilePositions())
                Playfield[position._row, position._column] = CurrentBlock._id;
            Score += SetScore(Playfield.ClearFullRows(), Combo, out bool comboSetup);
            if (comboSetup)
                Combo = comboSetup;
            if (IsGameOver())
                GameOver = true;
            else
            {
                CurrentBlock = BlockQueue.RerollBlock();
                CanHold = true;
            }
        }
        /// <summary>
        /// Sets the current value for the total score of the playthrough.
        /// </summary>
        /// <param name="rowsToClear"></param>
        /// <param name="combo"></param>
        /// <param name="comboSetup"></param>
        /// <returns>Value of Score to add.</returns>
        private int SetScore(int rowsToClear, bool combo, out bool comboSetup)
        {
            comboSetup = false;
            RowsCleared += rowsToClear;
            if (rowsToClear == 4)
                comboSetup = true;
            return rowsToClear switch
            {
                1 or 2 or 3 => rowsToClear * 100,
                4 => combo ? (rowsToClear * 600) : rowsToClear * 400,
                _ => 0,
            };
        }
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            Play(Constants.SoundEffects.MOVE_BLOCK);
            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
        private int TileDropDistance(Position position)
        {
            int drop = 0;

            while (Playfield.IsEmpty(position._row + drop + 1, position._column))
            {
                drop++;
            }

            return drop;
        }
        public int BlockDropDistance()
        {
            int drop = Playfield._rows;

            foreach (Position position in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(position));
            }
            return drop;
        }
        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
        public static void Play(string audioFile)
        {
            ISimpleAudioPlayer player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            try
            {
                string directory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                player.Load(GetStreamFromFile(audioFile));
                player.Play();
            }
            catch (Exception ex)
            {
                // Do nothing.
            }
        }
        private static Stream GetStreamFromFile(string fileName)
        {
            Uri resource = new(fileName, UriKind.Relative);
            Stream stream = System.Windows.Application.GetResourceStream(resource);
            return stream;
        }
    }
}
