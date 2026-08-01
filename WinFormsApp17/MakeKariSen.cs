namespace WinFormsApp17
{
    // 仮線設定：値の保持＋仮線（グレーの下書き線）の生成
    public class MakeKariSen
    {
        private readonly LineManager lineManager;
        private readonly RoadManager roadManager;

        public const int GuideLayer = 9;

        public decimal SetbackFromSite { get; set; } = 0;   // 隣地境界線からの離れ(mm)
        public decimal SetbackFromRoad { get; set; } = 0;   // 道路境界線からの離れ(mm)

        public MakeKariSen(LineManager lineManager, RoadManager roadManager)
        {
            this.lineManager = lineManager;
            this.roadManager = roadManager;
        }

        //==========================
        //  仮線（グレー）を生成
        //==========================
        public void Generate()
        {
            if (SetbackFromSite <= 0 || SetbackFromRoad <= 0)
            {
                MessageBox.Show("離れを設定してください。");
                return;
            }

            var siteLinesWithIndex = lineManager.decFile
                .Select((line, idx) => (line, idx))
                .Where(x => x.line.Layer == (int)LineManager.LayerType.Site)
                .ToList();

            if (siteLinesWithIndex.Count < 3)
            {
                MessageBox.Show("敷地の線が足りません。");
                return;
            }

            var segments = siteLinesWithIndex
                .Select(x => (x.line.start, x.line.end))
                .ToList();

            var centroid = CentroidCalculator.CalcCentroidFromSegments(segments);

            // 前回分のガイド線を削除
            RemoveGuideLines();

            var newGuideLines = new List<LineEntity>();

            foreach (var (line, idx) in siteLinesWithIndex)
            {
                decimal setback = roadManager.roadWidthMap.ContainsKey(idx)
                    ? SetbackFromRoad
                    : SetbackFromSite;

                newGuideLines.Add(OffsetLineToward(line, centroid, setback));
            }

            lineManager.decFile.AddRange(newGuideLines);
        }

        //==========================
        //  前回のガイド線を削除
        //==========================
        private void RemoveGuideLines()
        {
            var oldGuides = lineManager.decFile
                .Where(l => l.Layer == GuideLayer)
                .ToList();

            foreach (var line in oldGuides)
                lineManager.decFile.Remove(line);
        }

        //==========================
        //  線を「内側」方向へオフセット
        //==========================
        private LineEntity OffsetLineToward(LineEntity line, PointDec centroid, decimal distance)
        {
            PointDec a = line.start;
            PointDec b = line.end;

            decimal dx = b.x - a.x;
            decimal dy = b.y - a.y;

            double length = Math.Sqrt((double)(dx * dx + dy * dy));
            if (length == 0)
                return line;

            // 線に垂直な方向（法線ベクトル）
            decimal nx = (decimal)(-(double)dy / length);
            decimal ny = (decimal)((double)dx / length);

            // 線の中点
            PointDec mid = new PointDec((a.x + b.x) / 2m, (a.y + b.y) / 2m);

            // 中点→重心方向との内積で「内側」を判定し、必要なら法線を反転
            decimal toCenterX = centroid.x - mid.x;
            decimal toCenterY = centroid.y - mid.y;
            decimal dot = nx * toCenterX + ny * toCenterY;

            if (dot < 0)
            {
                nx = -nx;
                ny = -ny;
            }

            return new LineEntity
            {
                start = new PointDec(a.x + nx * distance, a.y + ny * distance),
                end = new PointDec(b.x + nx * distance, b.y + ny * distance),
                Layer = GuideLayer
            };
        }
    }
}