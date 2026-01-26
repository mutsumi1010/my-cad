namespace WinFormsApp17
{
    public struct PointDec
    {
        public decimal x;
        public decimal y;

        public PointDec(decimal x, decimal y)
        {
            this.x = x;
            this.y = y;
        }

        // String に変換するメソッド（描画用）
        public override string ToString() => $"({x}, {y})";

        // PointF に変換するメソッド（描画用）
        public PointF ToPointF() => new PointF((float)x, (float)y);

        public bool Equals(PointDec other) => x == other.x && y == other.y;
        public override bool Equals(object? obj) => obj is PointDec p && Equals(p);
        public override int GetHashCode() => HashCode.Combine(x, y);

    }

}
