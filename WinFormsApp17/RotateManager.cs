using WinFormsApp17;
using System.Diagnostics;
internal class RotateManager
{
    //===============================
    //  Rotate クラス：フィールド
    //===============================
    public LineManager lineManager;
    public float scalef; //選択範囲につかうだけなのでfloat
    public Function01 function;
    public SelectIndex selectIndex;

    public bool IsFirstClickFinished = false;  //1点目とってる？
    public bool IsSecondClickFinished = false; //2点目とってる？
    public bool IsThirdClickFinished = false;  //3点目とってる？

    private PointDec firstClickPoint;          //1点目の保存
    private PointDec secondClickPoint;         //2点目の保存
    private PointDec thirdClickPoint;          //3点目の以降の保存
    public PointDec currentMousePoint;         //現在のポイント

    //==================
    //  コンストラクタ
    //==================
    public RotateManager(
        LineManager manager, float sclf, Function01 func, SelectIndex selectIndex)
    {
        lineManager = manager ?? throw new ArgumentNullException(nameof(manager));
        function = func ?? throw new ArgumentNullException(nameof(func));
        scalef = sclf;
        this.selectIndex = selectIndex;
    }

    //============================
    //  Rotate : OnMouseClick
    //============================
    public void OnMouseClick(PointDec world, bool IsRightClick)
    {
        if (!IsFirstClickFinished)
        {
            //右クリック
            if (IsRightClick)
            {   
                var found = function.TrySnapToEndpoint02(world, out firstClickPoint);
                IsFirstClickFinished = true;
                return;
            }
            // 左クリック
            firstClickPoint = world;
            IsFirstClickFinished = true;
            return;
        }
        if (!IsSecondClickFinished)
        {
            if (IsRightClick)
            {   
                var found = function.TrySnapToEndpoint02(world, out secondClickPoint);
                IsSecondClickFinished = true;
                Debug.WriteLine($"second migi = {secondClickPoint}");
                return;
            }
            secondClickPoint = world;
            IsSecondClickFinished = true;
            return;
        }
        if (!IsThirdClickFinished)
        {
            if (IsRightClick)
            {   
                var found = function.TrySnapToEndpoint02(world, out thirdClickPoint);
                IsThirdClickFinished = true;
                ApplyRotate();
                return;
            }
            thirdClickPoint = world;
            IsThirdClickFinished = true;
            ApplyRotate();
            return;
        }

    }
    //=====================================
    //  Rotate : ApplyRotate 回転メソッド
    //=====================================

    //foreach (int idx in selectIndex.GetSelectedIndexList())
    // 選択された線を回転させる
    public void ApplyRotate()
    {
        if (!IsFirstClickFinished ||
            !IsSecondClickFinished ||
            !IsThirdClickFinished)
            return;

        double angle = GetRotateAngle(
            firstClickPoint,
            secondClickPoint,
            thirdClickPoint);

        foreach (int idx in selectIndex.GetSelectedIndexList())
        {
            var line = lineManager.decFile[idx];

            var newStart = RotatePoint(
                line.start, firstClickPoint, angle);

            var newEnd = RotatePoint(
                line.end, firstClickPoint, angle);

            lineManager.decFile[idx] = new LineEntity
            {
                start = newStart,
                end = newEnd,
                Layer = line.Layer
            };

        }
        // 回転が終わったらリセット処理をする
        Reset();
        //selectIndex.ClearSelectedIndexList();
        //IsFirstClickFinished = false;
        //IsSecondClickFinished = false;
        //IsThirdClickFinished = false;
    }

    //点を回転させる関数
    private PointDec RotatePoint(
    PointDec p, PointDec center, double angleRad)
    {
        double cos = Math.Cos(angleRad);
        double sin = Math.Sin(angleRad);

        double dx = (double)(p.x - center.x);
        double dy = (double)(p.y - center.y);

        double rx = dx * cos - dy * sin;
        double ry = dx * sin + dy * cos;

        return new PointDec(
            (decimal)(rx + (double)center.x),
            (decimal)(ry + (double)center.y)
        );
    }

    //回転角を求めるベクトルと角度
    private double GetRotateAngle(
    PointDec center, PointDec from, PointDec to)
    {
        double vx1 = (double)(from.x - center.x);
        double vy1 = (double)(from.y - center.y);

        double vx2 = (double)(to.x - center.x);
        double vy2 = (double)(to.y - center.y);

        double ang1 = Math.Atan2(vy1, vx1);
        double ang2 = Math.Atan2(vy2, vx2);

        return ang2 - ang1; // ラジアン
    }

    public void Reset()
    {
        selectIndex.ClearSelectedIndexList();
        IsFirstClickFinished = false;
        IsSecondClickFinished = false;
        IsThirdClickFinished = false;
    }

}
