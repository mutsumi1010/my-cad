namespace WinFormsApp17;

//===========================
//  IntersectionState クラス
//===========================
public class IntersectionState  //未使用
{
    public PointDec Point { get; set; }

    public IntersectionType Type { get; set; }

}


//=======================
//  RoadManager クラス
//=======================
public class RoadManager
{
    private readonly LineManager lineManager;
    private readonly Function01 function;
    private readonly RoadFunction roadFunction;
    private readonly float scalef;
    private readonly Form1 form1;
    private readonly FormSelectIntersection? formRoad02;
    private readonly MakeRoadIntersection makeRoadIntersection;

    public decimal Width1 { get; private set; }  //道路幅員１
    public decimal Width2 { get; private set; }  //道路幅員２

    public int SelectedIndex { get; private set; } = -1;
    public bool IsSelectingRoadBoundary { get; private set; } = false;

    //　交差点を選んでいるかどうか
    public bool IsSelectingIntersection { get; private set; } = false;  

    public Dictionary<int, (decimal w1, decimal w2)> roadWidthMap
        = new Dictionary<int, (decimal w1, decimal w2)>();

    public Dictionary<int, List<LineEntity>> roadVisualMap
        = new Dictionary<int, List<LineEntity>>();

    // 交差点のタイプを保存しておく   //未使用
    public List<IntersectionState>
        intersectionStates
        = new List<IntersectionState>();


    //=======================
    //  コンストラクタ
    //=======================
    public RoadManager(LineManager lineManager, float scalef, Form1 form1, FormSelectIntersection formRoad02)
    {
        this.form1 = form1;
        this.lineManager = lineManager;
        this.scalef = scalef;
        this.formRoad02 = formRoad02;
        this.function = new Function01(lineManager, form1);
        this.roadFunction = new RoadFunction(lineManager);
        this.makeRoadIntersection = new MakeRoadIntersection(
             lineManager,
             roadVisualMap,
             form1);

        //道路交点
        formRoad02.RoadPanelClicked += OnRoadPanelClicked;

    }

    //=======================
    //  道路幅員セット
    //=======================
    public void SetWidth(decimal w1, decimal w2)
    {
        //MessageBox.Show(Environment.StackTrace);

        Width1 = w1;
        Width2 = w2;

        if (SelectedIndex < 0)
            return;

        if (Width1 <= 0 || Width2 <= 0)
        {
            MessageBox.Show("幅員が0以下なので終了");
            return;
        }

        IsSelectingRoadBoundary = false;
        roadWidthMap[SelectedIndex] = (w1, w2);

        if (roadFunction.TryMakeOppositeRoadLine(
            SelectedIndex,
            Width1,
            Width2,
            out LineEntity oppositeLine))
        {
            RemoveRoadVisual(SelectedIndex);

            var sideLines = AddRoadSideLines();

            var roadLines = new List<LineEntity>();
            roadLines.Add(oppositeLine);
            roadLines.AddRange(sideLines);

            lineManager.decFile.AddRange(roadLines);
            roadVisualMap[SelectedIndex] = roadLines;

            //2つの道路の5ｍ延長線を消去
            RemoveConnectedRoadBoundaryExtensions(SelectedIndex);

            form1.Invalidate();
        }
    }

    //=======================
    //  道路境界線の選択
    //=======================
    public void OnMouseClick(PointDec clickPoint)
    {
        int index = function.GetNearestSiteLineIndex(clickPoint, scalef);

        var siteLines = lineManager.decFile
            .Where(l => l.Layer == 1)
            .ToList();

        if (siteLines.Count < 3)
        {
            MessageBox.Show("敷地線が3本未満です。\n先に敷地をつくってください。");
            return;
        }

        if (!roadFunction.IsClosedLoop(siteLines))
        {
            MessageBox.Show("敷地線が閉じたループになっていません。");
            return;
        }

        // 線が取れなかったクリックは無視
        if (index < 0)
            return;

        // 同じ線をクリック → 選択解除
        if (index == SelectedIndex)
        {
            SelectedIndex = -1;
            form1.Invalidate();
            return;
        }

        // 道路境界線を選択
        SelectedIndex = index;
        IsSelectingRoadBoundary = true;
        form1.Invalidate();

        form1.BeginInvoke(() =>
        {
            form1.RtextBox1.Focus();
            form1.RtextBox1.SelectAll();
        });

        if (roadWidthMap.TryGetValue(SelectedIndex, out var width))
        {
           
           
            form1.LoadingRoadWidth = true;

            form1.RtextBox1.Text = (width.w1 / 1000m).ToString("0.##");
            form1.RtextBox2.Text = (width.w2 / 1000m).ToString("0.##");

            form1.LoadingRoadWidth = false;

            // SetWidth(width.w1, width.w2);
        }
        else
        {
            form1.RtextBox1.Text = "";
            form1.RtextBox2.Text = "";

            form1.BeginInvoke(() =>
            {
                form1.RtextBox1.Focus();
                form1.RtextBox1.SelectAll();
            });
        }
    }
    

