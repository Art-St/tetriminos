namespace Tetriminos.MainFunctions
{
    public class TBlock : Blocks
    {
        private readonly Position[][] _tileStates = new Position[][]
        {
            new Position[] {new(0,1), new(1,0), new(1,1), new(1,2) },
            new Position[] {new(0,1), new(1,1), new(1,2), new(2,1) },
            new Position[] {new(1,0), new(1,1), new(1,2), new(2,1) },
            new Position[] {new(0,1), new(1,0), new(1,1), new(2,1) }
        };
        public override int _id => 6;
        protected override Position _startOffset => new Position(0, 3);
        protected override Position[][] _tiles => _tileStates;
    }
}
