using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace WinFormsApp17
{
    internal class ImportDxf
    {
        public (
            List<LineEntity> decFile,
            List<Dimension> dimensions
        ) ImportDxf01(string path)
        {
            var lines = new List<LineEntity>();
            var dimensions = new List<Dimension>();

            PointDec Ext1 = new();
            PointDec Ext2 = new();
            PointDec Ext3 = new();
            PointDec Ext4 = new();
            PointDec Sen1 = new();
            PointDec Sen2 = new();

            PointDec dimTextPos = new();
            string? dimTextValue = "";
            float dimTextAngle = 0f;
            float dimTextHeight = 240f;

            using var sr = new StreamReader(path);

            string? code;
            string? data;

            bool inLine = false;
            bool inText = false;

            string currentLayer = "0";

            PointDec p1 = new();
            PointDec p2 = new();

            while ((code = sr.ReadLine()) != null)
            {
                data = sr.ReadLine();
                if (data == null) break;

                // =========================
                // Entity start
                // =========================
                if (code.Trim() == "0")
                {
                    inLine = false;
                    inText = false;

                    if (data.Equals("LINE", StringComparison.OrdinalIgnoreCase))
                    {
                        inLine = true;
                        currentLayer = "0";
                        continue;
                    }

                    if (data.Equals("TEXT", StringComparison.OrdinalIgnoreCase))
                    {
                        inText = true;
                        currentLayer = "0";
                        dimTextValue = "";
                        dimTextAngle = 0f;
                        dimTextHeight = 240f;
                        continue;
                    }
                }

                // =========================
                // LINE
                // =========================
                if (inLine)
                {
                    switch (code.Trim())
                    {
                        case "8":
                            currentLayer = data.Trim();
                            break;

                        case "10":
                            p1.x = decimal.Parse(data, CultureInfo.InvariantCulture);
                            break;
                        case "20":
                            p1.y = decimal.Parse(data, CultureInfo.InvariantCulture);
                            break;
                        case "11":
                            p2.x = decimal.Parse(data, CultureInfo.InvariantCulture);
                            break;
                        case "21":
                            p2.y = decimal.Parse(data, CultureInfo.InvariantCulture);

                            if (currentLayer.EndsWith("4-1-1"))
                            {
                                Ext1 = new PointDec(p1.x, p1.y);
                                Ext2 = new PointDec(p2.x, p2.y);
                            }
                            else if (currentLayer.EndsWith("4-1-2"))
                            {
                                Ext3 = new PointDec(p1.x, p1.y);
                                Ext4 = new PointDec(p2.x, p2.y);
                            }
                            else if (currentLayer.EndsWith("4-2"))
                            {
                                Sen1 = new PointDec(p1.x, p1.y);
                                Sen2 = new PointDec(p2.x, p2.y);
                            }
                            else
                            {
                                // --- 通常の線（Layer保持） ---
                                int layer = 0;
                                int.TryParse(currentLayer, out layer);

                                lines.Add(new LineEntity
                                {
                                    start = new PointDec(p1.x, p1.y),
                                    end = new PointDec(p2.x, p2.y),
                                    Layer = layer
                                });

                              //  Debug.WriteLine(lines[^1].Layer);
                            }
                            break;
                    }
                }

                // =========================
                // TEXT
                // =========================
                if (inText)
                {
                    switch (code.Trim())
                    {
                        case "8":
                            currentLayer = data.Trim();
                            break;

                        case "10":
                            dimTextPos.x = decimal.Parse(data, CultureInfo.InvariantCulture);
                            break;
                        case "20":
                            dimTextPos.y = decimal.Parse(data, CultureInfo.InvariantCulture);
                            break;
                        case "40":
                            dimTextHeight = float.Parse(data, CultureInfo.InvariantCulture);
                            break;
                        case "50":
                            dimTextAngle = float.Parse(data, CultureInfo.InvariantCulture);
                            break;
                        case "1":
                            dimTextValue = data;

                            if (currentLayer.EndsWith("4-3"))
                            {
                                var dim = new Dimension
                                {
                                    Ext1 = Ext1,
                                    Ext2 = Ext2,
                                    Ext3 = Ext3,
                                    Ext4 = Ext4,
                                    Sen1 = Sen1,
                                    Sen2 = Sen2,
                                    dimTextPos = dimTextPos,
                                    dimTextValue = dimTextValue,
                                    dimTextAngle = dimTextAngle,
                                    dimTextHeight = dimTextHeight
                                };

                                dimensions.Add(dim);
                            }

                            inText = false;
                            break;
                    }
                }
            }

            // =========================
            // Y反転（図面LINE基準）
            // =========================
            decimal minY = decimal.MaxValue;
            decimal maxY = decimal.MinValue;

            foreach (var l in lines)
            {
                minY = Math.Min(minY, Math.Min(l.start.y, l.end.y));
                maxY = Math.Max(maxY, Math.Max(l.start.y, l.end.y));
            }

            if (minY != decimal.MaxValue)
            {
                decimal baseY = minY + maxY;

                for (int i = 0; i < lines.Count; i++)
                {
                    var l = lines[i];
                    lines[i] = new LineEntity
                    {
                        start = new PointDec(l.start.x, baseY - l.start.y),
                        end = new PointDec(l.end.x, baseY - l.end.y),
                        Layer = l.Layer
                    };
                }

                foreach (var d in dimensions)
                {
                    d.Ext1 = new PointDec(d.Ext1.x, baseY - d.Ext1.y);
                    d.Ext2 = new PointDec(d.Ext2.x, baseY - d.Ext2.y);
                    d.Ext3 = new PointDec(d.Ext3.x, baseY - d.Ext3.y);
                    d.Ext4 = new PointDec(d.Ext4.x, baseY - d.Ext4.y);
                    d.Sen1 = new PointDec(d.Sen1.x, baseY - d.Sen1.y);
                    d.Sen2 = new PointDec(d.Sen2.x, baseY - d.Sen2.y);
                    d.dimTextPos = new PointDec(d.dimTextPos.x, baseY - d.dimTextPos.y);
                }
            }

            return (lines, dimensions);
        }
    }
}
