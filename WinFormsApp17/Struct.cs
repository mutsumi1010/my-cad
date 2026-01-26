namespace WinFormsApp17
{
    public struct LineEntity
    {
        public PointDec start;
        public PointDec end;
        public int Layer;
    }
    public struct CircleEntity
    {
        public PointDec center;
        public Decimal radius;
        public int Layer;
    }

}
