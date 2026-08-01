using System;
using System.Drawing;
using System.Collections.Generic;

namespace WinFormsApp17
{
    public class ParallelManager
    {
        private LineManager lineManager;
        private Func<float> getScale;
        
        // ===== Parallel 状態 =====
        public int SelectedIndex { get; private set; } = -1;
        public bool IsCopyPreview { get; private set; } = false;
        public bool DirectionPositive { get; private set; } = true;

        private PointDec dragStartPointD;

        private decimal offsetDistance = 0;  //オフセットしたい距離 数値指定の場合

        //private readonly decimal scaledec;  //今は定数

        // ===== コンストラクタ =====
        public ParallelManager(LineManager manager, Func<float> getScale)
        {
            this.lineManager = manager;
            this.getScale = getScale;
        }

        //==============================================
        // SetOffsetDistance
        //==============================================
        // ===== オフセット距離セット メソッド =====
        public void SetOffsetDistance(decimal dist)
        {
             this.offsetDistance = dist;
        }
        // ===== オフセット距離GET メソッド =====
        public decimal GetOffsetDistance()
        {
            return offsetDistance;
        }

        //==============================================
        // ① マウスダウン
        //==============================================
        public void OnMouseDown(PointDec worldPos)
        {
            dragStartPointD = worldPos;
        }

        //==============================================
        // ② マウスムーブ：プレビュー方向判定
        //==============================================
        public void OnMouseMove(PointDec worldPos)
        {
            if (SelectedIndex < 0 || SelectedIndex >= lineManager.decFile.Count)
                return;

            var line = lineManager.decFile[SelectedIndex];
            var s = line.start;
            var g = line.end;

            decimal dx = g.x - s.x;
            decimal dy = g.y - s.y;

            double len = Math.Sqrt((double)(dx * dx + dy * dy));
            if (len == 0) return;

            double nx = -(double)dy / len;
            double ny = (double)dx / len;

            decimal deltaX = worldPos.x - dragStartPointD.x;
            decimal deltaY = worldPos.y - dragStartPointD.y;

            double dot = (double)deltaX * nx + (double)deltaY * ny;

            decimal scale = (decimal)getScale();
            decimal thresholdD = 3m / scale;
            double threshold = (double)thresholdD;

            if (Math.Abs(dot) > threshold)
            {
                DirectionPositive = dot > 0;
                IsCopyPreview = true;
            }
        }
        //==============================================
        // ③ マウスクリック：確定または線選択
        //==============================================
        public bool OnMouseClick(PointDec worldPos)
        {
            // すでにプレビュー中 → 確定処理
            //if (IsCopyPreview && SelectedIndex >= 0)
            if (IsCopyPreview &&
                SelectedIndex >= 0 &&
                offsetDistance != 0m)
            {
                   var line = lineManager.decFile[SelectedIndex];

                   PointDec p1 = line.start;
                   PointDec p2 = line.end;

                   var (offsetX, offsetY) =
                       GetNormalOffset(p1, p2, offsetDistance, DirectionPositive);

                   var newStart = new PointDec(p1.x + offsetX, p1.y + offsetY);
                   var newEnd = new PointDec(p2.x + offsetX, p2.y + offsetY);

                   //
                   //  リストにラインを追加
                   //
                   lineManager.AddLine(newStart, newEnd);

                   IsCopyPreview = false;
                   SelectedIndex = -1;
                   return true;
               } 

            if (SelectedIndex >= 0)
            {
                // 基準線の取り出し
                var line = lineManager.decFile[SelectedIndex];
                // 基準線のstart と　end
                PointDec p1 = line.start;
                PointDec p2 = line.end;

                // クリック点を通る平行線を作るための移動ベクトルを取得
                var (offsetX, offsetY) =
                    GetOffsetToPassThroughPoint(p1, p2, worldPos);

                var newStart = new PointDec(
                    p1.x + offsetX,
                    p1.y + offsetY
                );

                var newEnd = new PointDec(
                    p2.x + offsetX,
                    p2.y + offsetY
                );

                lineManager.AddLine(newStart, newEnd);

                IsCopyPreview = false;
                SelectedIndex = -1;

                return true;
            }


            // 単線を選択してリターン
            SelectedIndex = SelectLine(worldPos);
                return (SelectedIndex >= 0);
        }