    //===============================
    // 道路境界線の左右に5m線を追加
    //===============================
    public List<LineEntity> AddRoadSideLines()
    {
        var result = new List<LineEntity>();

        if (SelectedIndex < 0)
            return result;

        var baseLine = lineManager.decFile[SelectedIndex];
        PointDec a = baseLine.start;
        PointDec b = baseLine.end;

        decimal dx = b.x - a.x;
        decimal dy = b.y - a.y;

        double L = Math.Sqrt((double)(dx * dx + dy * dy));
        if (L == 0)
            return result;

        decimal ux = (decimal)((double)dx / L);
        decimal uy = (decimal)((double)dy / L);

        decimal len = 5000m; // 5m

        // 左側線
        var leftLine = new LineEntity
        {
            start = new PointDec(a.x - ux * len, a.y - uy * len),
            end = a,
            Layer = 2
        };

        // 右側線
        var rightLine = new LineEntity
        {
            start = b,
            end = new PointDec(b.x + ux * len, b.y + uy * len),
            Layer = 2
        };

        result.Add(leftLine);
        result.Add(rightLine);

        return result;
    }

    public List<LineEntity> AddRoadSideLines(int roadIndex)
    {
        var result = new List<LineEntity>();

        if (roadIndex < 0)
            return result;

        var baseLine = lineManager.decFile[roadIndex];
        PointDec a = baseLine.start;
        PointDec b = baseLine.end;

        decimal dx = b.x - a.x;
        decimal dy = b.y - a.y;

        double L = Math.Sqrt((double)(dx * dx + dy * dy));
        if (L == 0)
            return result;

        decimal ux = (decimal)((double)dx / L);
        decimal uy = (decimal)((double)dy / L);

        decimal len = 5000m; // 5m

        // 左側線
        var leftLine = new LineEntity
        {
            start = new PointDec(a.x - ux * len, a.y - uy * len),
            end = a,
            Layer = 2
        };

        // 右側線
        var rightLine = new LineEntity
        {
            start = b,
            end = new PointDec(b.x + ux * len, b.y + uy * len),
            Layer = 2
        };

        result.Add(leftLine);
        result.Add(rightLine);

        return result;
    }

    //===============================
    // 既存の道路表示を削除
    //===============================
    private void RemoveRoadVisual(int roadIndex)
    {
        if (roadVisualMap.TryGetValue(roadIndex, out var oldLines))
        {
            foreach (var line in oldLines)
            {
                lineManager.decFile.Remove(line);
            }

            roadVisualMap.Remove(roadIndex);
        }
    }

    //================================================
    // 接している道路境界線側の5m延長線を削除
    //================================================
    private void RemoveConnectedRoadBoundaryExtensions(int currentRoadIndex)
    {
        if (currentRoadIndex < 0 ||
            currentRoadIndex >= lineManager.decFile.Count)
            return;

        var currentBoundary = lineManager.decFile[currentRoadIndex];

        // すでに道路幅員が設定されている道路を確認
        foreach (int otherRoadIndex in roadWidthMap.Keys.ToList())
        {
            if (otherRoadIndex == currentRoadIndex)
                continue;

            if (otherRoadIndex < 0 ||
                otherRoadIndex >= lineManager.decFile.Count)
                continue;

            var otherBoundary = lineManager.decFile[otherRoadIndex];

            // 2本の道路境界線が接している点を取得
            if (!TryGetSharedEndpoint(
                currentBoundary,
                otherBoundary,
                out PointDec intersection))
            {
                continue;
            }

            // 今回作った道路の延長線を削除
            RemoveSideLineAtPoint(currentRoadIndex, intersection);

            // 先に作ってあった道路の延長線を削除
            RemoveSideLineAtPoint(otherRoadIndex, intersection);
        }
    }

    //================================================
    // 指定した交点から伸びている道路境界線側の延長線を削除
    //================================================
    private void RemoveSideLineAtPoint(
        int roadIndex,
        PointDec intersection)
    {
        if (!roadVisualMap.TryGetValue(
            roadIndex,
            out var roadLines))
        {
            return;
        }

        // roadLines[0] は反対側道路境界線。
        // それ以降が道路境界線側の5m延長線。
        var sideLinesToRemove = roadLines
            .Skip(1)
            .Where(line =>
                IsSamePoint(line.start, intersection) ||
                IsSamePoint(line.end, intersection))
            .ToList();

        foreach (var line in sideLinesToRemove)
        {
            lineManager.decFile.Remove(line);
            roadLines.Remove(line);
        }
    }

    //================================================
    // 2本の道路境界線に共通する端点を取得
    //================================================
    private bool TryGetSharedEndpoint(
        LineEntity line1,
        LineEntity line2,
        out PointDec sharedPoint)
    {
        if (IsSamePoint(line1.start, line2.start) ||
            IsSamePoint(line1.start, line2.end))
        {
            sharedPoint = line1.start;
            return true;
        }

        if (IsSamePoint(line1.end, line2.start) ||
            IsSamePoint(line1.end, line2.end))
        {
            sharedPoint = line1.end;
            return true;
        }

        sharedPoint = default;
        return false;
    }

