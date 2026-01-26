
using WinFormsApp17;
using System.Diagnostics;


internal class SelectIndex
{
    //=============================
    //  SelectIndexクラス：フィールド
    //=============================

    // **セレクテッドインデックスリスト**
    List<int> SelectedIndexList = new();

    public LineManager lineManager;
    public float scalef;          //選択範囲につかうだけなのでfloat
    public Function01 function;

    public bool IsFirstClickFinished = false;  //1点目とってる？
    public bool IsSecondClickFinished = false; //2点目とってる？
    public bool selectConfirm = false;         //Select確定
    private PointDec firstClickPoint;          //1点目の保存
    private PointDec secondClickPoint;         //2点目の保存
    private PointDec thirdClickPoint;          //3点目の以降の保存
    public PointDec currentMousePoint;         //現在のポイント
    public bool IsFirstClicked => IsFirstClickFinished;
    public bool IsSecondClicked => IsSecondClickFinished;
    public bool IsSelectConfirmed => selectConfirm;  //Select確定か？

    public PointDec FirstPoint => firstClickPoint;
    public PointDec SecondPoint => secondClickPoint;
    public PointDec CurrentPoint => currentMousePoint;

    decimal minX;
    decimal maxX;
    decimal minY;
    decimal maxY;

    //==================
    //  コンストラクタ
    //==================
    public SelectIndex(
        LineManager manager, float sclf, Function01 func)
    {

        lineManager = manager ?? throw new ArgumentNullException(nameof(manager));
        function = func ?? throw new ArgumentNullException(nameof(func));
        scalef = sclf;

    }

    //=========================================
    // SelectIndex:OnMouseClick(Form1からの入口）
    //=========================================

    public void OnMouseClick(PointDec world)
    {
        if (!IsFirstClickFinished)
        {
            firstClickPoint = world;
            IsFirstClickFinished = true;
            return;
        }

        if (!IsSecondClickFinished)
        {
            secondClickPoint = world;
            IsSecondClickFinished = true; // 2点目クリック終わった

            minX = Math.Min(firstClickPoint.x, secondClickPoint.x);
            maxX = Math.Max(firstClickPoint.x, secondClickPoint.x);
            minY = Math.Min(firstClickPoint.y, secondClickPoint.y);
            maxY = Math.Max(firstClickPoint.y, secondClickPoint.y);

            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                var line = lineManager.decFile[i];
                var s = line.start;
                var e = line.end;

                if (IsLineInside(s, e, minX, maxX, minY, maxY))
                {
                    SelectedIndexList.Add(i);
                }
            }
            return;
        }
        // 3点目からの処理
        thirdClickPoint = world;
        int idx = function.GetNearestLineIndex(thirdClickPoint, scalef);
        //SelectedIndexList に同じインデックスがあれば解除、なければ追加
        if (idx >= 0)
        {
            if (SelectedIndexList.Contains(idx))
            {
                // すでに選ばれている → 解除
                SelectedIndexList.Remove(idx);
            }
            else
            {
                // まだ選ばれていない → 追加
                SelectedIndexList.Add(idx);
            }
        }
        return;
    }
    //==================================================
    // SelectIndex:以下関数メソッド
    //==================================================
    // ラインが範囲にはいっているか
    private bool IsLineInside(
            PointDec start, PointDec end,
            decimal minX, decimal maxX,
            decimal minY, decimal maxY
            )
    {
        return
          IsPointInsideRect(start, minX, maxX, minY, maxY) &&
          IsPointInsideRect(end, minX, maxX, minY, maxY);
    }
    private bool IsPointInsideRect(
    PointDec p,
    decimal minX,
    decimal maxX,
    decimal minY,
    decimal maxY)
    {
        return
            p.x >= minX && p.x <= maxX &&
            p.y >= minY && p.y <= maxY;
    }

    // MouseMove を受け取るメソッド
    public void OnMouseMove(PointDec world)
    {
        if (!IsFirstClickFinished) return;
        if (IsSecondClickFinished) return;
        currentMousePoint = world;
    }

    //セレクト終了メソッド エンターがおされて確定するとき
    public void SelectConfirm()
    {
        selectConfirm = true;
        IsFirstClickFinished = false;
        IsSecondClickFinished = false;
    }

    // セレクテッドインデックスリストの取得メソッド
    public IReadOnlyList<int> GetSelectedIndexList()
    {
        return SelectedIndexList;
    }

    // セレクテッドインデックスリストのクリアメソッド
    public void ClearSelectedIndexList()
    {
        SelectedIndexList.Clear();
        selectConfirm = false;
    }

    public void Reset()
    {
        IsFirstClickFinished= false;
        IsSecondClickFinished= false;
        selectConfirm = false;
        firstClickPoint = default;
        secondClickPoint = default;
        thirdClickPoint = default;
        currentMousePoint= default;
    }
}
