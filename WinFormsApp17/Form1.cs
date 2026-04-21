using System.Diagnostics;
using System.Drawing;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.IO;

namespace WinFormsApp17
{
    public enum SnapMode { Free, Horizon }
    public partial class Form1 : Form
    {
        PointF? startPos;
        PointF? endPos;
        bool isFirstClick;
        Form2 form2;
        Form5 form5; // 敷地面積計算Form

        Image pdfImage;
        bool isJpegFirsted;

        SiteDataList siteDataList;
        RoadDataList roadDataList;
        BuildingDataList buildingDataList;
        DimensionDataList dataList;

        LineManager lineManager;
        CornerManager cornerManager;
        CircleManager circleManager;
        ParallelManager parallelManager;
        TrimManager trimManager;
        RotateManager rotateManager;
        SelectIndex selectIndex;
        EraseManager eraseManager;
        DimensionBuilder dimBuilder;
        Function01 function;
        private DimensionMouseHandler dimMouse;
        private DimensionRenderer dimRenderer;
        RoadManager roadManager;

        private DxfLoad dxfLoad;
        private DxfSave dxfSave;
        private PrintManager printManager;

        //public enum SnapMode { Free, Horizon }
        public SnapMode snapMode = SnapMode.Free;

        float scalef = 0.1f;
        decimal scaledec = 0.1m;
        //----------------------
        float zoom = 1.2f;
        decimal zoomdec = 1.2m;
        //----------------------
        float offsetX = 0;
        float offsetY = 0;

        // ===== 円 =====
        private PointDec? pendingCircleCenter = null;
        private decimal? pendingCircleRadius = null;
        private Form4? circleForm;

        // ===== 寸法線 =====

        public enum DimensionDirection { Horizontal, Vertical, }

        public enum DimStep { Ext1, Ext2, Points }

        // ===== 道路 =====
        private bool copyingWidth = false;

        // ===== Erase Undo =====
        private Stack<(int index, PointDec start, PointDec end)> eraseHistory
            = new Stack<(int, PointDec, PointDec)>(5);

        //=============================
        //  Form1() コンストラクタ
        //=============================
        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.KeyPreview = true;
            ClientSize = new Size(1800, 900);

            this.form2 = new Form2();
            form2.ClientSize = new Size(125, 900);
            Controls.Add(form2);
            form2.Location = new Point(10, 10);
            this.form2.Show();

            this.siteDataList = new SiteDataList();
            this.roadDataList = new RoadDataList();
            this.buildingDataList = new BuildingDataList();
            this.dataList = new DimensionDataList();

            // マネジャー
            this.lineManager = new LineManager(siteDataList, roadDataList, buildingDataList);

            this.form5 = new Form5(this.lineManager);  // 敷地面積計算
            form5.TopLevel = false;
            form5.FormBorderStyle = FormBorderStyle.None;
            form5.Dock = DockStyle.Right;
            Controls.Add(form5);
            form5.RequestRedraw = () => this.Invalidate();
            form5.Show();

            this.cornerManager = new CornerManager(
                      lineManager, siteDataList,
                      roadDataList, buildingDataList
                     );
            this.circleManager = new CircleManager(
                      lineManager, siteDataList,
                      roadDataList, buildingDataList
                     );
            this.function = new Function01(lineManager, this); //line のあと、trim のまえ
            this.trimManager = new TrimManager(
                      lineManager, scalef, function);
            this.selectIndex = new SelectIndex(
                      lineManager, scalef, function);
            this.rotateManager = new RotateManager(
                      lineManager, scalef, function, selectIndex);
            this.function = new Function01(lineManager, this);

            this.parallelManager = new ParallelManager(lineManager, GetScale);
            this.eraseManager = new EraseManager(lineManager, dataList.DimList, scalef);
            this.dimBuilder = new DimensionBuilder();

            this.dimMouse = new DimensionMouseHandler(
                     dimBuilder, dataList, function,
                     ScreenToWorld, WorldToScreen, Invalidate);
            this.dimRenderer = new DimensionRenderer();

            this.roadManager = new RoadManager(lineManager, scalef, this);

            // セーブ、ロード、プリント
            this.dxfSave = new DxfSave(lineManager, dataList.DimList);
            this.dxfLoad = new DxfLoad(lineManager, dataList.DimList);
            this.printManager = new PrintManager(lineManager);

