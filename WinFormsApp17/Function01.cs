using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace WinFormsApp17
{
    //=======================
    //  Function01 クラス 　  
    //======================= 
    public class Function01
    {
        private LineManager lineManager;
        Form1 form1;

        public Function01(LineManager manager, Form1 form1)
        {
            this.lineManager = manager;
            this.form1 = form1;
        }

        //=======================
        //  線の端点をとる  　  
        //======================= 
        public bool TrySnapToEndpoint(
                         PointDec worldPoint,
                         Func<PointDec, PointF> worldToScreen,
                         out PointDec snapped)
        {
            const float thresholdPx = 20f;  //範囲
            float minDistSq = thresholdPx * thresholdPx;

            PointF pScreen = worldToScreen(worldPoint);

            bool found = false;
            snapped = worldPoint;

            foreach (var line in lineManager.decFile)
            {
                // start
                PointF s = worldToScreen(line.start);
                float dx1 = s.X - pScreen.X;
                float dy1 = s.Y - pScreen.Y;
                float d1 = dx1 * dx1 + dy1 * dy1;

                if (d1 < minDistSq)
                {
                    minDistSq = d1;
                    snapped = line.start;
                    found = true;
                }

                // end
                PointF e = worldToScreen(line.end);
                float dx2 = e.X - pScreen.X;
                float dy2 = e.Y - pScreen.Y;
                float d2 = dx2 * dx2 + dy2 * dy2;

                if (d2 < minDistSq)
                {
                    minDistSq = d2;
                    snapped = line.end;
                    found = true;
                }
            }

            return found;
        }

        //=======================
        //  線の端点をとる  02　  
        //======================= 
        public bool TrySnapToEndpoint02(
                         PointDec worldPoint,
                         out PointDec snapped)
        {
            const float thresholdPx = 20f;  //範囲
            float minDistSq = thresholdPx * thresholdPx;

            PointF pScreen = form1.WorldToScreen(worldPoint);

            bool found = false;
            snapped = worldPoint;

            foreach (var line in lineManager.decFile)
            {
                // start
                PointF s = form1.WorldToScreen(line.start);
                float dx1 = s.X - pScreen.X;
                float dy1 = s.Y - pScreen.Y;
                float d1 = dx1 * dx1 + dy1 * dy1;

                if (d1 < minDistSq)
                {
                    minDistSq = d1;
                    snapped = line.start;
                    found = true;
                }

                // end
                PointF e = form1.WorldToScreen(line.end);
                float dx2 = e.X - pScreen.X;
                float dy2 = e.Y - pScreen.Y;
                float d2 = dx2 * dx2 + dy2 * dy2;

                if (d2 < minDistSq)
                {
                    minDistSq = d2;
                    snapped = line.end;
                    found = true;
                }
            }

            return found;
        }

        //=======================
        //  スナップ関数  　  
        //======================= 
        public PointF ApplySnap(PointF p, PointF? startPos)
        {
            if (form1.snapMode == SnapMode.Free)
                return p;

            float dx = Math.Abs(p.X - startPos.Value.X);
            float dy = Math.Abs(p.Y - startPos.Value.Y);

            if (dx > dy)
                return new PointF(p.X, startPos.Value.Y);
            else
                return new PointF(startPos.Value.X, p.Y);
        }

        //=======================================
        //   敷地レイヤーから近い線を探す (decimal)
        //=======================================
        // クリックポイント(Decimal)、scalef(float)をください。
        // 敷地レイヤー（Layer=1）のうち、クリックポイントに近い線のIndex番号(int)をリターンします。
        public int GetNearestSiteLineIndex(PointDec click, float scalef)
        {
            decimal minDist = 999999m;
            int index = -1;

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                var l = lineManager.decFile[i];

                // 敷地レイヤーだけ対象
                if (l.Layer != 1)
                    continue;

                decimal d = DistancePointToSegment(click, l.start, l.end);

                const float HitRadiusPx = 25f; // ヒット:10ピクセル約2.6ｍｍ
                decimal hitDistWorld = (decimal)(HitRadiusPx / scalef);

                if (d < minDist && d < hitDistWorld)
                {
                    minDist = d;
                    index = i;
                }
            }

            return index;
        }

        //===============================
        //   近い線を探す (decimal)
        //===============================
        // クリックポイント(Decimal)、scalef(float)をください。
        // クリックポイントに近い線のIndex番号(int)をリターンします。
        public int GetNearestLineIndex(PointDec click, float scalef)
        {
            decimal minDist = 999999m;
            int index = -1;

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                var l = lineManager.decFile[i];
                decimal d = DistancePointToSegment(click, l.start, l.end);

                const float HitRadiusPx = 25f; // ヒット:10ピクセル約2.6ｍｍ
                decimal hitDistWorld = (decimal)(HitRadiusPx / scalef);

                if (d < minDist && d < hitDistWorld)
                {
                    minDist = d;
                    index = i;
                }
            }

            return index;
        }

        //===============================
        //   2番目の近い線を探す (decimal)
        //===============================
        // クリックポイント(Decimal)、scalef(float)をください。
        // すでにセレクトされてる線のIndex(int)をください。
        // クリックポイントに近い線のIndex番号(int)をリターンします。
        public int GetNearestLineIndexSecond(
             PointDec click,
             float scalef,
             int baseIndex)
        {
            decimal minDist = decimal.MaxValue;
            int index = -1;

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                // 1回目に選んだ線は除外
                if (i == baseIndex)
                    continue;

                var l = lineManager.decFile[i];
                decimal d = DistancePointToSegment(click, l.start, l.end);

                const float HitRadiusPx = 25f;
                decimal hitDistWorld = (decimal)(HitRadiusPx / scalef);

                if (d < minDist && d < hitDistWorld)
                {
                    minDist = d;
                    index = i;
                }
            }

            return index;
        }

        //======================================
        //   クリックポイントから近い円のIndexを返す
        //======================================
        // クリックポイント(Decimal)、scalef(float)をください。
        // クリックポイントから近い円のIndex(int)を返します。
        public int GetNearestCircleIndex(
             PointDec click,
             float scalef)
        {
            decimal minDist = decimal.MaxValue;
            int index = -1;

            const float HitRadiusPx = 25f;
            decimal hitDistWorld = (decimal)(HitRadiusPx / scalef);

            for (int i = 0; i < lineManager.CircleFile.Count; i++)
            {
                var c = lineManager.CircleFile[i];

                // 点と円周の距離（中心距離 − 半径）
                decimal dx = click.x - c.center.x;
                decimal dy = click.y - c.center.y;
                decimal distCenter =
                    (decimal)Math.Sqrt((double)(dx * dx + dy * dy));

                decimal d = Math.Abs(distCenter - c.radius);

                if (d < minDist && d < hitDistWorld)
                {
                    minDist = d;
                    index = i;
                }
            }

            return index;
        }

        //======================================================
        //   距離計算（decimal）
        //======================================================
        public decimal DistancePointToSegment(PointDec p, PointDec a, PointDec b)
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

        //======================================================
        //  Distance メソッド　２点間距離を返す
        //======================================================
        private decimal Distance(PointDec p1, PointDec p2)
        {
            decimal dx = p1.x - p2.x;
            decimal dy = p1.y - p2.y;
            return (decimal)Math.Sqrt((double)(dx * dx + dy * dy));
        }

        //======================================================
        //   ライン（円）の交点計算（decimal）
        //======================================================
        public bool TrySnapToIntersection(
              List<LineEntity> lines,
              List<CircleEntity> circles,
              int baseLineIndex,
              out PointDec snapped)
        {
            snapped = default;
            var baseLine = lines[baseLineIndex];

            decimal minDist = decimal.MaxValue;

            bool found = false;

            // 線×線
            for (int i = 0; i < lines.Count; i++)
            {
                if (i == baseLineIndex) continue;

                if (TryGetLineLineIntersection(baseLine, lines[i], out var ip))
                {
                    decimal d = Distance(ip, baseLine.start);

                    if (d < minDist)
                    {
                        minDist = d;
                        snapped = ip;
                        found = true;
                    }
                }
            }

            // 線×円
            foreach (var c in circles)
            {
                if (TryGetLineCircleIntersection(baseLine, c, out var ip))
                {
                    decimal d = Distance(ip, baseLine.start);
                    Debug.WriteLine($"enn d = {d}");
                    Debug.WriteLine($"enn minDist = {minDist}");

                    if (d < minDist)
                    {
                        minDist = d;
                        snapped = ip;
                        found = true;
                    }
                }
            }

            return found;
        }

        //======================================================
        //   ラインとラインの交点計算（decimal）
        //======================================================
        public bool TryGetLineLineIntersection(
              LineEntity l1,
              LineEntity l2,
              out PointDec ip)
        {
            ip = default;

            decimal x1 = l1.start.x, y1 = l1.start.y;
            decimal x2 = l1.end.x, y2 = l1.end.y;
            decimal x3 = l2.start.x, y3 = l2.start.y;
            decimal x4 = l2.end.x, y4 = l2.end.y;

            decimal d = (x1 - x2) * (y3 - y4)
                      - (y1 - y2) * (x3 - x4);

            if (d == 0)
                return false; // 平行

            decimal px =
                ((x1 * y2 - y1 * x2) * (x3 - x4)
               - (x1 - x2) * (x3 * y4 - y3 * x4)) / d;

            decimal py =
                ((x1 * y2 - y1 * x2) * (y3 - y4)
               - (y1 - y2) * (x3 * y4 - y3 * x4)) / d;

            ip = new PointDec(px, py);
            return true;
        }

        public bool TryGetLineCircleIntersection(
              LineEntity line,
              CircleEntity circle,
              out PointDec ip)
        {
            ip = default;

            // 線方向
            decimal dx = line.end.x - line.start.x;
            decimal dy = line.end.y - line.start.y;

            decimal fx = line.start.x - circle.center.x;
            decimal fy = line.start.y - circle.center.y;

            decimal a = dx * dx + dy * dy;
            decimal b = 2 * (fx * dx + fy * dy);
            decimal c = fx * fx + fy * fy - circle.radius * circle.radius;

            decimal discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
                return false;

            decimal t = (-b + (decimal)Math.Sqrt((double)discriminant)) / (2 * a);

            ip = new PointDec(
                line.start.x + t * dx,
                line.start.y + t * dy
            );

            return true;
        }

        //======================================================
        //  円（円周）と クリックポイントとの距離を消す（decimal）
        //======================================================
        public decimal DistancePointToCircle(
             PointDec p,
             PointDec center,
             decimal radius)
        {
            decimal dx = p.x - center.x;
            decimal dy = p.y - center.y;

            decimal distCenter =
                (decimal)Math.Sqrt((double)(dx * dx + dy * dy));

            // 円周までの距離（内外どちらでもOK）
            return Math.Abs(distCenter - radius);
        }

        //======================================================
        //  X方向かY方向か長い軸のほうでパラメータｔをとる
        //======================================================
        public decimal GetParameterT(PointDec p, PointDec a, PointDec b)
        {
            decimal dx = b.x - a.x;
            decimal dy = b.y - a.y;

            if (dx == 0 && dy == 0)
                return 0m;

            // 数値安定のため、長い軸を使う
            if (Math.Abs(dx) >= Math.Abs(dy))
                return (p.x - a.x) / dx;
            else
                return (p.y - a.y) / dy;
        }

        //================================
        //  右クリックで線の交点をとる
        //================================
        public bool TrySnapToIntersectionNear(
              PointDec mouse,
              List<LineEntity> lines,
              List<CircleEntity> circles,
              float currentScale,
              out PointDec snapped)
        {
            snapped = default;

            decimal pickTol = (decimal)(20f / Math.Max(currentScale, 0.0001f));

            decimal minDist = decimal.MaxValue;
            bool found = false;

            // 線 × 線
            for (int i = 0; i < lines.Count; i++)
            {
                if (!IsLineNearPoint(lines[i], mouse, pickTol))
                    continue;

                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (!IsLineNearPoint(lines[j], mouse, pickTol))
                        continue;

                    if (TryGetLineLineIntersection(lines[i], lines[j], out var ip))
                    {
                        decimal d = Distance(mouse, ip);

                        if (d < pickTol && d < minDist)
                        {
                            minDist = d;
                            snapped = ip;
                            found = true;
                        }
                    }
                }
            }

            // 線 × 円
            foreach (var line in lines)
            {
                foreach (var c in circles)
                {
                    if (TryGetLineCircleIntersection(line, c, out var ip))
                    {
                        decimal d = Distance(mouse, ip);

                        if (d < pickTol && d < minDist)
                        {
                            minDist = d;
                            snapped = ip;
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        //---------------------------------------------
        //  右クリックで線の交点をとる:クリック点と線との距離
        //---------------------------------------------
        private bool IsLineNearPoint(LineEntity line, PointDec p, decimal tol)
        {
            decimal d = DistancePointToSegment(
                p,
                line.start,
                line.end
            );

            return d <= tol;  //tol許容範囲
        }

        /////////////////////////
        ///
        public bool TrySnapToNearestPoint(
           PointDec mouse,
           List<LineEntity> lines,
           List<CircleEntity> circles,
           float scale,
          out PointDec snapped)
        {
            snapped = mouse;

            decimal snapRange = 5m / Math.Max((decimal)scale, 0.001m);

            PointDec bestPoint = mouse;
            decimal bestDistance = decimal.MaxValue;
            bool found = false;

            void AddCandidate(PointDec p)
            {
                decimal d = Distance(mouse, p);

                if (d <= snapRange && d < bestDistance)
                {
                    bestDistance = d;
                    bestPoint = p;
                    found = true;
                }
            }

            // 端点候補
            foreach (var line in lines)
            {
                AddCandidate(line.start);
                AddCandidate(line.end);
            }

            // 交点候補：近い線だけ
            for (int i = 0; i < lines.Count; i++)
            {
                if (!IsLineNearPoint(lines[i], mouse, snapRange))
                    continue;

                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (!IsLineNearPoint(lines[j], mouse, snapRange))
                        continue;

                    if (TryGetLineLineIntersection(lines[i], lines[j], out var p))
                    {
                        AddCandidate(p);
                    }
                }
            }

            if (found)
            {
                snapped = bestPoint;
                return true;
            }

            return false;
        }
    }
}