        //==============================================
        // 指定した点を通る平行線を作るための
        // 法線方向の移動ベクトルを求める
        //==============================================
        private (decimal offsetX, decimal offsetY)
            GetOffsetToPassThroughPoint(
                PointDec lineStart,
                PointDec lineEnd,
                PointDec clickPoint)
        {
            decimal dx = lineEnd.x - lineStart.x;
            decimal dy = lineEnd.y - lineStart.y;

            decimal lengthSquared = dx * dx + dy * dy;

            // 始点と終点が同じ場合
            if (lengthSquared == 0)
                return (0m, 0m);

            // クリック点を基準線へ投影するための係数
            // 線分ではなく、基準線を無限に延長した直線へ投影する
            decimal t =
                ((clickPoint.x - lineStart.x) * dx
                + (clickPoint.y - lineStart.y) * dy)
                / lengthSquared;

            // 基準線上の垂直投影点
            decimal projectionX = lineStart.x + t * dx;
            decimal projectionY = lineStart.y + t * dy;

            // 投影点からクリック点までのベクトル
            // これが基準線に対する法線方向の移動量
            decimal offsetX = clickPoint.x - projectionX;
            decimal offsetY = clickPoint.y - projectionY;

            return (offsetX, offsetY);
        }

        //==============================================
        // 線の選択
        //==============================================
        private int SelectLine(PointDec click)
        {
            decimal minDist = 999999m;
            int index = -1;

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                var l = lineManager.decFile[i];
                decimal d = DistancePointToSegment(click, l.start, l.end);

                // スケール調整
                float scale = getScale();
                decimal hitRange = 20m / (decimal)scale;

                if (d < hitRange && d < minDist)
                {
                    minDist = d;
                    index = i;
                }
            }
            return index;
        }

        //==============================================
        // プレビュー線が存在する？
        //==============================================
        public bool HasPreview()
        {
            return IsCopyPreview && SelectedIndex >= 0;
        }

        //==============================================
        // プレビュー線（世界座標）を返す
        //==============================================
        public (PointDec start, PointDec end) GetPreviewLine()
        {
            var line = lineManager.decFile[SelectedIndex];
            var p1 = line.start;
            var p2 = line.end;

            var (ox, oy) = GetNormalOffset(p1, p2, offsetDistance, DirectionPositive);

            return (
                new PointDec(p1.x + ox, p1.y + oy),
                new PointDec(p2.x + ox, p2.y + oy)
            );
        }

        //==============================================
        // ESCでキャンセル
        //==============================================
        public void Reset()
        {
            SelectedIndex = -1;
            IsCopyPreview = false;
        }

        //==============================================
        // 法線オフセット（decimal返却）
        //==============================================
        private (decimal offsetX, decimal offsetY)
            GetNormalOffset(PointDec a, PointDec b, decimal dist, bool positive)
        {
            double dx = (double)(b.x - a.x);
            double dy = (double)(b.y - a.y);

            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len == 0) return (0, 0);

            double nx = -dy / len;
            double ny = dx / len;

            if (!positive) { nx = -nx; ny = -ny; }

            return ((decimal)(nx * (double)dist),
                    (decimal)(ny * (double)dist));
        }

        //==============================================
        // 距離計算（decimal）
        //==============================================
        private decimal DistancePointToSegment(PointDec p, PointDec a, PointDec b)
        {
            decimal dx = b.x - a.x;
            decimal dy = b.y - a.y;

            if (dx == 0 && dy == 0)
                return Distance(p, a);

            decimal t = ((p.x - a.x) * dx + (p.y - a.y) * dy)
                        / (dx * dx + dy * dy);
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

        //==============================================
        // 複線操作を最初の状態へ戻す
        //==============================================
        public void ResetOperation()
        {
            SelectedIndex = -1;
            IsCopyPreview = false;
        }
    }
}

