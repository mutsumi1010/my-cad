using WinFormsApp17;
using System.Diagnostics;

namespace WinFormsApp17
{
    //==================
    //  トリムマネジャー
    //==================
    internal class TrimManager
    {
        public LineManager lineManager;
        public float scalef; //選択範囲につかうだけなのでfloat
        public Function01 function;

        public int selectedIndex = -1;//Newされる1回のみ初期化
        public int secondLineIndex = -1;  //右クリックのときの2つめの線
        public int secondCircleIndex = -1;//右クリックのときの2つめの円
        private PointDec firstClickPoint; //交点のどっち側がクリックされたか

        //==================
        //  コンストラクタ
        //==================
        public TrimManager(LineManager manager,
            float sf, Function01 func)
        {
            lineManager = manager ?? throw new ArgumentNullException(nameof(manager));
            function = func ?? throw new ArgumentNullException(nameof(func));
            scalef = sf;
        }
        //=========================================
        // TrimManager:OnMouseClick(Form1からの入口）
        //=========================================

        public bool OnMouseClick(PointDec world, bool isRightClick)
        {
            //  トリムするのは線（円は今回実装しない）
            //　1回目クリック インデックスがとれたかどうかを返す
            if (selectedIndex < 0)
            {
                selectedIndex = function.GetNearestLineIndex(world, scalef);
                if (selectedIndex < 0)
                    return false;

                firstClickPoint = world;  // クリック位置を保存

                return true;
            }
            //　2回目クリック
            var line = lineManager.decFile[selectedIndex];

            PointDec p1 = line.start;
            PointDec p2 = line.end;
            PointDec targetPoint = world;

            if (isRightClick)
            {
                int secondLineIndex = function.GetNearestLineIndexSecond(
                    world, scalef, selectedIndex);

                int secondCircleIndex = function.GetNearestCircleIndex(
                    world, scalef);

                bool hasLine = secondLineIndex >= 0;
                bool hasCircle = secondCircleIndex >= 0;

                // 両方取れていない → キャンセル
                if (!hasLine && !hasCircle)
                    return false;

                // 両方取れている → クリック点から近い方を選ぶ
                if (hasLine && hasCircle)
                {
                    decimal dLine = function.DistancePointToSegment(
                        world,
                        lineManager.decFile[secondLineIndex].start,
                        lineManager.decFile[secondLineIndex].end);

                    var c = lineManager.CircleFile[secondCircleIndex];
                    decimal dCircle = function.DistancePointToCircle(
                        world, c.center, c.radius);

                    if (dCircle < dLine)
                    {
                        // 円を使う
                        if (!function.TryGetLineCircleIntersection(
                                lineManager.decFile[selectedIndex],
                                c,
                                out targetPoint))
                            return false;
                    }
                    else
                    {
                        // 線を使う
                        if (!function.TryGetLineLineIntersection(
                                lineManager.decFile[selectedIndex],
                                lineManager.decFile[secondLineIndex],
                                out targetPoint))
                            return false;
                    }
                }
                // 線だけ取れている
                else if (hasLine)
                {
                    if (!function.TryGetLineLineIntersection(
                            lineManager.decFile[selectedIndex],
                            lineManager.decFile[secondLineIndex],
                            out targetPoint))
                        return false;
                }
                // 円だけ取れている
                else // hasCircle
                {
                    if (!function.TryGetLineCircleIntersection(
                            lineManager.decFile[selectedIndex],
                            lineManager.CircleFile[secondCircleIndex],
                            out targetPoint))
                        return false;
                }
            }

            // クリック点を線（延長）上に射影
            PointDec proj = ProjectPointToLine(targetPoint, p1, p2);

            // どちらの端点を動かすか
            // パラメータ t を計算
            decimal tClick = function.GetParameterT(firstClickPoint, p1, p2);
            decimal tCross = function.GetParameterT(proj, p1, p2);

            bool keepStartSide = tClick < tCross;

            if (keepStartSide)
            {
                // start 側を残す → end を交点へ
                line.end = proj;
            }
            else
            {
                // end 側を残す → start を交点へ
                line.start = proj;
            }

            lineManager.decFile[selectedIndex] = line;
            selectedIndex = -1;
            secondLineIndex = -1;
            secondCircleIndex = -1;
            return true;

        }

        private decimal Distance(PointDec a, PointDec b)
        {
            decimal dx = a.x - b.x;
            decimal dy = a.y - b.y;
            return (decimal)Math.Sqrt((double)(dx * dx + dy * dy));
        }

        private PointDec ProjectPointToLine(PointDec p, PointDec a, PointDec b)
        {
            decimal dx = b.x - a.x;
            decimal dy = b.y - a.y;

            if (dx == 0 && dy == 0)
                return a;

            decimal t =
                ((p.x - a.x) * dx + (p.y - a.y) * dy)
                / (dx * dx + dy * dy);

            // ★ Trim / Extend 両対応なので clamp しない
            return new PointDec(
                a.x + t * dx,
                a.y + t * dy
            );
        }


    }





}

