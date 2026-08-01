namespace WinFormsApp17
{
    public enum RoadIntersectionType
    {
        None,
        Cross,
        THorizontal,
        TVertical,
        LShape
    }
    public partial class FormSelectIntersection : Form
    {
        public string selectedRoadType = "";
        public event Action? RoadPanelClicked;

        // 交差点タイプ
        public RoadIntersectionType SelectedIntersectionType { get; private set; }
          = RoadIntersectionType.None;

        public event Action<RoadIntersectionType>? IntersectionTypeChanged;

        public FormSelectIntersection()
        {
            InitializeComponent();

            panelCross.Click += panelCross_Click;
            panelTHorizontal.Click += panelTHorizontal_Click;
            panelTVertical.Click += panelTVertical_Click;
            panelLShape.Click += panelLShape_Click;

            panelCross.BackColor = Color.White;
            panelTHorizontal.BackColor = Color.White;
            panelTVertical.BackColor = Color.White;
            panelLShape.BackColor = Color.White;

            //panelCross.BorderStyle = BorderStyle.FixedSingle;
        }

 
        private void panelCross_Paint(object sender, PaintEventArgs e)
        {
            int w = panelCross.ClientSize.Width;
            int h = panelCross.ClientSize.Height;

            int cx = w / 2;
            int cy = h / 2;
            int lenX = w / 3;
            int lenY = h / 3;

            using var pen = new Pen(Color.Black, 4);

            e.Graphics.DrawLine(pen, cx - lenX, cy, cx + lenX, cy);
            e.Graphics.DrawLine(pen, cx, cy - lenY, cx, cy + lenY);
        }

        private void panelTHorizontal_Paint(object sender, PaintEventArgs e)
        {
            int w = panelTHorizontal.ClientSize.Width;
            int h = panelTHorizontal.ClientSize.Height;

            int cx = w / 2;
            int cy = h / 2;
            int lenX = w / 4;
            int lenY = h / 4;

            using var pen = new Pen(Color.Black, 4);

            e.Graphics.DrawLine(pen, cx - lenX, cy - lenY, cx + lenX, cy - lenY);
            e.Graphics.DrawLine(pen, cx, cy - lenY, cx, cy + lenY);
        }

        private void panelTVertical_Paint(object sender, PaintEventArgs e)
        {
            int w = panelTVertical.ClientSize.Width;
            int h = panelTVertical.ClientSize.Height;

            int cx = w / 2;
            int cy = h / 2;
            int lenX = w / 4;
            int lenY = h / 4;

            using var pen = new Pen(Color.Black, 4);

            e.Graphics.DrawLine(pen, cx - lenX, cy, cx + lenX, cy);
            e.Graphics.DrawLine(pen, cx + lenX, cy - lenY, cx + lenX, cy + lenY);
        }

        private void panelLShape_Paint(object sender, PaintEventArgs e)
        {
            int w = panelLShape.ClientSize.Width;
            int h = panelLShape.ClientSize.Height;

            int cx = w / 2;
            int cy = h / 2;

            int lenX = w / 3;
            int lenY = h / 4;

            int x = cx - w / 6;   // 縦線のX位置
            int y = cy + h / 4;   // 横線のY位置

            using var pen = new Pen(Color.Black, 4);

            // 縦線
            e.Graphics.DrawLine(pen, x, cy - lenY, x, y);

            // 横線
            e.Graphics.DrawLine(pen, x, y, cx + lenX, y);
        }

        //============================
        //  SelectedRoadPanel
        //============================
        private void SelectRoadPanel(
           string roadType,
           RoadIntersectionType intersectionType)
        {
            // 選択中のパネルをもう一度押したら解除
            if (selectedRoadType == roadType)
            {
                selectedRoadType = "";
                SelectedIntersectionType = RoadIntersectionType.None;
            }
            else
            {
                selectedRoadType = roadType;
                SelectedIntersectionType = intersectionType;
            }

            panelCross.BackColor =
                selectedRoadType == "Cross" ? Color.LightBlue : Color.White;

            panelTHorizontal.BackColor =
                selectedRoadType == "THorizontal" ? Color.LightBlue : Color.White;

            panelTVertical.BackColor =
                selectedRoadType == "TVertical" ? Color.LightBlue : Color.White;

            panelLShape.BackColor =
                selectedRoadType == "LShape" ? Color.LightBlue : Color.White;

            RoadPanelClicked?.Invoke();
            IntersectionTypeChanged?.Invoke(SelectedIntersectionType);
        }


        //------------------------------
        //  パネルクリック
        //------------------------------
  
        private void panelCross_Click(object sender, EventArgs e)
        {
            SelectRoadPanel(
                "Cross",
                RoadIntersectionType.Cross);
        }

        private void panelTHorizontal_Click(object sender, EventArgs e)
        {
            SelectRoadPanel(
                "THorizontal",
                RoadIntersectionType.THorizontal);
        }

        private void panelTVertical_Click(object sender, EventArgs e)
        {
            SelectRoadPanel(
                "TVertical",
                RoadIntersectionType.TVertical);
        }

        private void panelLShape_Click(object sender, EventArgs e)
        {
            SelectRoadPanel(
                "LShape",
                RoadIntersectionType.LShape);
        }


        //------------------------------
        //  セレクトクリア
        //------------------------------

        public void ClearSelection()
        {
            selectedRoadType = "";
            SelectedIntersectionType = RoadIntersectionType.None;

            panelCross.BackColor = Color.White;
            panelTHorizontal.BackColor = Color.White;
            panelTVertical.BackColor = Color.White;
            panelLShape.BackColor = Color.White;

            IntersectionTypeChanged?.Invoke(SelectedIntersectionType);
        }

    }
}