            this.MouseClick += Form1_MouseClick;
            this.MouseMove += Form1_MouseMove;
            this.MouseDown += Form1_MouseDown;
            this.MouseWheel += Form1_MouseWheel;
            this.Paint += Form1_Paint;
            this.KeyUp += Form1_KeyUp;
            this.KeyDown += Form1_KeyDown;

            roadlabel2.Visible = false;  // Road 幅員
            form2.ModeChanged += OnModeChanged;
            form2.TargetChanged += t =>
            {
                lineManager.SetCurrentTarget(t);

                if (t != TargetType.Road)
                {
                    roadManager.Reset();

                    RtextBox1.Text = "";
                    RtextBox2.Text = "";

                    roadlabel2.Visible = false;
                    Rlabel1.Visible = false;
                    Rlabel2.Visible = false;
                    RtextBox1.Visible = false;
                    RtextBox2.Visible = false;
                    labelRoadMessage.Visible = false;
                }
                else
                {
                    labelRoadMessage.Visible = true;
                }

                Invalidate();

            };
            form2.LoadClicked += () => { dxfLoad.Execute(); Invalidate(); };
            form2.SaveClicked += () => { dxfSave.Execute(); Invalidate(); };
            form2.PrintClicked += () => { printManager.Execute(); };

            //カテゴリー　道路　のときのメッセージ
            labelRoadMessage.Left = form2.Right + 5;
            labelRoadMessage.Top = form2.Top;
            labelRoadMessage.Visible = false;
            roadlabel2.Left = labelRoadMessage.Left;
            roadlabel2.Top = labelRoadMessage.Top;

            int gap = 10;
            Rlabel1.Left = roadlabel2.Right + gap;
            RtextBox1.Left = Rlabel1.Right + 5;
            Rlabel2.Left = RtextBox1.Right + gap;
            RtextBox2.Left = Rlabel2.Right + 5;

            // 垂直位置
            RtextBox1.Top = roadlabel2.Top;
            RtextBox2.Top = RtextBox1.Top;
            Rlabel1.Top = RtextBox1.Top + (RtextBox1.Height - Rlabel1.Height) / 2;
            Rlabel2.Top = RtextBox2.Top + (RtextBox2.Height - Rlabel2.Height) / 2;

            roadlabel2.Visible = false;  // Road 幅員
            Rlabel1.Visible = false;
            Rlabel2.Visible = false;
            RtextBox1.Visible = false;
            RtextBox2.Visible = false;

            //RtextBox1.Leave += RoadWidthChanged;
            //RtextBox2.Leave += RoadWidthChanged;
            //RtextBox1.TextChanged += RtextBox1_TextChanged;
            //RtextBox1.TextChanged += RoadWidthChanged;
            //RtextBox2.TextChanged += RoadWidthChanged;
            RtextBox1.TextChanged += RtextBox1_TextChanged;
            RtextBox2.TextChanged += RtextBox2_TextChanged;
        }
        //-----------------
        //  Enter key 
        //-----------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // TextBox がアクティブなら Enter は絶対に奪わない
            if (keyData == Keys.Enter && this.ActiveControl is TextBox)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            if (keyData == Keys.Enter)
            {
                // 回転モードのときだけ確定
                if (form2.GetMoveType() == MoveType.Rotate)
                {
                    selectIndex.SelectConfirm();
                    Invalidate();
                    return true;   // Enter を消費
                }

                // それ以外（円・TextBox入力など）は通常処理へ
                return base.ProcessCmdKey(ref msg, keyData);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //*******************//
        //   MouseWheel      //
        //*******************//
        private void Form1_MouseWheel(object? sender, MouseEventArgs e)
        {
            PointF screenBefore = new PointF(e.X, e.Y);
            //System.Diagnostics.Debug.WriteLine($"sBefore = {screenBefore.X:F18} , {screenBefore.Y:F18}");

            PointDec worldBefore = ScreenToWorld(screenBefore);
            //System.Diagnostics.Debug.WriteLine($"wBefore = {worldBefore.x:F18} , {worldBefore.y:F18}");
            //System.Diagnostics.Debug.WriteLine($"くるくる前のscale = {scalef:F18},{scalef:F18}");

            if (e.Delta > 0)
            {
                scalef *= zoom;
                scaledec *= zoomdec;
            }
            else
            {
                scalef /= zoom;
                scaledec /= zoomdec;
            }
            //System.Diagnostics.Debug.WriteLine("-----------くるくるしたよ---------------");
            //System.Diagnostics.Debug.WriteLine($"くるくる後のscale = {scalef:F18} , {scalef:F18}");
            PointF screenAfter = WorldToScreen(worldBefore);
            //System.Diagnostics.Debug.WriteLine($"sAfter = {screenAfter.X:F18},{screenAfter.Y:F18}");
            //System.Diagnostics.Debug.WriteLine($"更新前offset = {offsetX:F18},{offsetY:F18}");
            offsetX += (screenBefore.X - screenAfter.X);
            offsetY += (screenBefore.Y - screenAfter.Y);
            //System.Diagnostics.Debug.WriteLine($"更新後offset = {offsetX:F18} , {offsetY:F18}");
            //System.Diagnostics.Debug.WriteLine($"        ");
            Invalidate();
        }
        //*******************//
        //   MouseClick      //
        //*******************//
        private void Form1_MouseClick(object? sender, MouseEventArgs e)
        {
            var mode = form2.GetMoveType();

            //-----------------------------
            //  MouseClick : Road
            //-----------------------------
            if (lineManager.TargetM == TargetType.Road)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));
                roadManager.OnMouseClick(world);

