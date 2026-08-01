using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace WinFormsApp17
{
    public enum TargetType
    {
        None = 0,
        Site,
        Road,
        Building
    }

    public enum MoveType
    {
        None = 0,
        Draw = 1,
        Erase = 2,
        Parallel = 3,
        Corner = 4,
        Dimension = 5,
        DrawCircle = 6,
        Trim = 7,
        Rotate,
        RangeSelect   //範囲選択
    }
    //===============================
    //   Form2 クラス
    //===============================
    public partial class Form2 : Form
    {
        private MoveType moveType;
        private Button? activeButton;
        private TargetType currentTarget = TargetType.None;
        private Button? activeTargetButton;

        public event Action? ModeChanged;
        public event Action? PrintClicked;
        public event Action<TargetType>? TargetChanged;
        public event Action? BackgroundToggleClicked;

        // parallel で使用
        public decimal GetOffsetDistance { get; private set; } = 0;

        public MoveType GetMoveType() => moveType;

        private bool isUseBoundaryMode = false;

        public bool IsUseBoundaryMode => isUseBoundaryMode;

        //===============================
        //   コンストラクタ
        //===============================
        public Form2()
        {
            InitializeComponent();
            this.Shown += (s, e) => this.ActiveControl = null;
            this.moveType = MoveType.None;
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.None;
            SetToolButtonsInitialColor();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        //----------------------------
        //  文字　グレーアウト
        //----------------------------
        private void SetToolButtonsInitialColor()
        {
            Color c = SystemColors.ControlLight;

            button1.ForeColor = c; // 線
            button2.ForeColor = c; // 消す
            button5.ForeColor = c; // 複線
            button6.ForeColor = c; // コーナ
            button8.ForeColor = c; // 寸法
            button12.ForeColor = c; // 円
            button13.ForeColor = c; // 伸縮
            button14.ForeColor = c; // 回転
            btnUseBoundary.ForeColor = c; // 用途境界線
            btnRange.ForeColor = c; // 範囲選択
        }
        //----------------------------
        //  文字　黒
        //----------------------------
        private void SetToolButtonsNormalColor()
        {
            Color c = SystemColors.ControlText;

            button1.ForeColor = c;
            button2.ForeColor = c;
            button5.ForeColor = c;
            button6.ForeColor = c;
            button8.ForeColor = c;
            button12.ForeColor = c;
            button13.ForeColor = c;
            button14.ForeColor = c;
            btnRange.ForeColor = c; // 範囲選択
            btnUseBoundary.ForeColor = c; // 用途境界線
        }

        //===============================
        //   セットアクティブボタン
        //===============================
        private void SetActiveButton(Button btn)
        {
            //ひとつ前までアクティブだったボタンを元に戻す処理
            if (activeButton != null)
            {
                activeButton.BackColor = SystemColors.Control;     //Back背景：普通の色
                activeButton.ForeColor = SystemColors.ControlText; //ForeColor文字の色 : 黒文字
            }

            activeButton = btn;
            activeButton.BackColor = Color.DodgerBlue;
            activeButton.ForeColor = Color.White;

            // ターゲットボタン
            // button9 敷地 button10 道路　button11 建物 
            if (btn == button9 || btn == button10 || btn == button11)
            {
                if (activeTargetButton != null && activeTargetButton != btn)
                {
                    activeTargetButton.BackColor = SystemColors.Control;
                    activeTargetButton.ForeColor = SystemColors.ControlText;
                }
                activeTargetButton = btn;
            }
            else
            {
                if (activeTargetButton != null)
                {
                    activeTargetButton.BackColor = Color.DodgerBlue;
                    activeTargetButton.ForeColor = Color.White;
                }
            }
        }

        private bool IsTargetSelected()
        {
            return currentTarget != TargetType.None;
        }

        public TargetType GetCurrentTarget()
        {
            return currentTarget;
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;
            if (currentTarget == TargetType.Road) return;

            moveType = MoveType.Draw;
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;
            if (currentTarget == TargetType.Road) return;

            moveType = MoveType.Erase;
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
        }

        public event Action? SaveClicked;
        private void Button3_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
            SaveClicked?.Invoke();
        }

        public event Action? LoadClicked;
        private void Button4_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
            LoadClicked?.Invoke();
        }

          private void Button5_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;

            SetActiveButton((Button)sender);

            moveType = MoveType.Parallel;
            ModeChanged?.Invoke();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;
            if (currentTarget == TargetType.Road) return;

            moveType = MoveType.Corner;
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            SetActiveButton((Button)sender);
            PrintClicked?.Invoke();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;
            if (currentTarget == TargetType.Road) return;

            moveType = MoveType.Dimension;
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
        }
        private void Button12_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;
            if (currentTarget == TargetType.Road) return;

            moveType = MoveType.DrawCircle;
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
        }
        private void Button13_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;
            if (currentTarget == TargetType.Road) return;

            moveType = MoveType.Trim;
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;
            if (currentTarget == TargetType.Road) return;

            moveType = MoveType.Rotate;     //回転
            SetActiveButton((Button)sender);
            ModeChanged?.Invoke();
        }


        //---------カテゴリ ターゲット---------
        //  button9    Site
        //  button10   Road
        //  button11   Building
        //-----------------------------------

        private void button9_Click(object sender, EventArgs e)
        {
            currentTarget = TargetType.Site;
            SetActiveButton((Button)sender);
            SetToolButtonsNormalColor();
            SetActiveButton(button1);

            moveType = MoveType.Draw;
            TargetChanged?.Invoke(currentTarget);
            ModeChanged?.Invoke();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            currentTarget = TargetType.Road;     // 道路
            SetActiveButton((Button)sender);
            SetToolButtonsInitialColor();

            // 用途境OFF
            isUseBoundaryMode = false;
            btnUseBoundary.BackColor = SystemColors.Control;
            btnUseBoundary.ForeColor = SystemColors.ControlLight;

            moveType = MoveType.None;
            TargetChanged?.Invoke(currentTarget);
            ModeChanged?.Invoke();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            currentTarget = TargetType.Building;
            SetActiveButton((Button)sender);
            SetToolButtonsNormalColor();

            // 例外処理　用途境ボタン表示
            btnUseBoundary.ForeColor = SystemColors.ControlLight;

            SetActiveButton(button1);

            // 用途境OFF
            isUseBoundaryMode = false;
            btnUseBoundary.BackColor = SystemColors.Control;
            btnUseBoundary.ForeColor = SystemColors.ControlLight;

            moveType = MoveType.Draw;
            TargetChanged?.Invoke(currentTarget);
            ModeChanged?.Invoke();
        }

        public void SelectDrawMode()
        {
            button1.PerformClick();
        }

        public void SelectEraseMode()
        {
            button2.PerformClick();
        }

        public void SelectParallelMode()
        {
            button5.PerformClick();
        }

        public void SelectCornerMode()
        {
            button6.PerformClick();
        }

        public void SelectDimensionMode()
        {
            button8.PerformClick();
        }
        public void SelectCircleMode()
        {
            button12.PerformClick();
        }
        public void SelectTrimMode()
        {
            button13.PerformClick();
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            BackgroundToggleClicked?.Invoke();
        }

        private void btnUseBoundary_Click(object sender, EventArgs e)
        {
            if (currentTarget != TargetType.Site)
                return;

            isUseBoundaryMode = !isUseBoundaryMode;

            if (isUseBoundaryMode)
            {
                btnUseBoundary.BackColor = Color.Orange;
                btnUseBoundary.ForeColor = Color.White;
            }
            else
            {
                btnUseBoundary.BackColor = SystemColors.Control;
                btnUseBoundary.ForeColor = SystemColors.ControlText;
            }

        }

 
        private void btnRange_Click(object sender, EventArgs e)
        {
            if (!IsTargetSelected()) return;

            if (currentTarget == TargetType.Road) return;

            SetActiveButton((Button)sender);

            moveType = MoveType.RangeSelect;

            ModeChanged?.Invoke();
        }
    }
}
