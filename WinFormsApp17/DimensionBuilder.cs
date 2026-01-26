using System;
using System.Collections.Generic;

namespace WinFormsApp17
{
    public class DimensionBuilder
    {
        private readonly List<PointDec> points = new();

        public decimal Ext1X { get; private set; }
        public decimal Ext2X { get; private set; }
        public decimal Ext1Y { get; private set; }
        public decimal Ext2Y { get; private set; }
        
        public void SetExt1(PointDec p)
        {
            Ext1X = p.x;
            Ext1Y = p.y;
        }

        public void SetExt2(PointDec p)
        {
            Ext2X = p.x;
            Ext2Y = p.y;
        }

        public void AddPoint(PointDec p)
        {
            points.Add(p);
        }

        public Dimension? TryBuildNext(Form1.DimensionDirection direction)
        {
            if (points.Count < 2)
                return null;

            PointDec p1 = points[^2];
            PointDec p2 = points[^1];

            if (direction == Form1.DimensionDirection.Horizontal)
            {
                decimal value = Math.Abs(p2.x - p1.x);

                return new Dimension
                {
                    Ext1 = new PointDec(p1.x, Ext1Y),
                    Ext2 = new PointDec(p1.x, Ext2Y),
                    Ext3 = new PointDec(p2.x, Ext1Y),
                    Ext4 = new PointDec(p2.x, Ext2Y),

                    Sen1 = new PointDec(p1.x, Ext2Y),
                    Sen2 = new PointDec(p2.x, Ext2Y),

                    dimTextPos = new PointDec(
                        (p1.x + p2.x) / 2m, Ext2Y),
                
                    dimTextValue = value.ToString("#,##0.##"),
                    dimTextAngle = 0f,
                    dimTextHeight = 240f
                };
            }
            else // Vertical
            {
                decimal value = Math.Abs(p2.y - p1.y);

                return new Dimension
                {
                    Ext1 = new PointDec(Ext1X, p1.y),
                    Ext2 = new PointDec(Ext2X, p1.y),
                    Ext3 = new PointDec(Ext1X, p2.y),
                    Ext4 = new PointDec(Ext2X, p2.y),

                    Sen1 = new PointDec(Ext2X, p1.y),
                    Sen2 = new PointDec(Ext2X, p2.y),

                    dimTextPos = new PointDec(
                        Ext2X,
                        (p1.y + p2.y) / 2m
                    ),
                    dimTextValue = value.ToString("#,##0.##"),
                    dimTextAngle = 90f,
                    dimTextHeight = 240f
                };
            }
        }

        public void Reset()
        {
            points.Clear();
        }

        public int PointCount => points.Count;
    }
}
