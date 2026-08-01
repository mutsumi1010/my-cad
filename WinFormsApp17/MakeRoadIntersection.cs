using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp17
{
    public enum IntersectionType
    {
        None,
        Cross,
        THorizontal,
        TVertical,
        LShape
    }
    public class MakeRoadIntersection
    {
        private readonly LineManager lineManager;
        private readonly Dictionary<int, List<LineEntity>> roadVisualMap;
        private readonly Form1 form1;

        public MakeRoadIntersection(
            LineManager lineManager,
            Dictionary<int, List<LineEntity>> roadVisualMap,
            Form1 form1)
        {
            this.lineManager = lineManager;
            this.roadVisualMap = roadVisualMap;
            this.form1 = form1;
        }


        //***********************************
        //************************************************
        //  CROSS
        //************************************************
        //***********************************

        //================================================
        // クロス交差点
        //
        // SIDEラインは、現在できている処理を維持する。
        //
        // X方向OPPOSITE
        // → Y方向SIDEとの交点から外側を残す
        // → OPPOSITE同士の交点から5m突き抜ける線を作る
        // → 2つの交点間は削除する
        //
        // Y方向OPPOSITE
        // → X方向SIDEとの交点から外側を残す
        // → OPPOSITE同士の交点から5m突き抜ける線を作る
        // → 2つの交点間は削除する
        //
        // したがって、各OPPOSITEは2本に分かれる。
        // 元の敷地境界線は変更しない。
        //================================================
        public void MakeCrossIntersection(
            PointDec siteCrossPoint)
        {
            //================================================
            // 敷地交点につながっている道路を2本取得
            //================================================
            List<int> connectedRoadIndexes =
                roadVisualMap.Keys
                    .Where(roadIndex =>
                    {
                        if (roadIndex < 0 ||
                            roadIndex >= lineManager.decFile.Count)
                        {
                            return false;
                        }

                        LineEntity siteBoundary =
                            lineManager.decFile[roadIndex];

                        return
                            IsSamePoint(
                                siteBoundary.start,
                                siteCrossPoint) ||
                            IsSamePoint(
                                siteBoundary.end,
                                siteCrossPoint);
                    })
                    .ToList();

            if (connectedRoadIndexes.Count != 2)
            {
                MessageBox.Show(
                    "この交点につながる道路が2本見つかりません");

                return;
            }

            int roadIndex1 =
                connectedRoadIndexes[0];

            int roadIndex2 =
                connectedRoadIndexes[1];

            LineEntity siteBoundary1 =
                lineManager.decFile[roadIndex1];

            LineEntity siteBoundary2 =
                lineManager.decFile[roadIndex2];

            //================================================
            // roadVisualMapからOPPOSITEラインを取得
            // roadLines[0]をOPPOSITEラインとして扱う
            //================================================
            if (!roadVisualMap.TryGetValue(
                    roadIndex1,
                    out List<LineEntity>? roadLines1) ||
                !roadVisualMap.TryGetValue(
                    roadIndex2,
                    out List<LineEntity>? roadLines2) ||
                roadLines1.Count == 0 ||
                roadLines2.Count == 0)
            {
                MessageBox.Show(
                    "反対側道路境界線が見つかりません");

                return;
            }

            LineEntity oppositeLine1 =
                roadLines1[0];

            LineEntity oppositeLine2 =
                roadLines2[0];

            //================================================
            // 敷地境界線の方向から
            // X方向道路とY方向道路を判定
            //================================================
            decimal dx1 =
                Math.Abs(
                    siteBoundary1.end.x -
                    siteBoundary1.start.x);

            decimal dy1 =
                Math.Abs(
                    siteBoundary1.end.y -
                    siteBoundary1.start.y);

            decimal dx2 =
                Math.Abs(
                    siteBoundary2.end.x -
                    siteBoundary2.start.x);

            decimal dy2 =
                Math.Abs(
                    siteBoundary2.end.y -
                    siteBoundary2.start.y);

            bool line1IsXDirection =
                dx1 >= dy1;

            bool line2IsXDirection =
                dx2 >= dy2;

            if (line1IsXDirection ==
                line2IsXDirection)
            {
                MessageBox.Show(
                    "X方向とY方向の道路を判別できません");

                return;
            }

            //================================================
            // X方向道路とY方向道路へ振り分ける
            //================================================
            int xRoadIndex;
            int yRoadIndex;

            List<LineEntity> xRoadLines;
            List<LineEntity> yRoadLines;

            LineEntity xSiteBoundary;
            LineEntity ySiteBoundary;

            LineEntity xOppositeLine;
            LineEntity yOppositeLine;

            if (line1IsXDirection)
            {
                xRoadIndex =
                    roadIndex1;

                yRoadIndex =
                    roadIndex2;

                xRoadLines =
                    roadLines1;

                yRoadLines =
                    roadLines2;

                xSiteBoundary =
                    siteBoundary1;

                ySiteBoundary =
                    siteBoundary2;

                xOppositeLine =
                    oppositeLine1;

                yOppositeLine =
                    oppositeLine2;
            }
            else
            {
                xRoadIndex =
                    roadIndex2;

                yRoadIndex =
                    roadIndex1;

                xRoadLines =
                    roadLines2;

                yRoadLines =
                    roadLines1;

                xSiteBoundary =
                    siteBoundary2;

                ySiteBoundary =
                    siteBoundary1;

                xOppositeLine =
                    oppositeLine2;

                yOppositeLine =
                    oppositeLine1;
            }

            //================================================
            // 元のOPPOSITEラインのdecFile内位置
            //================================================
            int xOppositeDecFileIndex =
                lineManager.decFile.IndexOf(
                    xOppositeLine);

            int yOppositeDecFileIndex =
                lineManager.decFile.IndexOf(
                    yOppositeLine);

            if (xOppositeDecFileIndex < 0 ||
                yOppositeDecFileIndex < 0)
            {
                MessageBox.Show(
                    "decFile内にOPPOSITEラインが見つかりません");

                return;
            }

            //================================================
            // 横SIDEラインを伸ばす方向
            //
            // X方向敷地境界線が敷地交点から
            // 伸びている方向とは反対側へ伸ばす
            //================================================
            PointDec xFarPoint =
                GetRoadDistanceSquared(
                    xSiteBoundary.start,
                    siteCrossPoint) >=
                GetRoadDistanceSquared(
                    xSiteBoundary.end,
                    siteCrossPoint)
                    ? xSiteBoundary.start
                    : xSiteBoundary.end;

            decimal xDirectionX =
                xFarPoint.x -
                siteCrossPoint.x;

            decimal xDirectionY =
                xFarPoint.y -
                siteCrossPoint.y;

            decimal xDirectionLength =
                (decimal)Math.Sqrt(
                    (double)(
                        xDirectionX * xDirectionX +
                        xDirectionY * xDirectionY));

            if (xDirectionLength == 0)
            {
                MessageBox.Show(
                    "X方向道路の方向を取得できません");

                return;
            }

            decimal xSideUnitX =
                -xDirectionX /
                xDirectionLength;

            decimal xSideUnitY =
                -xDirectionY /
                xDirectionLength;

            //================================================
            // 縦SIDEラインを伸ばす方向
            //
            // Y方向敷地境界線が敷地交点から
            // 伸びている方向とは反対側へ伸ばす
            //================================================
            PointDec yFarPoint =
                GetRoadDistanceSquared(
                    ySiteBoundary.start,
                    siteCrossPoint) >=
                GetRoadDistanceSquared(
                    ySiteBoundary.end,
                    siteCrossPoint)
                    ? ySiteBoundary.start
                    : ySiteBoundary.end;

            decimal yDirectionX =
                yFarPoint.x -
                siteCrossPoint.x;

            decimal yDirectionY =
                yFarPoint.y -
                siteCrossPoint.y;

            decimal yDirectionLength =
                (decimal)Math.Sqrt(
                    (double)(
                        yDirectionX * yDirectionX +
                        yDirectionY * yDirectionY));

            if (yDirectionLength == 0)
            {
                MessageBox.Show(
                    "Y方向道路の方向を取得できません");

                return;
            }

            decimal ySideUnitX =
                -yDirectionX /
                yDirectionLength;

            decimal ySideUnitY =
                -yDirectionY /
                yDirectionLength;

            //================================================
            // 交点計算用の仮横SIDEライン
            //================================================
            LineEntity temporaryXSideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = siteCrossPoint.x,
                            y = siteCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x =
                                siteCrossPoint.x +
                                xSideUnitX * 10000m,

                            y =
                                siteCrossPoint.y +
                                xSideUnitY * 10000m
                        },

                    Layer = 2
                };

            //================================================
            // 交点計算用の仮縦SIDEライン
            //================================================
            LineEntity temporaryYSideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = siteCrossPoint.x,
                            y = siteCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x =
                                siteCrossPoint.x +
                                ySideUnitX * 10000m,

                            y =
                                siteCrossPoint.y +
                                ySideUnitY * 10000m
                        },

                    Layer = 2
                };

            //================================================
            // 横SIDEラインとY方向OPPOSITEとの交点
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    temporaryXSideLine,
                    yOppositeLine,
                    out PointDec xSideCrossPoint))
            {
                MessageBox.Show(
                    "横SIDEラインとY方向OPPOSITEの交点を取得できません");

                return;
            }

            //================================================
            // 縦SIDEラインとX方向OPPOSITEとの交点
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    temporaryYSideLine,
                    xOppositeLine,
                    out PointDec ySideCrossPoint))
            {
                MessageBox.Show(
                    "縦SIDEラインとX方向OPPOSITEの交点を取得できません");

                return;
            }

            //================================================
            // X方向OPPOSITEとY方向OPPOSITEとの交点
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    xOppositeLine,
                    yOppositeLine,
                    out PointDec oppositeCrossPoint))
            {
                MessageBox.Show(
                    "OPPOSITEライン同士の交点を取得できません");

                return;
            }

            //================================================
            // 完成済みの横SIDEライン
            // Y方向OPPOSITEを5m突き抜ける
            //================================================
            PointDec xSidePastPoint =
                new PointDec
                {
                    x =
                        xSideCrossPoint.x +
                        xSideUnitX * 5000m,

                    y =
                        xSideCrossPoint.y +
                        xSideUnitY * 5000m
                };

            LineEntity corneredXSideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = xSideCrossPoint.x,
                            y = xSideCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = xSidePastPoint.x,
                            y = xSidePastPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // 完成済みの縦SIDEライン
            // X方向OPPOSITEを5m突き抜ける
            //================================================
            PointDec ySidePastPoint =
                new PointDec
                {
                    x =
                        ySideCrossPoint.x +
                        ySideUnitX * 5000m,

                    y =
                        ySideCrossPoint.y +
                        ySideUnitY * 5000m
                };

            LineEntity corneredYSideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = ySideCrossPoint.x,
                            y = ySideCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = ySidePastPoint.x,
                            y = ySidePastPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // X方向OPPOSITEの外側端点を取得
            //
            // OPPOSITE同士の交点から遠い端点を残す
            //================================================
            PointDec xOuterPoint =
                GetRoadDistanceSquared(
                    xOppositeLine.start,
                    oppositeCrossPoint) >=
                GetRoadDistanceSquared(
                    xOppositeLine.end,
                    oppositeCrossPoint)
                    ? xOppositeLine.start
                    : xOppositeLine.end;

            //================================================
            // Y方向OPPOSITEの外側端点を取得
            //================================================
            PointDec yOuterPoint =
                GetRoadDistanceSquared(
                    yOppositeLine.start,
                    oppositeCrossPoint) >=
                GetRoadDistanceSquared(
                    yOppositeLine.end,
                    oppositeCrossPoint)
                    ? yOppositeLine.start
                    : yOppositeLine.end;

            //================================================
            // X方向OPPOSITE・外側部分
            //
            // 縦SIDEとの交点から外側だけを残す。
            // 縦SIDEとの交点からOPPOSITE同士の交点までは
            // 線を作らない。
            //================================================
            LineEntity outerXOppositeLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = xOuterPoint.x,
                            y = xOuterPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = ySideCrossPoint.x,
                            y = ySideCrossPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // Y方向OPPOSITE・外側部分
            //
            // 横SIDEとの交点から外側だけを残す。
            // 横SIDEとの交点からOPPOSITE同士の交点までは
            // 線を作らない。
            //================================================
            LineEntity outerYOppositeLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = yOuterPoint.x,
                            y = yOuterPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = xSideCrossPoint.x,
                            y = xSideCrossPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // X方向OPPOSITEを交点から5m突き抜ける方向
            //
            // 外側端点からOPPOSITE交点へ向かい、
            // そのまま交点を越える方向
            //================================================
            decimal xOppositeDirectionX =
                oppositeCrossPoint.x -
                xOuterPoint.x;

            decimal xOppositeDirectionY =
                oppositeCrossPoint.y -
                xOuterPoint.y;

            decimal xOppositeDirectionLength =
                (decimal)Math.Sqrt(
                    (double)(
                        xOppositeDirectionX *
                        xOppositeDirectionX +

                        xOppositeDirectionY *
                        xOppositeDirectionY));

            if (xOppositeDirectionLength == 0)
            {
                MessageBox.Show(
                    "X方向OPPOSITEの延長方向を取得できません");

                return;
            }

            decimal xOppositeUnitX =
                xOppositeDirectionX /
                xOppositeDirectionLength;

            decimal xOppositeUnitY =
                xOppositeDirectionY /
                xOppositeDirectionLength;

            PointDec xOppositePastPoint =
                new PointDec
                {
                    x =
                        oppositeCrossPoint.x +
                        xOppositeUnitX * 5000m,

                    y =
                        oppositeCrossPoint.y +
                        xOppositeUnitY * 5000m
                };

            //================================================
            // X方向OPPOSITE・交差部分
            //
            // OPPOSITE同士の交点から5m先まで
            //================================================
            LineEntity crossingXOppositeLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = oppositeCrossPoint.x,
                            y = oppositeCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = xOppositePastPoint.x,
                            y = xOppositePastPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // Y方向OPPOSITEを交点から5m突き抜ける方向
            //================================================
            decimal yOppositeDirectionX =
                oppositeCrossPoint.x -
                yOuterPoint.x;

            decimal yOppositeDirectionY =
                oppositeCrossPoint.y -
                yOuterPoint.y;

            decimal yOppositeDirectionLength =
                (decimal)Math.Sqrt(
                    (double)(
                        yOppositeDirectionX *
                        yOppositeDirectionX +

                        yOppositeDirectionY *
                        yOppositeDirectionY));

            if (yOppositeDirectionLength == 0)
            {
                MessageBox.Show(
                    "Y方向OPPOSITEの延長方向を取得できません");

                return;
            }

            decimal yOppositeUnitX =
                yOppositeDirectionX /
                yOppositeDirectionLength;

            decimal yOppositeUnitY =
                yOppositeDirectionY /
                yOppositeDirectionLength;

            PointDec yOppositePastPoint =
                new PointDec
                {
                    x =
                        oppositeCrossPoint.x +
                        yOppositeUnitX * 5000m,

                    y =
                        oppositeCrossPoint.y +
                        yOppositeUnitY * 5000m
                };

            //================================================
            // Y方向OPPOSITE・交差部分
            //
            // OPPOSITE同士の交点から5m先まで
            //================================================
            LineEntity crossingYOppositeLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = oppositeCrossPoint.x,
                            y = oppositeCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = yOppositePastPoint.x,
                            y = yOppositePastPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // roadVisualMapを更新
            //
            // 各道路の[0]を外側OPPOSITEへ置換し、
            // 交差部分のOPPOSITEを追加する。
            // SIDEラインも現在どおり追加する。
            //================================================
            xRoadLines[0] =
                outerXOppositeLine;

            xRoadLines.Add(
                crossingXOppositeLine);

            xRoadLines.Add(
                corneredXSideLine);

            yRoadLines[0] =
                outerYOppositeLine;

            yRoadLines.Add(
                crossingYOppositeLine);

            yRoadLines.Add(
                corneredYSideLine);

            roadVisualMap[xRoadIndex] =
                xRoadLines;

            roadVisualMap[yRoadIndex] =
                yRoadLines;

            //================================================
            // decFileを更新
            //
            // 元のOPPOSITEを外側部分へ置換し、
            // 交差部分を新しい線として追加する。
            //================================================
            lineManager.decFile[xOppositeDecFileIndex] =
                outerXOppositeLine;

            lineManager.decFile[yOppositeDecFileIndex] =
                outerYOppositeLine;

            lineManager.decFile.Add(
                crossingXOppositeLine);

            lineManager.decFile.Add(
                crossingYOppositeLine);

            lineManager.decFile.Add(
                corneredXSideLine);

            lineManager.decFile.Add(
                corneredYSideLine);

            form1.Invalidate();
        }





        //***********************************
        //***********************************
        //  T Horizontal
        //***********************************
        //***********************************

        //================================================
        // T型交差点・横通し
        //
        // X方向道路を通し側とする。
        //
        // 新しいSIDEライン
        // → 敷地交点から作成
        // → Y方向OPPOSITEを5m突き抜ける
        // → 交点から5m突き抜けた側を残す
        //
        // Y方向OPPOSITE
        // → X方向OPPOSITEとの交点に近い側を削除
        // → 新しいSIDEラインとの交点から
        //    遠い側だけを残す
        //
        // 元の敷地境界線は変更しない
        //================================================
        public void MakeTHorizontalIntersection(
            PointDec siteCrossPoint)
        {
            //================================================
            // 敷地交点につながっている道路を取得
            //================================================
            List<int> connectedRoadIndexes =
                roadVisualMap.Keys
                    .Where(roadIndex =>
                    {
                        if (roadIndex < 0 ||
                            roadIndex >= lineManager.decFile.Count)
                        {
                            return false;
                        }

                        LineEntity siteBoundary =
                            lineManager.decFile[roadIndex];

                        return
                            IsSamePoint(
                                siteBoundary.start,
                                siteCrossPoint) ||
                            IsSamePoint(
                                siteBoundary.end,
                                siteCrossPoint);
                    })
                    .ToList();

            if (connectedRoadIndexes.Count != 2)
            {
                MessageBox.Show(
                    "この交点につながる道路が2本見つかりません");

                return;
            }

            int roadIndex1 =
                connectedRoadIndexes[0];

            int roadIndex2 =
                connectedRoadIndexes[1];

            LineEntity siteBoundary1 =
                lineManager.decFile[roadIndex1];

            LineEntity siteBoundary2 =
                lineManager.decFile[roadIndex2];

            //================================================
            // roadVisualMapからOPPOSITEラインを取得
            // roadLines[0]をOPPOSITEラインとして扱う
            //================================================
            if (!roadVisualMap.TryGetValue(
                    roadIndex1,
                    out List<LineEntity>? roadLines1) ||
                !roadVisualMap.TryGetValue(
                    roadIndex2,
                    out List<LineEntity>? roadLines2) ||
                roadLines1.Count == 0 ||
                roadLines2.Count == 0)
            {
                MessageBox.Show(
                    "反対側道路境界線が見つかりません");

                return;
            }

            LineEntity oppositeLine1 =
                roadLines1[0];

            LineEntity oppositeLine2 =
                roadLines2[0];

            //================================================
            // 敷地境界線の方向から
            // X方向道路とY方向道路を判定
            //================================================
            decimal dx1 =
                Math.Abs(
                    siteBoundary1.end.x -
                    siteBoundary1.start.x);

            decimal dy1 =
                Math.Abs(
                    siteBoundary1.end.y -
                    siteBoundary1.start.y);

            decimal dx2 =
                Math.Abs(
                    siteBoundary2.end.x -
                    siteBoundary2.start.x);

            decimal dy2 =
                Math.Abs(
                    siteBoundary2.end.y -
                    siteBoundary2.start.y);

            bool line1IsXDirection =
                dx1 >= dy1;

            bool line2IsXDirection =
                dx2 >= dy2;

            if (line1IsXDirection ==
                line2IsXDirection)
            {
                MessageBox.Show(
                    "X方向とY方向の道路を判別できません");

                return;
            }

            //================================================
            // X方向道路とY方向道路へ振り分ける
            //================================================
            int xRoadIndex;
            int yRoadIndex;

            List<LineEntity> xRoadLines;
            List<LineEntity> yRoadLines;

            LineEntity xSiteBoundary;
            LineEntity xOppositeLine;
            LineEntity yOppositeLine;

            if (line1IsXDirection)
            {
                xRoadIndex =
                    roadIndex1;

                yRoadIndex =
                    roadIndex2;

                xRoadLines =
                    roadLines1;

                yRoadLines =
                    roadLines2;

                xSiteBoundary =
                    siteBoundary1;

                xOppositeLine =
                    oppositeLine1;

                yOppositeLine =
                    oppositeLine2;
            }
            else
            {
                xRoadIndex =
                    roadIndex2;

                yRoadIndex =
                    roadIndex1;

                xRoadLines =
                    roadLines2;

                yRoadLines =
                    roadLines1;

                xSiteBoundary =
                    siteBoundary2;

                xOppositeLine =
                    oppositeLine2;

                yOppositeLine =
                    oppositeLine1;
            }

            //================================================
            // OPPOSITEラインのdecFile内の位置
            //================================================
            int xOppositeDecFileIndex =
                lineManager.decFile.IndexOf(
                    xOppositeLine);

            int yOppositeDecFileIndex =
                lineManager.decFile.IndexOf(
                    yOppositeLine);

            if (xOppositeDecFileIndex < 0 ||
                yOppositeDecFileIndex < 0)
            {
                MessageBox.Show(
                    "decFile内にOPPOSITEラインが見つかりません");

                return;
            }

            //================================================
            // 新しいSIDEラインを伸ばす方向を求める
            //
            // X方向敷地境界線が敷地交点から
            // 伸びている方向とは反対側へ伸ばす
            //================================================
            decimal startDistance =
                GetRoadDistanceSquared(
                    xSiteBoundary.start,
                    siteCrossPoint);

            decimal endDistance =
                GetRoadDistanceSquared(
                    xSiteBoundary.end,
                    siteCrossPoint);

            PointDec xFarPoint =
                startDistance >= endDistance
                    ? xSiteBoundary.start
                    : xSiteBoundary.end;

            decimal existingDirectionX =
                xFarPoint.x -
                siteCrossPoint.x;

            decimal existingDirectionY =
                xFarPoint.y -
                siteCrossPoint.y;

            decimal existingLength =
                (decimal)Math.Sqrt(
                    (double)(
                        existingDirectionX *
                        existingDirectionX +

                        existingDirectionY *
                        existingDirectionY));

            if (existingLength == 0)
            {
                MessageBox.Show(
                    "X方向道路の方向を取得できません");

                return;
            }

            // 既存線とは反対方向
            decimal sideUnitX =
                -existingDirectionX /
                existingLength;

            decimal sideUnitY =
                -existingDirectionY /
                existingLength;

            //================================================
            // 交点計算用の仮SIDEライン
            //================================================
            LineEntity temporarySideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = siteCrossPoint.x,
                            y = siteCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x =
                                siteCrossPoint.x +
                                sideUnitX * 10000m,

                            y =
                                siteCrossPoint.y +
                                sideUnitY * 10000m
                        },

                    Layer = 2
                };

            //================================================
            // 新しいSIDEラインと
            // Y方向OPPOSITEとの交点
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    temporarySideLine,
                    yOppositeLine,
                    out PointDec sideCrossPoint))
            {
                MessageBox.Show(
                    "SIDEラインとY方向OPPOSITEの交点を取得できません");

                return;
            }

            //================================================
            // SIDEラインが交点から5m突き抜けた点
            //================================================
            PointDec sidePastPoint =
                new PointDec
                {
                    x =
                        sideCrossPoint.x +
                        sideUnitX * 5000m,

                    y =
                        sideCrossPoint.y +
                        sideUnitY * 5000m
                };

            //================================================
            // 新しいSIDEライン
            //
            // 残すのは、
            // 交点から5m突き抜けた側
            //
            // 敷地交点側は残さない
            //================================================
            LineEntity corneredSideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = sideCrossPoint.x,
                            y = sideCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = sidePastPoint.x,
                            y = sidePastPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // X方向OPPOSITEと
            // Y方向OPPOSITEとの交点
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    xOppositeLine,
                    yOppositeLine,
                    out PointDec oppositeCrossPoint))
            {
                MessageBox.Show(
                    "OPPOSITEライン同士の交点を取得できません");

                return;
            }

            //================================================
            // X方向OPPOSITEを、
            // Y方向OPPOSITEとの交点から
            // さらに5m先まで突き抜けさせる
            //================================================
            LineEntity extendedXOppositeLine =
                ExtendRoadLinePastIntersection(
                    xOppositeLine,
                    siteCrossPoint,
                    oppositeCrossPoint,
                    5000m);

            //================================================
            // Y方向OPPOSITEをコーナー処理
            //
            // X方向OPPOSITEとの交点に近い側を削除する。
            //
            // 新しいSIDEラインとの交点から、
            // oppositeCrossPointとは反対側へ伸びている
            // 元の端点を残す
            //================================================
            LineEntity corneredYOppositeLine =
                KeepRoadLineSideAwayFromPoint(
                    yOppositeLine,
                    sideCrossPoint,
                    oppositeCrossPoint);

            //================================================
            // roadVisualMapを更新
            //================================================
            xRoadLines[0] =
                extendedXOppositeLine;

            // 新しく作成したSIDEラインだけ追加
            xRoadLines.Add(
                corneredSideLine);

            yRoadLines[0] =
                corneredYOppositeLine;

            roadVisualMap[xRoadIndex] =
                xRoadLines;

            roadVisualMap[yRoadIndex] =
                yRoadLines;

            //================================================
            // decFileを更新
            //
            // 元の敷地境界線は変更しない
            //================================================
            lineManager.decFile[xOppositeDecFileIndex] =
                extendedXOppositeLine;

            lineManager.decFile[yOppositeDecFileIndex] =
                corneredYOppositeLine;

            lineManager.decFile.Add(
                corneredSideLine);

            form1.Invalidate();
        }


        //================================================
        // 敷地交点に近い端点を、
        // 交点から指定距離だけ先まで延長する
        //================================================
        private LineEntity ExtendRoadLinePastIntersection(
            LineEntity line,
            PointDec siteCrossPoint,
            PointDec intersectionPoint,
            decimal extensionLength)
        {
            decimal startDistance =
                GetRoadDistanceSquared(
                    line.start,
                    siteCrossPoint);

            decimal endDistance =
                GetRoadDistanceSquared(
                    line.end,
                    siteCrossPoint);

            bool startIsNear =
                startDistance <= endDistance;

            PointDec farPoint =
                startIsNear
                    ? line.end
                    : line.start;

            decimal dx =
                intersectionPoint.x -
                farPoint.x;

            decimal dy =
                intersectionPoint.y -
                farPoint.y;

            decimal length =
                (decimal)Math.Sqrt(
                    (double)(
                        dx * dx +
                        dy * dy));

            if (length == 0)
            {
                return line;
            }

            decimal unitX =
                dx / length;

            decimal unitY =
                dy / length;

            PointDec extendedPoint =
                new PointDec
                {
                    x =
                        intersectionPoint.x +
                        unitX * extensionLength,

                    y =
                        intersectionPoint.y +
                        unitY * extensionLength
                };

            if (startIsNear)
            {
                line.start =
                    extendedPoint;
            }
            else
            {
                line.end =
                    extendedPoint;
            }

            return line;
        }


        //================================================
        // trimPointで線を切る
        //
        // deleteSidePointに近い側を削除し、
        // deleteSidePointから遠い側を残す
        //================================================
        private LineEntity KeepRoadLineSideAwayFromPoint(
            LineEntity line,
            PointDec trimPoint,
            PointDec deleteSidePoint)
        {
            decimal startDistanceToDeleteSide =
                GetRoadDistanceSquared(
                    line.start,
                    deleteSidePoint);

            decimal endDistanceToDeleteSide =
                GetRoadDistanceSquared(
                    line.end,
                    deleteSidePoint);

            // start側のほうが削除側に近い
            // → start側を消してtrimPointへ移動
            // → end側を残す
            if (startDistanceToDeleteSide <=
                endDistanceToDeleteSide)
            {
                line.start =
                    new PointDec
                    {
                        x = trimPoint.x,
                        y = trimPoint.y
                    };
            }
            // end側のほうが削除側に近い
            // → end側を消してtrimPointへ移動
            // → start側を残す
            else
            {
                line.end =
                    new PointDec
                    {
                        x = trimPoint.x,
                        y = trimPoint.y
                    };
            }

            return line;
        }


        //================================================
        // 2点間距離の二乗
        //================================================
        private decimal GetRoadDistanceSquared(
            PointDec point1,
            PointDec point2)
        {
            decimal dx =
                point1.x -
                point2.x;

            decimal dy =
                point1.y -
                point2.y;

            return
                dx * dx +
                dy * dy;
        }




        //********************************************
        //********************************************
        //  T たて通し
        //********************************************
        //********************************************
        //================================================
        // T型交差点・縦通し
        //
        // Y方向道路を通し側とする。
        //
        // 新しいSIDEライン
        // → 敷地交点からY方向へ作成
        // → X方向OPPOSITEを5m突き抜ける
        // → 交点から5m突き抜けた側を残す
        //
        // X方向OPPOSITE
        // → Y方向OPPOSITEとの交点に近い側を削除
        // → 新しいSIDEラインとの交点から
        //    遠い側だけを残す
        //
        // 元の敷地境界線は変更しない
        //================================================
        public void MakeTVerticalIntersection(
            PointDec siteCrossPoint)
        {
            //================================================
            // 敷地交点につながっている道路を取得
            //================================================
            List<int> connectedRoadIndexes =
                roadVisualMap.Keys
                    .Where(roadIndex =>
                    {
                        if (roadIndex < 0 ||
                            roadIndex >= lineManager.decFile.Count)
                        {
                            return false;
                        }

                        LineEntity siteBoundary =
                            lineManager.decFile[roadIndex];

                        return
                            IsSamePoint(
                                siteBoundary.start,
                                siteCrossPoint) ||
                            IsSamePoint(
                                siteBoundary.end,
                                siteCrossPoint);
                    })
                    .ToList();

            if (connectedRoadIndexes.Count != 2)
            {
                MessageBox.Show(
                    "この交点につながる道路が2本見つかりません");

                return;
            }

            int roadIndex1 =
                connectedRoadIndexes[0];

            int roadIndex2 =
                connectedRoadIndexes[1];

            LineEntity siteBoundary1 =
                lineManager.decFile[roadIndex1];

            LineEntity siteBoundary2 =
                lineManager.decFile[roadIndex2];

            //================================================
            // roadVisualMapからOPPOSITEラインを取得
            // roadLines[0]をOPPOSITEラインとして扱う
            //================================================
            if (!roadVisualMap.TryGetValue(
                    roadIndex1,
                    out List<LineEntity>? roadLines1) ||
                !roadVisualMap.TryGetValue(
                    roadIndex2,
                    out List<LineEntity>? roadLines2) ||
                roadLines1.Count == 0 ||
                roadLines2.Count == 0)
            {
                MessageBox.Show(
                    "反対側道路境界線が見つかりません");

                return;
            }

            LineEntity oppositeLine1 =
                roadLines1[0];

            LineEntity oppositeLine2 =
                roadLines2[0];

            //================================================
            // 敷地境界線の方向から
            // X方向道路とY方向道路を判定
            //================================================
            decimal dx1 =
                Math.Abs(
                    siteBoundary1.end.x -
                    siteBoundary1.start.x);

            decimal dy1 =
                Math.Abs(
                    siteBoundary1.end.y -
                    siteBoundary1.start.y);

            decimal dx2 =
                Math.Abs(
                    siteBoundary2.end.x -
                    siteBoundary2.start.x);

            decimal dy2 =
                Math.Abs(
                    siteBoundary2.end.y -
                    siteBoundary2.start.y);

            bool line1IsXDirection =
                dx1 >= dy1;

            bool line2IsXDirection =
                dx2 >= dy2;

            if (line1IsXDirection ==
                line2IsXDirection)
            {
                MessageBox.Show(
                    "X方向とY方向の道路を判別できません");

                return;
            }

            //================================================
            // X方向道路とY方向道路へ振り分ける
            //================================================
            int xRoadIndex;
            int yRoadIndex;

            List<LineEntity> xRoadLines;
            List<LineEntity> yRoadLines;

            LineEntity ySiteBoundary;
            LineEntity xOppositeLine;
            LineEntity yOppositeLine;

            if (line1IsXDirection)
            {
                xRoadIndex =
                    roadIndex1;

                yRoadIndex =
                    roadIndex2;

                xRoadLines =
                    roadLines1;

                yRoadLines =
                    roadLines2;

                ySiteBoundary =
                    siteBoundary2;

                xOppositeLine =
                    oppositeLine1;

                yOppositeLine =
                    oppositeLine2;
            }
            else
            {
                xRoadIndex =
                    roadIndex2;

                yRoadIndex =
                    roadIndex1;

                xRoadLines =
                    roadLines2;

                yRoadLines =
                    roadLines1;

                ySiteBoundary =
                    siteBoundary1;

                xOppositeLine =
                    oppositeLine2;

                yOppositeLine =
                    oppositeLine1;
            }

            //================================================
            // OPPOSITEラインのdecFile内の位置
            //================================================
            int xOppositeDecFileIndex =
                lineManager.decFile.IndexOf(
                    xOppositeLine);

            int yOppositeDecFileIndex =
                lineManager.decFile.IndexOf(
                    yOppositeLine);

            if (xOppositeDecFileIndex < 0 ||
                yOppositeDecFileIndex < 0)
            {
                MessageBox.Show(
                    "decFile内にOPPOSITEラインが見つかりません");

                return;
            }

            //================================================
            // 新しいSIDEラインを伸ばす方向を求める
            //
            // Y方向敷地境界線が敷地交点から
            // 伸びている方向とは反対側へ伸ばす
            //================================================
            decimal startDistance =
                GetRoadDistanceSquared(
                    ySiteBoundary.start,
                    siteCrossPoint);

            decimal endDistance =
                GetRoadDistanceSquared(
                    ySiteBoundary.end,
                    siteCrossPoint);

            PointDec yFarPoint =
                startDistance >= endDistance
                    ? ySiteBoundary.start
                    : ySiteBoundary.end;

            decimal existingDirectionX =
                yFarPoint.x -
                siteCrossPoint.x;

            decimal existingDirectionY =
                yFarPoint.y -
                siteCrossPoint.y;

            decimal existingLength =
                (decimal)Math.Sqrt(
                    (double)(
                        existingDirectionX *
                        existingDirectionX +

                        existingDirectionY *
                        existingDirectionY));

            if (existingLength == 0)
            {
                MessageBox.Show(
                    "Y方向道路の方向を取得できません");

                return;
            }

            // 既存線とは反対方向
            decimal sideUnitX =
                -existingDirectionX /
                existingLength;

            decimal sideUnitY =
                -existingDirectionY /
                existingLength;

            //================================================
            // 交点計算用の仮SIDEライン
            //================================================
            LineEntity temporarySideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = siteCrossPoint.x,
                            y = siteCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x =
                                siteCrossPoint.x +
                                sideUnitX * 10000m,

                            y =
                                siteCrossPoint.y +
                                sideUnitY * 10000m
                        },

                    Layer = 2
                };

            //================================================
            // 新しいSIDEラインと
            // X方向OPPOSITEとの交点
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    temporarySideLine,
                    xOppositeLine,
                    out PointDec sideCrossPoint))
            {
                MessageBox.Show(
                    "SIDEラインとX方向OPPOSITEの交点を取得できません");

                return;
            }

            //================================================
            // SIDEラインが交点から5m突き抜けた点
            //================================================
            PointDec sidePastPoint =
                new PointDec
                {
                    x =
                        sideCrossPoint.x +
                        sideUnitX * 5000m,

                    y =
                        sideCrossPoint.y +
                        sideUnitY * 5000m
                };

            //================================================
            // 新しいSIDEライン
            //
            // 残すのは、
            // 交点から5m突き抜けた側
            //
            // 敷地交点側は残さない
            //================================================
            LineEntity corneredSideLine =
                new LineEntity
                {
                    start =
                        new PointDec
                        {
                            x = sideCrossPoint.x,
                            y = sideCrossPoint.y
                        },

                    end =
                        new PointDec
                        {
                            x = sidePastPoint.x,
                            y = sidePastPoint.y
                        },

                    Layer = 2
                };

            //================================================
            // X方向OPPOSITEと
            // Y方向OPPOSITEとの交点
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    xOppositeLine,
                    yOppositeLine,
                    out PointDec oppositeCrossPoint))
            {
                MessageBox.Show(
                    "OPPOSITEライン同士の交点を取得できません");

                return;
            }

            //================================================
            // Y方向OPPOSITEを、
            // X方向OPPOSITEとの交点から
            // さらに5m先まで突き抜けさせる
            //================================================
            LineEntity extendedYOppositeLine =
                ExtendRoadLinePastIntersection(
                    yOppositeLine,
                    siteCrossPoint,
                    oppositeCrossPoint,
                    5000m);

            //================================================
            // X方向OPPOSITEをコーナー処理
            //
            // Y方向OPPOSITEとの交点に近い側を削除する。
            //
            // 新しいSIDEラインとの交点から、
            // oppositeCrossPointとは反対側へ伸びている
            // 元の端点を残す
            //================================================
            LineEntity corneredXOppositeLine =
                KeepRoadLineSideAwayFromPoint(
                    xOppositeLine,
                    sideCrossPoint,
                    oppositeCrossPoint);

            //================================================
            // roadVisualMapを更新
            //================================================
            xRoadLines[0] =
                corneredXOppositeLine;

            yRoadLines[0] =
                extendedYOppositeLine;

            // 新しく作成したSIDEラインだけ追加
            yRoadLines.Add(
                corneredSideLine);

            roadVisualMap[xRoadIndex] =
                xRoadLines;

            roadVisualMap[yRoadIndex] =
                yRoadLines;

            //================================================
            // decFileを更新
            //
            // 元の敷地境界線は変更しない
            //================================================
            lineManager.decFile[xOppositeDecFileIndex] =
                corneredXOppositeLine;

            lineManager.decFile[yOppositeDecFileIndex] =
                extendedYOppositeLine;

            lineManager.decFile.Add(
                corneredSideLine);

            form1.Invalidate();
        }









        //********************************************
        //********************************************
        //  L Shape
        //********************************************
        //********************************************
        //================================================
        // L型交差点を作る
        // OPPOSITEライン2本の近い側の端点を交点へ移動し、
        // roadVisualMapとdecFileの元の位置へ上書きする
        //================================================
        public void MakeLshapeIntersection(
            PointDec siteCrossPoint)
        {
            //================================================
            // クリックした敷地交点につながる道路境界線を取得
            // roadVisualMapのキーは、
            // 元の道路境界線があるdecFileのインデックス
            //================================================
            List<int> connectedRoadIndexes =
                roadVisualMap.Keys
                    .Where(roadIndex =>
                    {
                        if (roadIndex < 0 ||
                            roadIndex >= lineManager.decFile.Count)
                        {
                            return false;
                        }

                        LineEntity boundary =
                            lineManager.decFile[roadIndex];

                        return
                            IsSamePoint(
                                boundary.start,
                                siteCrossPoint) ||
                            IsSamePoint(
                                boundary.end,
                                siteCrossPoint);
                    })
                    .ToList();

            if (connectedRoadIndexes.Count != 2)
            {
                MessageBox.Show(
                    "この交点につながる道路が2本見つかりません");

                return;
            }

            int roadIndex1 = connectedRoadIndexes[0];
            int roadIndex2 = connectedRoadIndexes[1];

            //================================================
            // 各道路境界線に対応する道路表示線を取得
            // 値の先頭[0]をOPPOSITEラインとして扱う
            //================================================
            if (!roadVisualMap.TryGetValue(
                    roadIndex1,
                    out List<LineEntity>? roadLines1) ||
                !roadVisualMap.TryGetValue(
                    roadIndex2,
                    out List<LineEntity>? roadLines2) ||
                roadLines1.Count == 0 ||
                roadLines2.Count == 0)
            {
                MessageBox.Show(
                    "反対側道路境界線が見つかりません");

                return;
            }

            LineEntity originalOppositeLine1 =
                roadLines1[0];

            LineEntity originalOppositeLine2 =
                roadLines2[0];

            //================================================
            // 線を変更する前に、
            // decFile内の元の保存位置を取得する
            //================================================
            int decFileIndex1 =
                lineManager.decFile.IndexOf(
                    originalOppositeLine1);

            int decFileIndex2 =
                lineManager.decFile.IndexOf(
                    originalOppositeLine2);

            if (decFileIndex1 < 0 ||
                decFileIndex2 < 0)
            {
                MessageBox.Show(
                    "decFile内に元の反対側道路境界線が見つかりません");

                return;
            }

            //================================================
            // OPPOSITEライン2本の無限直線上の交点を求める
            //================================================
            if (!TryGetInfiniteLineIntersection(
                    originalOppositeLine1,
                    originalOppositeLine2,
                    out PointDec oppositeCrossPoint))
            {
                MessageBox.Show(
                    "反対側道路境界線が平行です");

                return;
            }

            //================================================
            // 敷地交点に近い側の端点を、
            // OPPOSITEライン同士の交点へ移動する
            //================================================
            LineEntity modifiedOppositeLine1 =
                MoveNearEndpointToIntersection(
                    originalOppositeLine1,
                    siteCrossPoint,
                    oppositeCrossPoint);

            LineEntity modifiedOppositeLine2 =
                MoveNearEndpointToIntersection(
                    originalOppositeLine2,
                    siteCrossPoint,
                    oppositeCrossPoint);

            //================================================
            // roadVisualMapの同じ値の位置へ上書き
            //================================================
            roadLines1[0] = modifiedOppositeLine1;
            roadLines2[0] = modifiedOppositeLine2;

            roadVisualMap[roadIndex1] = roadLines1;
            roadVisualMap[roadIndex2] = roadLines2;

            //================================================
            // decFile内の元のOPPOSITEラインの位置へ上書き
            // Addはしない
            //================================================
            lineManager.decFile[decFileIndex1] =
                modifiedOppositeLine1;

            lineManager.decFile[decFileIndex2] =
                modifiedOppositeLine2;

            //================================================
            // 最終結果を再描画
            //================================================
            form1.Invalidate();
        }


        //================================================
        // 敷地交点に近い側の端点を指定交点へ移動し、
        // 修正後のLineEntityを返す
        // class・structのどちらでも書き戻せる形
        //================================================
        private LineEntity MoveNearEndpointToIntersection(
            LineEntity line,
            PointDec siteCrossPoint,
            PointDec oppositeCrossPoint)
        {
            decimal startDistance =
                GetDistanceSquared(
                    line.start,
                    siteCrossPoint);

            decimal endDistance =
                GetDistanceSquared(
                    line.end,
                    siteCrossPoint);

            if (startDistance <= endDistance)
            {
                line.start = oppositeCrossPoint;
            }
            else
            {
                line.end = oppositeCrossPoint;
            }

            return line;
        }



        //********************************************
        //********************************************
        //  Hepl Method
        //********************************************
        //********************************************
  
        //================================================
        // 2点が同じ位置か判定する
        //================================================
        private bool IsSamePoint(
            PointDec point1,
            PointDec point2)
        {
            const decimal tolerance =
                0.001m;

            return
                Math.Abs(point1.x - point2.x) <= tolerance &&
                Math.Abs(point1.y - point2.y) <= tolerance;
        }


        //================================================
        // 2本の無限直線の交点を取得する
        //================================================
        private bool TryGetInfiniteLineIntersection(
            LineEntity line1,
            LineEntity line2,
            out PointDec intersectionPoint)
        {
            decimal x1 = line1.start.x;
            decimal y1 = line1.start.y;
            decimal x2 = line1.end.x;
            decimal y2 = line1.end.y;

            decimal x3 = line2.start.x;
            decimal y3 = line2.start.y;
            decimal x4 = line2.end.x;
            decimal y4 = line2.end.y;

            decimal denominator =
                (x1 - x2) * (y3 - y4) -
                (y1 - y2) * (x3 - x4);

            if (Math.Abs(denominator) < 0.000001m)
            {
                intersectionPoint =
                    new PointDec
                    {
                        x = 0,
                        y = 0
                    };

                return false;
            }

            decimal determinant1 =
                x1 * y2 -
                y1 * x2;

            decimal determinant2 =
                x3 * y4 -
                y3 * x4;

            intersectionPoint =
                new PointDec
                {
                    x =
                        (
                            determinant1 * (x3 - x4) -
                            (x1 - x2) * determinant2
                        ) / denominator,

                    y =
                        (
                            determinant1 * (y3 - y4) -
                            (y1 - y2) * determinant2
                        ) / denominator
                };

            return true;
        }


        //================================================
        // 2点間距離の二乗
        //================================================
        private decimal GetDistanceSquared(
            PointDec point1,
            PointDec point2)
        {
            decimal dx =
                point1.x -
                point2.x;

            decimal dy =
                point1.y -
                point2.y;

            return
                dx * dx +
                dy * dy;
        }




    }
}
