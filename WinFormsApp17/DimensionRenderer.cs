using System.Collections.Generic;
using System.Drawing;

namespace WinFormsApp17
{
    public class DimensionRenderer
    {
        public void Draw(
            Graphics g,
            IReadOnlyList<Dimension> dimFile,
            float scalef)
        {
            foreach (var d in dimFile)
            {
                DrawExtensionLines(g, d);
                DrawDimensionLine(g, d);
                DrawIntersectionMarkers(g, d, scalef);
                DrawDimensionText(g, d);
            }
        }

        // =========================
        // 引出線
        // =========================
        private void DrawExtensionLines(Graphics g, Dimension d)
        {
            g.DrawLine(Pens.Green, d.Ext1.ToPointF(), d.Ext2.ToPointF());
            g.DrawLine(Pens.Green, d.Ext3.ToPointF(), d.Ext4.ToPointF());
        }

        // =========================
        // 寸法線
        // =========================
        private void DrawDimensionLine(Graphics g, Dimension d)
        {
            g.DrawLine(Pens.Green, d.Sen1.ToPointF(), d.Sen2.ToPointF());
        }

        // =========================
        // 交点マーカー
        // =========================
        private void DrawIntersectionMarkers(Graphics g, Dimension d, float scalef)
        {
            float r = 4f / scalef;

            PointF ip1 = d.Ext2.ToPointF();
            PointF ip2 = d.Ext4.ToPointF();

            g.FillEllipse(Brushes.Black, ip1.X - r, ip1.Y - r, r * 2, r * 2);
            g.FillEllipse(Brushes.Black, ip2.X - r, ip2.Y - r, r * 2, r * 2);
        }

        // =========================
        // 寸法文字
        // =========================
        private void DrawDimensionText(Graphics g, Dimension d)
        {
            using var font = new Font("MS Gothic", 240f, GraphicsUnit.Pixel);

            float textOffsetWorld = 12f;
            float fontFix = font.Size * 0.3f;
            float extraLift = font.Size * 0.1f;

            string text = d.dimTextValue ?? "";

            var format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            if (d.Sen1.x == d.Sen2.x)
            {
                // 垂直寸法
                float cx = d.Sen1.ToPointF().X - textOffsetWorld - fontFix - extraLift;
                float cy = (d.Sen1.ToPointF().Y + d.Sen2.ToPointF().Y) / 2f;

                var state = g.Save();
                g.TranslateTransform(cx, cy);
                g.RotateTransform(-90f);
                g.DrawString(text, font, Brushes.Green, 0, 0, format);
                g.Restore(state);
            }
            else
            {
                // 水平寸法
                float cx = (d.Sen1.ToPointF().X + d.Sen2.ToPointF().X) / 2f;
                float cy = d.Sen1.ToPointF().Y - textOffsetWorld - fontFix - extraLift;

                g.DrawString(text, font, Brushes.Green, cx, cy, format);
            }
        }
    }
}
