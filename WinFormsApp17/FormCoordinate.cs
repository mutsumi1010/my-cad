using System.Text;

namespace WinFormsApp17
{
    public partial class FormCoordinate : Form
    {
        public FormCoordinate()
        {
            InitializeComponent();
            textBox1.Font = new Font("MS UI Gothic", 20);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";
            sfd.Title = "座標ファイルを保存";
            sfd.FileName = "coordinate.txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, textBox1.Text, Encoding.UTF8);

                MessageBox.Show("保存しました");
            }
            //this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";
            ofd.Title = "座標ファイルを読み込む";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string text = File.ReadAllText(ofd.FileName, Encoding.UTF8);

                textBox1.Text = text;

                // MessageBox.Show("読み込みました");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var lines = textBox1.Text.Split(
                 new[] { "\r\n", "\n" },
                 StringSplitOptions.RemoveEmptyEntries);

            List<PointDec> points = new List<PointDec>();

            foreach (var line in lines)
            {
                var parts = line.Trim().Split(
                    new[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                decimal x = decimal.Parse(parts[0]) * 1000m;
                decimal y = -decimal.Parse(parts[1]) * 1000m;

                points.Add(new PointDec(x, y));
            }
            //Form1 f1 = (Form1)this.Owner;
            Form1? f1 = this.Owner as Form1;
            if (f1 == null)
            {
                MessageBox.Show("Form1から開かれていません。");
                return;
            }

            AddSiteLinesCentered(f1, points);
        }

        private void AddSiteLinesCentered(Form1 f1, List<PointDec> points)
        {
            if (points.Count < 2)
                return;

            decimal minX = points.Min(p => p.x);
            decimal maxX = points.Max(p => p.x);
            decimal minY = points.Min(p => p.y);
            decimal maxY = points.Max(p => p.y);

            decimal shapeCenterX = (minX + maxX) / 2m;
            decimal shapeCenterY = (minY + maxY) / 2m;

            PointDec screenCenterWorld = f1.ScreenToWorld(
                new PointF(f1.ClientSize.Width / 2f, f1.ClientSize.Height / 2f)
            );

            decimal moveX = screenCenterWorld.x - shapeCenterX;
            decimal moveY = screenCenterWorld.y - shapeCenterY;

            for (int i = 0; i < points.Count - 1; i++)
            {
                PointDec start = new PointDec(
                    points[i].x + moveX,
                    points[i].y + moveY
                );

                PointDec end = new PointDec(
                    points[i + 1].x + moveX,
                    points[i + 1].y + moveY
                );

                f1.lineManager.AddSiteLine(start, end);
            }

            f1.Invalidate();
            this.Close();
        }

    }
}
