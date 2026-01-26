namespace WinFormsApp17
{
    public partial class Form4 : Form
    {
        public event Action<decimal>? RadiusEntered;

        public Form4()
        {
            InitializeComponent();
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

        private void radiusBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (decimal.TryParse(radiusBox.Text, out var r))
                {
                    RadiusEntered?.Invoke(r);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void radiusBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
