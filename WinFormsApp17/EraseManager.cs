namespace WinFormsApp17
{
    public class EraseManager
    {
        private readonly LineManager lineManager;
        private readonly List<Dimension> dimFile;


        // Undo 履歴（最大5）
        private Stack<(int index, PointDec start, PointDec end)> history
            = new Stack<(int, PointDec, PointDec)>(5);

        public EraseManager(LineManager manager, List<Dimension>dimFile)
        {
            this.lineManager = manager;
            this.dimFile = dimFile;
        }

        //-------------------------------------
        // クリックで削除（成功なら true）
        //-------------------------------------
        public bool OnMouseClick(PointDec world)
        {
            // ==========================
            // ① 通常線の消去（従来どおり）
            // ==========================
            float minDist = float.MaxValue;
            int eraseIndex = -1;

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                var l = lineManager.decFile[i];
                float d = DistancePointToSegment(
                    world.ToPointF(),
                    l.start.ToPointF(),
                    l.end.ToPointF()
                );

                if (d < 200 && d < minDist)
                {
                    minDist = d;
                    eraseIndex = i;
                }
            }

            if (eraseIndex >= 0)
            {
                var line = lineManager.decFile[eraseIndex];

                if (history.Count == 5)
                {
                    history = new Stack<(int, PointDec, PointDec)>(
                        history.Reverse().Skip(1)
                    );
                }

                history.Push((eraseIndex, line.start, line.end));
                //----------------------------------------
                //　線の削除 : LineManager のメソッド呼び出し
                //----------------------------------------
                lineManager.RemoveLine(line);

                return true;
            }

            // ==========================
            // ② 寸法線の消去（Sen1–Sen2）
            // ==========================
            for (int i = 0; i < dimFile.Count; i++)
            {
                var d = dimFile[i];

                float dist = DistancePointToSegment(
                    world.ToPointF(),
                    d.Sen1.ToPointF(),
                    d.Sen2.ToPointF()
                );

                if (dist < 200)
                {
                    dimFile.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        //-------------------------------------
        // Undo（Ctrl+Z）
        //-------------------------------------
        public bool Undo()
        {
            if (history.Count == 0) return false;

            var item = history.Pop();
            //--------------------
            //   線をインサート
            //--------------------
            lineManager.InsertLine(item.index, item.start, item.end);
            return true;
        }

        //-------------------------------------
        // 最近接距離（float）
        //-------------------------------------
        private float DistancePointToSegment(PointF p, PointF a, PointF b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            if (dx == 0 && dy == 0) return Distance(p, a);

            float t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));

            float px = a.X + t * dx;
            float py = a.Y + t * dy;
            return Distance(p, new PointF(px, py));
        }

        private float Distance(PointF p1, PointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        //-------------------------------------
        // 履歴クリア（モード変更時）
        //-------------------------------------
        public void Reset()
        {
            history.Clear();
        }
    }
}
