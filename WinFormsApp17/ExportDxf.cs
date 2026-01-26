using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using static System.Windows.Forms.LinkLabel;

namespace WinFormsApp17
{
    internal class ExportDxf
    {
        private static readonly CultureInfo ci = CultureInfo.InvariantCulture;

        public void ExportDxf01(
            string filename,
            List<LineEntity> decFile,
            List<Dimension> dimensions)
        {
            // ----------------------------
            // ① 図形ラインのみで Y 範囲を取得
            // ----------------------------
            decimal minY = decimal.MaxValue;
            decimal maxY = decimal.MinValue;
            decimal baseY = 0m; // いずれのリストにも線がない場合の既定値

            // 1. 建物のLineListをチェックし、線があればbaseYを計算

            if (decFile != null && decFile.Count > 0)
            {
                foreach (var line in decFile)
                {
                    minY = Math.Min(minY, Math.Min(line.start.y, line.end.y));
                    maxY = Math.Max(maxY, Math.Max(line.start.y, line.end.y));
                }
                baseY = minY + maxY;
            }

            using (var sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                // ----------------------------
                // DXF ヘッダ
                // ----------------------------
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("ENTITIES");

                // ----------------------------
                // 通常の線（decFile）
                // ----------------------------
                foreach (var line in decFile)
                {
                    WriteLineEntity(sw, line.start, line.end, baseY, 
                        line.Layer.ToString());
                }

                // ----------------------------
                // 寸法
                // ----------------------------
                foreach (var d in dimensions)
                {
                    // 引出線
                    WriteLineEntity(sw, d.Ext1, d.Ext2, baseY, "4-1-1");
                    WriteLineEntity(sw, d.Ext3, d.Ext4, baseY, "4-1-2");

                    // 寸法線
                    WriteLineEntity(sw, d.Sen1, d.Sen2, baseY, "4-2");

                    // 寸法文字
                    bool isVertical = IsVertical(d.Sen1, d.Sen2);
                    WriteTextEntity(
                        sw,
                        GetMidPoint(d.Sen1, d.Sen2),
                        d.dimTextValue ?? "",
                        isVertical,
                        baseY,
                        "4-3"
                    );
                }

                // ----------------------------
                // セクション終了
                // ----------------------------
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("EOF");
            }
        }

        // ============================
        // LINE エンティティ（Y反転）
        // ============================
        private void WriteLineEntity(
            StreamWriter sw,
            PointDec start,
            PointDec end,
            decimal baseY,
            string layerName)
        {
            sw.WriteLine("0");
            sw.WriteLine("LINE");
            sw.WriteLine("8");
            sw.WriteLine(layerName);
            sw.WriteLine("6");
            sw.WriteLine("CONTINUOUS");

            sw.WriteLine("10");
            sw.WriteLine(start.x.ToString("0.0000000000000000", ci));
            sw.WriteLine("20");
            sw.WriteLine((baseY - start.y).ToString("0.0000000000000000", ci));

            sw.WriteLine("11");
            sw.WriteLine(end.x.ToString("0.0000000000000000", ci));
            sw.WriteLine("21");
            sw.WriteLine((baseY - end.y).ToString("0.0000000000000000", ci));
        }

        // ============================
        // TEXT エンティティ（Y反転）
        // ============================
        private void WriteTextEntity(
            StreamWriter sw,
            PointDec pos,
            string text,
            bool vertical,
            decimal baseY,
            string layerName)
        {
            sw.WriteLine("0");
            sw.WriteLine("TEXT");
            sw.WriteLine("8");
            sw.WriteLine(layerName);

            sw.WriteLine("10");
            sw.WriteLine(pos.x.ToString("0.0000000000000000", ci));
            sw.WriteLine("20");
            sw.WriteLine((baseY - pos.y).ToString("0.0000000000000000", ci));

            sw.WriteLine("40");
            sw.WriteLine("240"); // 文字高さ

            sw.WriteLine("50");
            sw.WriteLine(vertical ? "90" : "0");

            sw.WriteLine("1");
            sw.WriteLine(text);
        }

        // ============================
        // 中点
        // ============================
        private PointDec GetMidPoint(PointDec a, PointDec b)
        {
            return new PointDec(
                (a.x + b.x) / 2m,
                (a.y + b.y) / 2m
            );
        }

        // ============================
        // 垂直判定
        // ============================
        private bool IsVertical(PointDec a, PointDec b)
        {
            return Math.Abs((double)(a.x - b.x))
                 < Math.Abs((double)(a.y - b.y));
        }
    }
}
