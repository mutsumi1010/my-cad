namespace WinFormsApp17;

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

    public decimal Width1 { get; private set; }  //道路幅員１
    public decimal Width2 { get; private set; }  //道路幅員２

    public int SelectedIndex { get; private set; } = -1;
    public bool IsSelectingRoadBoundary { get; private set; } = false;

    private Dictionary<int, (decimal w1, decimal w2)> roadWidthMap
        = new Dictionary<int, (decimal w1, decimal w2)>();

    private Dictionary<int, List<LineEntity>> roadVisualMap
        = new Dictionary<int, List<LineEntity>>();

    //=======================
    //  コンストラクタ
    //=======================
    public RoadManager(LineManager lineManager, float scalef, Form1 form1)
    {
        this.lineManager = lineManager;
        this.scalef = scalef;
        this.function = new Function01(lineManager, form1);
        this.roadFunction = new RoadFunction(lineManager);
        this.form1 = form1;
    }

    //=======================
    //  道路幅員セット
    //=======================
    public void SetWidth(decimal w1, decimal w2)
    {
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
            form1.RtextBox1.Text = (width.w1 / 1000m).ToString("0.##");
            form1.RtextBox2.Text = (width.w2 / 1000m).ToString("0.##");

            SetWidth(width.w1, width.w2);
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

    //=======================
    //  選択解除
    //=======================
    public void Reset()
    {
        SelectedIndex = -1;
        Width1 = 0;
        Width2 = 0;
    }
}