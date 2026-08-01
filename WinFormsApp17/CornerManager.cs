using System;
using System.Collections.Generic;
using System.Drawing;
using WinFormsApp17;

namespace WinFormsApp17
{
    public class CornerManager
    {
        private LineManager lineManager;
        private SiteDataList siteDataList;
        private RoadDataList roadDataList;
        private BuildingDataList buildingDataList;

        // 状態管理
        public int ClickCount { get; private set; } = 0;
        public int FirstIndex { get; private set; } = -1;
        public int SecondIndex { get; private set; } = -1;

        public int siteFirstIndex { get; private set; } = -1;
        public int siteSecondIndex { get; private set; } = -1;

        public int roadFirstIndex { get; private set; } = -1;
        public int roadSecondIndex { get; private set; } = -1;

        public int buildingFirstIndex { get; private set; } = -1;
        public int buildingSecondIndex { get; private set; } = -1;

        public PointDec FirstClickPointD { get; private set; }
        public PointDec SecondClickPointD { get; private set; }

        //--------------------------
        //  コンストラクタ
        //--------------------------
        public CornerManager(
                      LineManager manager,
                      SiteDataList site,
                      RoadDataList road,
                      BuildingDataList building)
        {
            this.lineManager = manager;
            this.siteDataList = site;
            this.roadDataList = road;
            this.buildingDataList = building;
        }


        //===============================
        //   近い線を探す (decimal)
        //===============================
        public int GetNearestLineIndex(PointDec click, float scalef)
        {
            decimal minDist = 999999m;
            int index = -1;

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                var l = lineManager.decFile[i];

                if (l.Layer == 8)   // DXF背景線は対象外
                    continue;
                if (l.Layer == 9)
                    continue;

                decimal d = DistancePointToSegment(click, l.start, l.end);

                const float HitRadiusPx = 12f; // ヒット:10ピクセル約2.6ｍｍ
                decimal hitDistWorld = (decimal)(HitRadiusPx / scalef);

                if (d < minDist && d < hitDistWorld)
                {
                    minDist = d;
                    index = i;
                }
            }
            return index;
        }

        //======================================================
        //   ClickCorner() : Form1 から呼ばれる入口
        //======================================================
        public bool ClickCorner(PointDec click, float scalef)
        {
            int idx = GetNearestLineIndex(click, scalef);
            if (idx < 0) return false;

            if (ClickCount == 0)
            {
                FirstIndex = idx;
                FirstClickPointD = click;
                ClickCount = 1;
                return true;
            }
            else
            {
                SecondIndex = idx;
                SecondClickPointD = click;
                ClickCount = 0;

                CreateCorner();
                return true;
            }
        }

        //======================================================
        //  Corner作成（交点→トリム）
        //======================================================
        private void CreateCorner()
        {
            // 同じ線を選んでいたら中断
            if (FirstIndex == SecondIndex)
            {
                FirstIndex = -1;
                SecondIndex = -1;
                return;
            }

            // =========================
            // decFile から元の線を取得
            // =========================
            var line1 = lineManager.decFile[FirstIndex];
            var line2 = lineManager.decFile[SecondIndex];

            PointDec A1 = line1.start;
            PointDec B1 = line1.end;
            PointDec A2 = line2.start;
            PointDec B2 = line2.end;

            // =========================
            // 交点計算
            // =========================
            if (!TryGetIntersection(A1, B1, A2, B2, out PointDec ip))
            {
                FirstIndex = -1;
                SecondIndex = -1;
                return;
            }

            // =========================
            // Site / Road / Building 側の index 探索
            // （旧タプル構造のため）
            // =========================
            siteFirstIndex = FindIndex(siteDataList.LineList, A1, B1);
            siteSecondIndex = FindIndex(siteDataList.LineList, A2, B2);

            roadFirstIndex = FindIndex(roadDataList.LineList, A1, B1);
            roadSecondIndex = FindIndex(roadDataList.LineList, A2, B2);

            buildingFirstIndex = FindIndex(buildingDataList.LineList, A1, B1);
            buildingSecondIndex = FindIndex(buildingDataList.LineList, A2, B2);

            // =========================
            // Trim（形状計算はタプルでOK）
            // =========================
            var t1 = TrimTowardClick((A1, B1), ip, FirstClickPointD);
            var t2 = TrimTowardClick((A2, B2), ip, SecondClickPointD);

            // =========================
            // LineEntity に戻す（Layerは元の線を継承）
            // =========================
            var newLine1 = new LineEntity
            {
                start = t1.A,
                end = t1.B,
                Layer = line1.Layer
            };

            var newLine2 = new LineEntity
            {
                start = t2.A,
                end = t2.B,
                Layer = line2.Layer
            };

            // =========================
            // Corner 反映
            // =========================
            lineManager.CornerLine(
                FirstIndex,
                SecondIndex,
                newLine1,
                newLine2,
                siteFirstIndex,
                siteSecondIndex,
                roadFirstIndex,
                roadSecondIndex,
                buildingFirstIndex,
                buildingSecondIndex
            );

            FirstIndex = -1;
            SecondIndex = -1;
        }

