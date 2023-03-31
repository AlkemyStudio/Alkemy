namespace IA
{
    public struct TouchData
    {
        public Vector2 Position;
        public bool IsBomb;
        public bool IsWall;
        public bool IsTerrain;

        public TouchData(Vector2 Position, bool IsBomb = false, bool IsWall = false, bool IsTerrain = false)
        {
            this.Position = Position;
            this.IsBomb = IsBomb;
            this.IsWall = IsWall;
            this.IsTerrain = IsTerrain;
        }
    }
}