    //================================================
    // 座標が同じ点か判定
    //================================================
    private bool IsSamePoint(PointDec p1, PointDec p2)
    {
        const decimal tolerance = 0.001m;

        return Math.Abs(p1.x - p2.x) <= tolerance &&
               Math.Abs(p1.y - p2.y) <= tolerance;
    }

    //=======================
    //  選択解除
    //=======================
    public void Reset()
    {
        SelectedIndex = -1;
        Width1 = 0;
        Width2 = 0;
    }

    //===============================
    //  道路交点パネルが押されたとき
    //===============================
    public void OnRoadPanelClicked()
    {
        IsSelectingIntersection = true;

        //メッセージラベル：道路交点をクリックしてください
        form1.labelRoadIntersection.Visible = true;
        form1.labelRoadIntersection.BringToFront();

        form1.Invalidate();
    }

    //===============================
    //  道路交点が押されたとき
    //===============================

    public void OnIntersectionClick(PointDec world, RoadIntersectionType type)
    {
        if (!function.TrySnapToIntersectionNear(
              world,
              lineManager.decFile,
              lineManager.CircleFile,
              scalef,
              out var crossPoint))
        {
            MessageBox.Show("道路交点が見つかりません");
            return;
        }

        if (!TryReBuildRoadVisualMap(crossPoint))
        {
            return;
        }

        switch (type)
        {
            case RoadIntersectionType.Cross:
                makeRoadIntersection.MakeCrossIntersection(crossPoint);
                break;

            case RoadIntersectionType.THorizontal:
                makeRoadIntersection
                    .MakeTHorizontalIntersection(crossPoint); 
                break;
  
            case RoadIntersectionType.TVertical:
                makeRoadIntersection
                .MakeTVerticalIntersection(crossPoint);

                break;

            case RoadIntersectionType.LShape:
                makeRoadIntersection.MakeLshapeIntersection(crossPoint);
                break;

            default:
                MessageBox.Show("交点タイプが選択されていません");
                break;
        }

        //=================================
        // 交差点作成完了
        //=================================
        IsSelectingIntersection = false;

        formRoad02.ClearSelection();

        form1.Invalidate();

    }


    //===========================================
    // roadVisualMap の 初期値構築
    //===========================================
    private bool TryReBuildRoadVisualMap(PointDec siteCrossPoint)
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

            return false;
        }

        //RoadWhdthMap から キーroadIndex1 の値をとる
        int roadIndex1 = connectedRoadIndexes[0];
        int roadIndex2 = connectedRoadIndexes[1];


        //================================================
        // roadWidthMapから1本目の道路幅員を取得
        //================================================
        if (!roadWidthMap.TryGetValue(
                roadIndex1,
                out var roadWidth1))
        {
            MessageBox.Show(
                $"roadWidthMapに道路幅員情報がありません。Index:{roadIndex1}");
            return false;
        }

        decimal road1W1 = roadWidth1.w1;
        decimal road1W2 = roadWidth1.w2;


        //OPPOSITE LINE をつくる

        if (roadFunction.TryMakeOppositeRoadLine(
         roadIndex1,
         road1W1,
         road1W2,
         out LineEntity oppositeLine))
        {
            RemoveRoadVisual(roadIndex1);

            var sideLines = AddRoadSideLines(roadIndex1);

            var roadLines = new List<LineEntity>();
            roadLines.Add(oppositeLine);
            roadLines.AddRange(sideLines);

            lineManager.decFile.AddRange(roadLines);
            roadVisualMap[roadIndex1] = roadLines;

            //2つの道路の5ｍ延長線を消去
            RemoveConnectedRoadBoundaryExtensions(roadIndex1);

        }

        //================================================
        // roadWidthMapから2本目の道路幅員を取得
        //================================================
        if (!roadWidthMap.TryGetValue(
                roadIndex2,
                out var roadWidth2))
        {
            MessageBox.Show(
                $"roadWidthMapに道路幅員情報がありません。Index:{roadIndex2}");
            return false;
        }

        decimal road2W1 = roadWidth2.w1;
        decimal road2W2 = roadWidth2.w2;

        //OPPOSITE LINE をつくる（2本目）
        if (roadFunction.TryMakeOppositeRoadLine(
                roadIndex2,
                road2W1,
                road2W2,
                out LineEntity oppositeLine2))
        {
            RemoveRoadVisual(roadIndex2);

            var sideLines2 = AddRoadSideLines(roadIndex2);

            var roadLines2 = new List<LineEntity>();
            roadLines2.Add(oppositeLine2);
            roadLines2.AddRange(sideLines2);

            lineManager.decFile.AddRange(roadLines2);
            roadVisualMap[roadIndex2] = roadLines2;

            //2つの道路の5ｍ延長線を消去
            RemoveConnectedRoadBoundaryExtensions(roadIndex2);

        }
        return true;

    }




}