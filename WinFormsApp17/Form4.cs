namespace WinFormsApp17
{
    public partial class Form4 : Form
    {
      
        // 現在入力されている半径（候補）
        private decimal? currentRadius = null;

        public Form4()
        {
            InitializeComponent();
            radiusBox.TextChanged += radiusBox_TextChanged;
            radiusBox.KeyDown += radiusBox_KeyDown;

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.None;
            this.BackColor = Color.FromArgb(230, 230, 230);
            this.AutoScaleMode = AutoScaleMode.None;
            label1.BackColor = Color.FromArgb(230, 230, 230);
            
            // フォーカス設定
            this.Shown += (s, e) =>
            {
                radiusBox.Focus();
                radiusBox.SelectAll();
            };
        }


        // エンター（ピコ音出さない）
        private void radiusBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
            }
        }

        // 数字が入力されたら「候補値」として保持するだけ
        private void radiusBox_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(radiusBox.Text, out var r))
            {
                currentRadius = r;
            }
            else
            {
                currentRadius = null;
            }
        }

        // Form1（円の中心クリック）から呼ばれる
        // 今入力されている半径を取得する
        public bool TryGetRadius(out decimal radius)
        {
            if (currentRadius.HasValue)
            {
                radius = currentRadius.Value;
                return true;
            }

            radius = 0;
            return false;
        }

        // 円を1つ描いたあと、次の入力に備える
        public void ResetForNextCircle()
        {
            radiusBox.Focus();
            radiusBox.SelectAll();
        }
    }
}
