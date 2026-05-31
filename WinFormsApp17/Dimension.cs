namespace WinFormsApp17
{
    public class Dimension
    {
        public int Layer { get; set; } = 0;
        public PointDec Ext1 { get; set; }
        public PointDec Ext2 { get; set; }
        public PointDec Ext3 { get; set; }
        public PointDec Ext4 { get; set; }

        public PointDec Sen1 { get; set; }
        public PointDec Sen2 { get; set; }

        public PointDec dimTextPos { get; set; }
        public string? dimTextValue { get; set; } 
        public float dimTextAngle { get; set; }
        public float dimTextHeight { get; set; }
    }
}