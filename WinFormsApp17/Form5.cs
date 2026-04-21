using System.Data;

namespace WinFormsApp17
{
    public partial class Form5 : Form
    {
        private readonly LineManager lineManager;

        private decimal targetSiteAreaM;

        public Action? RequestRedraw { get; set; }

        public Form5(LineManager lineManager)
        {
            InitializeComponent();
            this.lineManager = lineManager;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            textBox2.TextChanged += CalcKenpeiritsu;
            textBox3.TextChanged += CalcKenpeiritsu;

            textBox2.TextChanged += CalcYousekiritsu;
            textBox4.TextChanged += CalcYousekiritsu;

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;


            textBox2.Leave += textBox2_Leave;       //　敷地面積 ##,0.00

            textBox2.TextAlign = HorizontalAlignment.Right;
            textBox3.TextAlign = HorizontalAlignment.Right;
            textBox4.TextAlign = HorizontalAlignment.Right;
            textBox5.TextAlign = HorizontalAlignment.Right;
            textBox6.TextAlign = HorizontalAlignment.Right;

            comboBox1.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox7_TextChanged(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // 自動生成
        }

        //------------------------
        //   ボタン1　クリック
        //------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var siteLines =
                    lineManager.decFile
                        .Where(l => l.Layer == (int)LineManager.LayerType.Site)
                        .Select(l => (l.start, l.end))
                        .ToList();

                if (siteLines.Count < 3)
                {
                    MessageBox.Show("敷地の線が足りません。");
                    return;
                }

                decimal area = AreaCalculator.CalcAreaFromSegments(siteLines);
                decimal areaM = area / 1000000m;
                //areaM = Math.Floor(areaM * 100) / 100;
                areaM = Math.Round(areaM, 2, MidpointRounding.AwayFromZero);
                textBox1.Text = areaM.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "面積計算に失敗しました。\n" + ex.Message,
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CalcKenpeiritsu(object sender, EventArgs e)
        {

            if (decimal.TryParse(textBox2.Text, out decimal siteArea) &&
                decimal.TryParse(textBox3.Text, out decimal kenpeiritsu))
            {
                decimal yuryoKenchiku = siteArea * kenpeiritsu / 100m;
                yuryoKenchiku = Math.Floor(yuryoKenchiku * 100) / 100;
                textBox5.Text = yuryoKenchiku.ToString("#,##0.00");
            }
            else
            {
                textBox5.Text = "";
            }
        }

        private void CalcYousekiritsu(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox2.Text, out decimal siteArea) &&
                decimal.TryParse(textBox4.Text, out decimal yousekiritsu))
            {
                decimal yuryoEnnka = siteArea * yousekiritsu / 100m;
                yuryoEnnka = Math.Floor(yuryoEnnka * 100) / 100;
                textBox6.Text = yuryoEnnka.ToString("#,##0.00");
            }
            else
            {
                textBox6.Text = "";
            }
        }


        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox2.Text, out decimal siteArea))
            {
                textBox2.Text = siteArea.ToString("#,##0.00");
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {


            if (comboBox1.SelectedItem == null)
            {
                textBox8.Text = "";
                return;
            }

            if (decimal.TryParse(textBox7.Text, out decimal roadWidth))
            {
                string youto = comboBox1.SelectedItem.ToString() ?? "";
                decimal k = IsResidential(youto) ? 0.4m : 0.6m;
                decimal result = roadWidth * k * 100m;
                textBox8.Text = result.ToString("#,##0.##");
            }
            else
            {
                textBox8.Text = "";
            }

        }

        private bool IsResidential(string youto)
        {
            switch (youto)
            {
                case "第一種低層住居専用地域":
                case "第二種低層住居専用地域":
                case "第一種中高層住居専用地域":
                case "第二種中高層住居専用地域":
                case "第一種住居地域":
                case "第二種住居地域":
                case "準住居地域":
                case "田園住居地域":
                    return true;

                default:
                    return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var siteLines =
                    lineManager.decFile
                        .Where(l => l.Layer == (int)LineManager.LayerType.Site)
                        .Select(l => (l.start, l.end))
                        .ToList();

            if (siteLines.Count < 3)
            {
                MessageBox.Show("敷地の線が足りません。");
                return;
            }

            if (!decimal.TryParse(textBox13.Text, out targetSiteAreaM))
            {
                MessageBox.Show("面積の数字を入力してください");
                return;
            }
            decimal targetSiteArea = targetSiteAreaM * 1000000;
            decimal motoSiteArea = AreaCalculator.CalcAreaFromSegments(siteLines);

            if (motoSiteArea == 0)
            {
                MessageBox.Show("元の敷地面積が0です。敷地形状を確認してください。");
                return;
            }

            decimal areaScale = targetSiteArea / motoSiteArea;
            decimal scale = (decimal)Math.Sqrt((double)areaScale);

            var centroid = CentroidCalculator.CalcCentroidFromSegments(siteLines);

            var scaledSiteLines = siteLines
                .Select(seg => (
                A: ScalePointFromCenter(seg.start, centroid, scale),
                B: ScalePointFromCenter(seg.end, centroid, scale)
                ))
                .ToList();

            ///////////////////////////////////
            ///
            // 元の点（重複除去して3点だけ取る）
           /*var originalPoints = siteLines
                 .SelectMany(seg => new[] { seg.start, seg.end })
            //     .Distinct()
                .ToList();

         /*   // 変換後の点
            var scaledPoints = scaledSiteLines
                .SelectMany(seg => new[] { seg.A, seg.B })
                .Distinct()
                .ToList();

            // 表示用文字列作成
            string msg = "【元の点】\n";
            for (int i = 0; i < originalPoints.Count; i++)
            {
                msg += $"P{i}: ({originalPoints[i].x}, {originalPoints[i].y})\n";
            }

            msg += $"\n【重心】\n({centroid.x}, {centroid.y})\n";

            msg += "\n【変換後の点】\n";
            for (int i = 0; i < scaledPoints.Count; i++)
            {
                msg += $"P{i}: ({scaledPoints[i].x}, {scaledPoints[i].y})\n";
            }

            MessageBox.Show(msg);
            */////////////////////////////////////////////////



            // Layer 1 の線を先に退避
            var oldSiteLines = lineManager.decFile
                .Where(l => l.Layer == (int)LineManager.LayerType.Site)
                .ToList();

            // 既存の Layer 1 を削除
               foreach (var line in oldSiteLines)
               {
                  lineManager.decFile.Remove(line);
               }

            // 拡大縮小後の線を追加
            foreach (var seg in scaledSiteLines)
            {
                lineManager.decFile.Add(new LineEntity
                {
                    start = seg.A,
                    end = seg.B,
                    Layer = (int)LineManager.LayerType.Site
                });
            }
          
            // 再描画
            RequestRedraw?.Invoke();

        }
        private PointDec ScalePointFromCenter(PointDec p, PointDec center, decimal scale)
        {
            return new PointDec
            {
                x = center.x + (p.x - center.x) * scale,
                y = center.y + (p.y - center.y) * scale
            };
        }
    }
}
