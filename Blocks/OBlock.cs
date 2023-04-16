namespace Tetriminos.MainFunctions
{
    public class OBlock : Blocks
    {
        private readonly Position[][] _tileStates = new Position[][]
        {
            new Position[] {new(0,0), new(0,1), new(1,0), new(1,1) }
        };
        public override int _id => 4;
        protected override Position _startOffset => new Position(0, 4);
        protected override Position[][] _tiles => _tileStates;
    }
}
