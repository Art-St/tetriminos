using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tetriminos.MainFunctions;

namespace Tetriminos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri(Constants.Tile.EMPTY_TILE, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.Tile.CYAN_TILE, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.Tile.BLUE_TILE, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.Tile.ORANGE_TILE, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.Tile.YELLOW_TILE, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.Tile.GREEN_TILE, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.Tile.PURPLE_TILE, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.Tile.RED_TILE, UriKind.Relative))
        };
        private readonly ImageSource[] previewImages = new ImageSource[]
        {
            new BitmapImage(new Uri(Constants.PreviewIcon.EMPTY_BLOCK, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.PreviewIcon.I_BLOCK, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.PreviewIcon.J_BLOCK, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.PreviewIcon.L_BLOCK, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.PreviewIcon.O_BLOCK, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.PreviewIcon.S_BLOCK, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.PreviewIcon.T_BLOCK, UriKind.Relative)),
            new BitmapImage(new Uri(Constants.PreviewIcon.Z_BLOCK, UriKind.Relative))
        };
        private readonly Image[,] imageControls;
        private const int maxDelay = 1000;
        private const int minDelay = 75;
        private const int delayDecrease = 25;
        private PlayfieldState playfieldState = new();
        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(playfieldState._playfield);
        }
        private Image[,] SetupGameCanvas(Playfield grid)
        {
            Image[,] imageControls = new Image[grid._rows, grid._columns];
            int cellSize = 25;
            for (int row = 0; row < grid._rows; row++)
            {
                for (int column = 0; column < grid._columns; column++)
                {
                    Image imageControl = new()
                    {
                        Width = cellSize,
                        Height = cellSize
                    };
                    Canvas.SetTop(imageControl, (row - 2) * cellSize);
                    Canvas.SetLeft(imageControl, column * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[row, column] = imageControl;
                }
            }
            return imageControls;
        }
        private void DrawGrid(Playfield grid)
        {
            for (int row = 0; row < grid._rows; row++)
            {
                for (int column = 0; column < grid._columns; column++)
                {
                    int id = grid[row, column];
                    imageControls[row, column].Opacity = 1;
                    imageControls[row, column].Source = tileImages[id];
                }
            }
        }
        private void DrawBlock(Blocks block)
        {
            foreach (Position position in block.TilePositions())
            {
                imageControls[position._row, position._column].Opacity = 1;
                imageControls[position._row, position._column].Source = tileImages[block._id];
            }
        }
        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Blocks nextBlock = blockQueue._nextBlock;
            this.NextBlock.Source = previewImages[nextBlock._id];
        }
        private void DrawHeldBlock(Blocks heldBlock)
        {
            if (heldBlock == null)
                HoldBlock.Source = previewImages[0];
            else
                HoldBlock.Source = previewImages[heldBlock._id];
        }
        private void DrawGhostBlock(Blocks ghostBlock)
        {
            int dropDistance = playfieldState.BlockDropDistance();
            foreach(Position position in ghostBlock.TilePositions())
            {
                imageControls[position._row, position._column].Opacity = 0.25;
                imageControls[position._row, position._column].Source = tileImages[ghostBlock._id];
            }
        }
        private void Draw(PlayfieldState playfieldState)
        {
            DrawGrid(playfieldState._playfield);
            DrawGhostBlock(playfieldState.currentBlock);
            DrawBlock(playfieldState.currentBlock);
            DrawNextBlock(playfieldState._blockQueue);
            DrawHeldBlock(playfieldState._heldBlock);
            ScoreText.Text = $"Score: {playfieldState._score}";
        }
        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (playfieldState._gameOver)
                return;
            switch (e.Key)
            {
                case Key.Left:
                    playfieldState.MoveBlockLeft();
                    break;
                case Key.Right:
                    playfieldState.MoveBlockRight();
                    break;
                case Key.Down:
                    playfieldState.MoveBlockDown();
                    break;
                case Key.Z:
                    playfieldState.RotateBlockLeft();
                    break;
                case Key.X:
                    playfieldState.RotateBlockRight();
                    break;
                default:
                    return;
            }
            Draw(playfieldState);
        }
        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            playfieldState = new();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
        private async Task GameLoop()
        {
            Draw(playfieldState);

            while (!playfieldState._gameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (playfieldState._score * delayDecrease));
                await Task.Delay(delay);
                playfieldState.MoveBlockDown();
                Draw(playfieldState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {playfieldState._score}";
        }
    }
}
