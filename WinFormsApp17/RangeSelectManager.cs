using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace WinFormsApp17
{
    internal class RangeSelectManager
    {
        private readonly LineManager lineManager;

        // 1点目を取得済みか
        public bool IsSelecting { get; private set; }

        // 範囲の1点目・2点目
        public PointDec StartPoint { get; private set; }
        public PointDec EndPoint { get; private set; }

        // 四角が確定して存在していることを表す値
        public bool HasRange { get; private set; }

        // 選択された線のdecFile内インデックス
        public List<int> SelectedIndices { get; }
            = new List<int>();

        public RangeSelectManager(LineManager lineManager)
        {
            this.lineManager = lineManager;
        }

        //==============================================
        // 1点目取得後、マウス位置を2点目候補として更新
        //==============================================
        public void OnMouseMove(PointDec world)
        {
            if (!IsSelecting)
                return;

            EndPoint = world;
        }

        //==============================================
        // 範囲選択：1点目・2点目
        //==============================================
 
        public void OnMouseClick(PointDec world,float scalef)
        {
            // 四角がすでに確定している
            // 今は何もせず、その四角を残す
            if (HasRange && !IsSelecting)
            {
                int clickedIndex = SelectLine(world, scalef);

                // 線が見つからなければ何もしない
                if (clickedIndex < 0)
                    return;

                // すでに選択済みなら解除
                if (SelectedIndices.Contains(clickedIndex))
                {
                    SelectedIndices.Remove(clickedIndex);
                }
                // 選択されていなければ追加
                else
                {
                    SelectedIndices.Add(clickedIndex);
                }

                return;
            }

            // 1点目
            if (!IsSelecting)
            {
                StartPoint = world;
                EndPoint = world;

                IsSelecting = true;
                HasRange = true;

                return;
            }

            // 2点目
            EndPoint = world;

            // マウス追従は終了
            IsSelecting = false;

            // 四角は残す
            HasRange = true;

            SelectLinesInsideRange();
        }

        //==============================================
        // 完全に範囲内に入っている線を選択
        //==============================================
        private void SelectLinesInsideRange()
        {
            SelectedIndices.Clear();

            decimal minX = Math.Min(StartPoint.x, EndPoint.x);
            decimal maxX = Math.Max(StartPoint.x, EndPoint.x);
            decimal minY = Math.Min(StartPoint.y, EndPoint.y);
            decimal maxY = Math.Max(StartPoint.y, EndPoint.y);

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                LineEntity line = lineManager.decFile[i];

                bool startInside =
                    IsPointInside(
                        line.start,
                        minX,
                        maxX,
                        minY,
                        maxY
                    );

                bool endInside =
                    IsPointInside(
                        line.end,
                        minX,
                        maxX,
                        minY,
                        maxY
                    );

                // 始点と終点が両方とも範囲内なら選択
                if (startInside && endInside)
                {
                    SelectedIndices.Add(i);
                }
            }
        }

        //==============================================
        // 点が選択範囲内にあるか
        //==============================================
        private bool IsPointInside(
            PointDec point,
            decimal minX,
            decimal maxX,
            decimal minY,
            decimal maxY)
        {
            return
                point.x >= minX &&
                point.x <= maxX &&
                point.y >= minY &&
                point.y <= maxY;
        }

        //==============================================
        // 範囲選択を解除
        //==============================================
         public void Reset()
        {
            IsSelecting = false;
            HasRange = false;
            SelectedIndices.Clear();
        }


        //==============================================
        // クリック位置に近い線を探す
        //==============================================
        private int SelectLine(PointDec world, float scalef)
        {
            // 画面上でおよそ8ピクセル以内を選択対象にする
            decimal tolerance = 8m / (decimal)scalef;

            int nearestIndex = -1;
            decimal nearestDistance = decimal.MaxValue;

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                LineEntity line = lineManager.decFile[i];

                decimal distance = GetDistanceToLineSegment(
                    world,
                    line.start,
                    line.end
                );

                if (distance <= tolerance &&
                    distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }


        //==============================================
        // 点から線分までの最短距離
        //==============================================
        private decimal GetDistanceToLineSegment(
            PointDec point,
            PointDec lineStart,
            PointDec lineEnd)
        {
            decimal dx = lineEnd.x - lineStart.x;
            decimal dy = lineEnd.y - lineStart.y;

            decimal lengthSquared =
                dx * dx + dy * dy;

            // 長さ0の線
            if (lengthSquared == 0m)
            {
                return GetDistance(point, lineStart);
            }

            decimal t =
                ((point.x - lineStart.x) * dx +
                 (point.y - lineStart.y) * dy)
                / lengthSquared;

            // 線分の始点より外
            if (t < 0m)
                t = 0m;

            // 線分の終点より外
            if (t > 1m)
                t = 1m;

            PointDec nearestPoint = new PointDec(
                lineStart.x + t * dx,
                lineStart.y + t * dy
            );

            return GetDistance(point, nearestPoint);
        }


        private decimal GetDistance(
              PointDec p1,
              PointDec p2)
        {
            double dx = (double)(p1.x - p2.x);
            double dy = (double)(p1.y - p2.y);

            return (decimal)Math.Sqrt(
                dx * dx + dy * dy
            );
        }



    }
}