        private int FindIndex(
              List<(PointDec start, PointDec end)> list,
              PointDec s,
              PointDec e)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].start.Equals(s) && list[i].end.Equals(e))
                    return i;
            }
            return -1;
        }

        //======================================================
        //  クリック方向に向かって交点でトリム
        //======================================================
        private (PointDec A, PointDec B) TrimTowardClick(
            (PointDec A, PointDec B) line,
            PointDec ip,
            PointDec click)
        {
            var (A, B) = line;

            decimal dClickX = click.x - ip.x;
            decimal dClickY = click.y - ip.y;

            decimal projA = (A.x - ip.x) * dClickX + (A.y - ip.y) * dClickY;
            decimal projB = (B.x - ip.x) * dClickX + (B.y - ip.y) * dClickY;

            if (projA > projB)
                return (A, ip);
            else
                return (ip, B);
        }

        //======================================================
        //  線同士の交点計算（decimal）
        //======================================================
        private bool TryGetIntersection(
            PointDec p1, PointDec p2,
            PointDec p3, PointDec p4,
            out PointDec ip)
        {
            ip = new PointDec();

            decimal A1 = p2.y - p1.y;
            decimal B1 = p1.x - p2.x;
            decimal C1 = A1 * p1.x + B1 * p1.y;

            decimal A2 = p4.y - p3.y;
            decimal B2 = p3.x - p4.x;
            decimal C2 = A2 * p3.x + B2 * p3.y;

            decimal det = A1 * B2 - A2 * B1;
            if (Math.Abs(det) < 0.0000001m)
                return false;

            ip.x = (B2 * C1 - B1 * C2) / det;
            ip.y = (A1 * C2 - A2 * C1) / det;
            return true;
        }

        //======================================================
        //   距離計算（decimal）
        //======================================================
        private decimal DistancePointToSegment(PointDec p, PointDec a, PointDec b)
        {
            decimal dx = b.x - a.x;
            decimal dy = b.y - a.y;

            if (dx == 0 && dy == 0)
                return Distance(p, a);

            decimal t = ((p.x - a.x) * dx + (p.y - a.y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0m, Math.Min(1m, t));

            decimal px = a.x + t * dx;
            decimal py = a.y + t * dy;

            return Distance(p, new PointDec(px, py));
        }

        private decimal Distance(PointDec p1, PointDec p2)
        {
            decimal dx = p1.x - p2.x;
            decimal dy = p1.y - p2.y;
            return (decimal)Math.Sqrt((double)(dx * dx + dy * dy));
        }

        //===============================
        // Corner 状態リセット
        //===============================
        public void Reset()
        {
            ClickCount = 0;
            FirstIndex = -1;
            SecondIndex = -1;
        }
    }
}

