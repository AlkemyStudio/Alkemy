namespace IA
{
    public struct RaycastResult
    {
        public TouchData top;
        public TouchData right;
        public TouchData bottom;
        public TouchData left;

        bool ContainsBomb()
        {
            return top.IsBomb && right.IsBomb && bottom.IsBomb && left.IsBomb;
        }
    }
}