                // メッセージ切り替え
                labelRoadMessage.Visible = (roadManager.SelectedIndex < 0);
                roadlabel2.Visible = (roadManager.SelectedIndex >= 0);
                Rlabel1.Visible = (roadManager.SelectedIndex >= 0);
                Rlabel2.Visible = (roadManager.SelectedIndex >= 0);
                RtextBox1.Visible = (roadManager.SelectedIndex >= 0);
                RtextBox2.Visible = (roadManager.SelectedIndex >= 0);

                Invalidate();
                return;

            }

            //-----------------------------
            //  MouseClick : Parallel
            //-----------------------------
            if (mode == MoveType.Parallel)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));

                if (parallelManager.OnMouseClick(world))
                {
                    Invalidate();
                }
                return;
            }

            //-----------------------------
            //  MouseClick : Trim
            //-----------------------------
            if (mode == MoveType.Trim)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));
                bool isRightClick = (e.Button == MouseButtons.Right);

                if (trimManager.OnMouseClick(world, isRightClick))
                {
                    Invalidate();
                }
                return;
            }
            //-----------------------------
            //  MouseClick : Rotate
            //-----------------------------
            if (mode == MoveType.Rotate)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));
                //　セレクトが確定していなかったら
                if (!selectIndex.IsSelectConfirmed)
                {
                    selectIndex.OnMouseClick(world);  // 矩計内部の線をセレクト
                    Invalidate();
                    return;
                }
                //　セレクトが確定したら
                bool isRightClick = (e.Button == MouseButtons.Right);
                rotateManager.OnMouseClick(world, isRightClick);
                Invalidate();
                return;
            }

            //-----------------------------
            //  MouseClick : Draw
            //-----------------------------
            if (mode == MoveType.Draw)
            {
                var worldF = ScreenToWorldF(new PointF(e.X, e.Y));

                if (e.Button == MouseButtons.Right)
                {
                    var worldDec = ToWorld(worldF);

                    if (!function.TrySnapToEndpoint(worldDec, WorldToScreen,
                        out var snappedDec))
                    {
                        return;
                    }
                    worldF = ToScreen(snappedDec);
                }

                if (!isFirstClick)
                {
                    startPos = worldF;
                    isFirstClick = true;
                }
                else
                {
                    endPos = function.ApplySnap(worldF, startPos);

                    var p1 = ToWorld(startPos.Value);
                    var p2 = ToWorld(endPos.Value);

                    lineManager.AddLine(p1, p2);
                    isFirstClick = false;
                    Invalidate();
                }
                return;
            }

            //-----------------------------
            //  MouseClick : DrawCircle
            //-----------------------------
            if (mode == MoveType.DrawCircle)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));

                // 右クリック → 線端点スナップのみ許可
                if (e.Button == MouseButtons.Right)
                {
                    if (!function.TrySnapToEndpoint(world, WorldToScreen, out var snappedDec))
                    {
                        return; // 取れなければ何もしない
                    }

                    world = snappedDec; // 取れた端点を中心にする
                }

                if (circleForm == null)
                    return;

                if (!circleForm.TryGetRadius(out var radius))
                    return;

                if (radius <= 0)
                    return;

                circleManager.AddCircle(
                    world,
                    radius,
                    lineManager.TargetM
                );

                circleForm.ResetForNextCircle();
                Invalidate();
                return;
            }

            //-----------------------------
            //  MouseClick : Erase
            //-----------------------------
            if (mode == MoveType.Erase)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));
                // 円を先にチェック
                decimal tol = 5m / scaledec; // ← 画面5px相当
                if (circleManager.RemoveCircleAt(world, tol))
                {
                    Invalidate();
                    return;
                }
                // 線の削除
                if (eraseManager.OnMouseClick(world))
                    Invalidate();
                return;
            }
            //-----------------------------
            //  MouseClick : Corner
            //-----------------------------
            if (mode == MoveType.Corner)
            {
                PointDec click = ScreenToWorld(new PointF(e.X, e.Y));

                if (cornerManager.ClickCorner(click, scalef))
                {
                    Invalidate();
                }
                return;
            }
            //-----------------------------
            //  MouseClick : Dimension
            //-----------------------------
            if (mode == MoveType.Dimension)
            {
                dimMouse.OnMouseClick(e);
                return;
            }
        }

        //*******************//
        //   MouseDown       //
        //*******************//
        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (form2.GetMoveType() == MoveType.Parallel)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));
                parallelManager.OnMouseDown(world);
            }
        }

        //*******************//
        //   MouseMove       //
        //*******************//
        private void Form1_MouseMove(object? sender, MouseEventArgs e)
        {
            var mode = form2.GetMoveType();

            //-----------------------------
            //  MouseMove : Roted
            //-----------------------------
            if (mode == MoveType.Rotate)
            {
                var world = ScreenToWorld(new PointF(e.X, e.Y));
                selectIndex.OnMouseMove(world);
                Invalidate();
            }

            //-----------------------------
            //  MouseMove : Draw
            //-----------------------------
            if (mode == MoveType.Draw)
            {
                if (isFirstClick)
                {
                    var worldP = ScreenToWorldF(new PointF(e.X, e.Y));
                    endPos = function.ApplySnap(worldP, startPos);
                    Invalidate();
                }
                return;
            }
            //-----------------------------
            //  MouseMove : Parallel
            //-----------------------------
            if (mode == MoveType.Parallel)
            {
                PointDec world = ScreenToWorld(new PointF(e.X, e.Y));
                parallelManager.OnMouseMove(world);
                Invalidate();
                return;
            }
        }
        //*******************//
        //     Paint         //
        //*******************//
        private void Form1_Paint(object? sender, PaintEventArgs e)
        {
            var target = form2.GetCurrentTarget();
            var mode = form2.GetMoveType();

            if (pdfImage != null && isJpegFirsted)
            {
                float clientW = this.ClientSize.Width;
                float clientH = this.ClientSize.Height;

                float imgW = pdfImage.Width * scalef;
                float imgH = pdfImage.Height * scalef;

                offsetX = (clientW - imgW) / 2f;
                offsetY = (clientH - imgH) / 2f;

                isJpegFirsted = false;
            }

            e.Graphics.TranslateTransform(offsetX, offsetY);
            e.Graphics.ScaleTransform(scalef, scalef);

            if (pdfImage != null)
            {
                e.Graphics.DrawImage(pdfImage, 0, 0);
            }

            //=========================
            //  Paint : 通常の線
            //=========================
            for (int i = 0; i < lineManager.decFile.Count; i++)
            {
                var line = lineManager.decFile[i];
                var p1 = line.start.ToPointF();
                var p2 = line.end.ToPointF();

                var layer = line.Layer;
                
                //Debug.WriteLine(layer);
                Pen pen = layer switch
                {
                    1 => new Pen(Color.LimeGreen, 2f / scalef),
                    2 => new Pen(Color.LimeGreen, 2f / scalef),
                    _ => new Pen(Color.Black, 2f / scalef),
                };

                if (mode == MoveType.Parallel && i == parallelManager.SelectedIndex)
                    pen = new Pen(Color.Red, 2f / scalef);

                if (mode == MoveType.Corner && i == cornerManager.FirstIndex)
                    pen = new Pen(Color.Red, 2f / scalef);

                if (mode == MoveType.Trim && i == trimManager.selectedIndex)
                    pen = new Pen(Color.Red, 2f / scalef);

                // 道路カテゴリーで選択中の敷地境界線
                if (lineManager.TargetM == TargetType.Road && i == roadManager.SelectedIndex)
                {
                    if (roadManager.IsSelectingRoadBoundary)
                        pen = new Pen(Color.Red, 2f / scalef);      // 選択中
                    else
                        pen = new Pen(Color.LimeGreen, 2f / scalef); // 幅員入力後
                }

                e.Graphics.DrawLine(pen, p1, p2);
                pen.Dispose();  //  閉じる
            }
            //  Rotate 選択線を赤で上描き
            if (mode == MoveType.Rotate)
            {
                foreach (int idx in selectIndex.GetSelectedIndexList())
                {
                    var line = lineManager.decFile[idx];
                    using var pen = new Pen(Color.Red, 2f / scalef);
                    e.Graphics.DrawLine(
                        new Pen(Color.Red, 2f / scalef),
                      line.start.ToPointF(),
                        line.end.ToPointF());
                    e.Graphics.DrawLine(
                        pen, line.start.ToPointF(), line.end.ToPointF());

                }
            }

            //=========================
            // Rotate : 仮選択矩形（青）
            //=========================
            if (mode == MoveType.Rotate && selectIndex.IsFirstClicked)
            {
                PointF p1 = selectIndex.FirstPoint.ToPointF();
                PointF p2;

                // 2点目がまだならマウス位置、終わってたら2点目
                if (!selectIndex.IsSecondClickFinished)
                {
                    p2 = selectIndex.CurrentPoint.ToPointF();
                }
                else
                {
                    p2 = selectIndex.SecondPoint.ToPointF();
                }

                float x = Math.Min(p1.X, p2.X);
                float y = Math.Min(p1.Y, p2.Y);
                float w = Math.Abs(p1.X - p2.X);
                float h = Math.Abs(p1.Y - p2.Y);

                using var pen = new Pen(Color.DodgerBlue, 1f / scalef)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
                };

                e.Graphics.DrawRectangle(pen, x, y, w, h);
            }

            //----------------------------------
            //  Paint : Draw （仮線）
            //----------------------------------
            if (mode == MoveType.Draw && isFirstClick && endPos != null)
            {
                e.Graphics.DrawLine(Pens.Gray, startPos.Value, endPos.Value);
            }
            //----------------------------------
            //  Paint : Parallel
            //----------------------------------
            if (mode == MoveType.Parallel && parallelManager.HasPreview())
            {
                var (s, g) = parallelManager.GetPreviewLine();
                e.Graphics.DrawLine(Pens.Blue, s.ToPointF(), g.ToPointF());
            }
            //----------------------------------
            //  Paint : Circle
            //----------------------------------
            //if (mode == MoveType.DrawCircle)
            {
                foreach (var c in lineManager.CircleFile)
                {
                    var centerF = c.center.ToPointF();
                    float r = (float)c.radius;

                    e.Graphics.DrawEllipse(
                        Pens.Blue,
                        centerF.X - r, centerF.Y - r,
                        r * 2, r * 2);
                }
            }
            //----------------------------------
            //  Paint : Dimension
            //----------------------------------
            dimRenderer.Draw(e.Graphics, dataList.DimList, scalef);

            // ===== 仮線 =====
            var ext1 = dimMouse.Ext1Point;
            var ext2 = dimMouse.Ext2Point;
            var dir = dimMouse.Direction;

            if (mode == MoveType.Dimension)
            {
                using var pen = new Pen(Color.Gray, 1f / scalef);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                float big = 100000f;
                if (ext1.HasValue)
                {
                    var p = ext1.Value;
                    if (dir == DimensionDirection.Horizontal)
                    {
                        e.Graphics.DrawLine(pen, -big, (float)p.y, big, (float)p.y);
                    }
                    else
                    {
                        e.Graphics.DrawLine(pen, (float)p.x, -big, (float)p.x, big);
                    }
                }
                if (ext2.HasValue)
                {
                    var p = ext2.Value;
                    if (dir == DimensionDirection.Horizontal)
                    {
                        e.Graphics.DrawLine(pen, -big, (float)p.y, big, (float)p.y);
                    }
                    else
                    {
                        e.Graphics.DrawLine(pen, (float)p.x, -big, (float)p.x, big);
                    }
                }
            }
        }
        //******************************//
        //     KeyUp                    //
        //******************************//
        //スペースキーが押されたとき
        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            //============================================
            //  KeyUp : Dimension : 仮線 水平・垂直 切り替え
            //============================================
            if (e.KeyCode == Keys.Space)
            {
                // 寸法モード ＆ Ext1 のときだけ方向切替
                if (form2.GetMoveType() == MoveType.Dimension)

                {
                    dimMouse.ToggleDirectionIfAllowed();
                    e.Handled = true;
                    return;
                }

                //=============================
                // KeyUp : Draw 
                //=============================
                if (form2.GetMoveType() != MoveType.Dimension)
                {
                    snapMode = snapMode == SnapMode.Free
                        ? SnapMode.Horizon
                        : SnapMode.Free;
                    e.Handled = true;
                }
            }
        }

        //*************************//
        //    OnModeChanged()      //
        //*************************//
        public void OnModeChanged()
        {
            startPos = null;
            endPos = null;
            isFirstClick = false;

            // Circle
            if (form2.GetMoveType() == MoveType.DrawCircle)
            {
                if (circleForm == null || circleForm.IsDisposed)
                {
                    circleForm = new Form4();
                    // circleForm.RadiusEntered += OnRadiusEntered;
                    //  Form1 の子として追加（※TopLevel=false前提）
                    this.Controls.Add(circleForm);
                    //  位置指定：Form2 の右・上端揃え
                    circleForm.Location = new Point(
                        form2.Right + 5,   // すぐ右
                        form2.Top          // 上端揃え
                    );

                    circleForm.Show();
                    circleForm.BringToFront();
                    circleForm.Activate();

                }
            }
            if (form2.GetMoveType() != MoveType.DrawCircle)
            {
                // 状態クリア
                pendingCircleCenter = null;
                pendingCircleRadius = null;

                // Form4 を閉じる
                if (circleForm != null && !circleForm.IsDisposed)
                {
                    circleForm.Close();
                    circleForm = null;
                }
            }
            // Rotate
            if (form2.GetMoveType() == MoveType.Rotate)
            {
                rotateManager.Reset();
                selectIndex.Reset();
            }
            // Parallel 
            if (form2.GetMoveType() == MoveType.Parallel)
            {
                parallelManager.SetOffsetDistance(form2.GetOffsetDistance);
            }
            // Parallel 以外に切り替わったら赤い線を解除
            if (form2.GetMoveType() != MoveType.Parallel)
            {
                parallelManager.Reset();
            }
            // Corner 以外に切り替わったら状態クリア
            if (form2.GetMoveType() != MoveType.Corner)
            {
                cornerManager.Reset();
            }
            // Erase 以外に切り替わったらスタッククリア
            if (form2.GetMoveType() != MoveType.Erase)
            {
                eraseManager.Reset();
            }
            // Dimension に入った瞬間
            if (form2.GetMoveType() == MoveType.Dimension)
            {
                dimMouse.Reset();
            }
            // Dimension から出た瞬間
            if (form2.GetMoveType() != MoveType.Dimension)
            {
                dimMouse.Reset();
            }

            Invalidate();
        }

        //================================
        //   道路幅員　入力
        //================================
        private void RoadWidthChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(RtextBox1.Text, out decimal w1) &&
                decimal.TryParse(RtextBox2.Text, out decimal w2))
            {
                roadManager.SetWidth(w1, w2);
            }
        }
        //---------------------------------------
        private void RtextBox1_TextChanged(object sender, EventArgs e)
        {
            if (copyingWidth)
                return;

            copyingWidth = true;
            RtextBox2.Text = RtextBox1.Text;   // 左を右へコピー
            copyingWidth = false;

            ApplyRoadWidth();
        }
        //---------------------------------------
        private void RtextBox2_TextChanged(object sender, EventArgs e)
        {
            if (copyingWidth)
                return;

            ApplyRoadWidth();
        }

        //---------------------------------------
        private void ApplyRoadWidth()
        {
            if (decimal.TryParse(RtextBox1.Text, out decimal w1) &&
                decimal.TryParse(RtextBox2.Text, out decimal w2))
            {
                // m → mm
                roadManager.SetWidth(w1 * 1000m, w2 * 1000m);
            }
        }

        //================================
        //   画面座標 → 世界座標変換関数 
        //================================
        private PointDec ToWorld(PointF p)
        {
            return new PointDec((decimal)p.X, (decimal)p.Y);
        }

        //================================
        //   世界座標 → 画面座標変換関数 
        //================================
        private PointF ToScreen(PointDec p)
        {
            return new PointF((float)p.x, (float)p.y);
        }

        //=========================
        // World → Screen
        //=========================
        public PointF WorldToScreen(PointDec p)
        {
            return new PointF(
                (float)((float)p.x * scalef + offsetX),
                (float)((float)p.y * scalef + offsetY)
            );
        }

        // float 版
        private PointF WorldToScreenF(PointF p)
        {
            return new PointF(
                p.X * scalef + offsetX,
                p.Y * scalef + offsetY
            );
        }

        //=========================
        // Screen → World
        //=========================
        private PointDec ScreenToWorld(PointF p)
        {
            return new PointDec(
                (decimal)((p.X - offsetX) / scalef),
                (decimal)((p.Y - offsetY) / scalef)
            );
        }

        private PointF ScreenToWorldF(PointF p)
        {
            return new PointF(
                (p.X - offsetX) / scalef,
                (p.Y - offsetY) / scalef
            );
        }

        //**************************//
        //  KeyDown  (ESC UNDO)     //
        //**************************//
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // ------------------------------
            //   KeyDowm : Ctrl + Z : Erase
            // ------------------------------
            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (eraseManager.Undo())
                    Invalidate();
                return;
            }

            // ----------------------------
            //   KeyDowm : ESC
            // ----------------------------
            if (e.KeyCode == Keys.Escape)
            {
                var mode = form2.GetMoveType();

                // ----------------------------
                //   KeyDowm : Esc : Parallel
                // ----------------------------
                if (mode == MoveType.Parallel)
                {
                    parallelManager.Reset();
                    Invalidate();
                    return;
                }

                // ----------------------------
                //   KeyDowm : Esc : Draw
                // ----------------------------
                if (mode == MoveType.Draw)
                {
                    // 1点目だけ決めている → 完全にキャンセル（グレー線消える）
                    if (isFirstClick)
                    {
                        startPos = null;
                        endPos = null;
                        isFirstClick = false;
                        Invalidate();
                        return;
                    }

                    // 2点目を決めた直後 → 直近の線をUNDO（1点目に戻る）
                    if (!isFirstClick && startPos != null && endPos != null)
                    {
                        // 追加された線を消す
                        if (lineManager.decFile.Count > 0)
                            lineManager.RemoveLastLine();

                        // 1点目に戻す
                        endPos = null;
                        isFirstClick = true;
                        Invalidate();
                        return;
                    }

                    // 完全に何もない → 何もしない
                    return;
                }

                // ----------------------------
                // ③ Erase の場合は何もしない
                // ----------------------------
                if (mode == MoveType.Erase)
                {
                    return;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled) return;

            switch (e.KeyCode)
            {
                case Keys.H:
                    form2.SelectDrawMode();
                    break;

                case Keys.D:
                    form2.SelectEraseMode();
                    break;

                case Keys.F:
                    form2.SelectParallelMode();
                    break;

                case Keys.V:
                    form2.SelectCornerMode();
                    break;

                case Keys.S:   // 例：Dimension を M に割り当てるなら
                    form2.SelectDimensionMode();
                    break;

                case Keys.E:
                    form2.SelectCircleMode();
                    break;

                case Keys.T:
                    form2.SelectTrimMode();
                    break;
            }
        }
        public float GetScale()
        {
            return scalef;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void pDF読込ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "画像ファイル (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;

                pdfImage = Image.FromFile(path);

                MessageBox.Show($"読込OK  {pdfImage.Width} × {pdfImage.Height}");

                isJpegFirsted = true;

                Invalidate();
                
            }
        }

    }
}


