using System.Data;

namespace WinFormsApp17
{
    public partial class Form5 : Form
    {
        private readonly LineManager lineManager;

        private decimal targetSiteAreaM;

        private List<Control> siteOnlyControls;
        private List<Control> buildingHiddenControls;
        private List<Control> buildingOnlyControls;

        public Action? RequestRedraw { get; set; }
        public Action? RequestKariSen { get; set; }

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

            textBox1.TextAlign = HorizontalAlignment.Right;
            textBox2.TextAlign = HorizontalAlignment.Right;
            textBox3.TextAlign = HorizontalAlignment.Right;
            textBox4.TextAlign = HorizontalAlignment.Right;
            textBox5.TextAlign = HorizontalAlignment.Right;
            textBox6.TextAlign = HorizontalAlignment.Right;
            textBox7.TextAlign = HorizontalAlignment.Right;
            textBox8.TextAlign = HorizontalAlignment.Right;
            textBox9.TextAlign = HorizontalAlignment.Right;
            textBox10.TextAlign = HorizontalAlignment.Right;
            textBox11.TextAlign = HorizontalAlignment.Right;
            textBox12.TextAlign = HorizontalAlignment.Right;
            textBox13.TextAlign = HorizontalAlignment.Right;
            textBox14.TextAlign = HorizontalAlignment.Right;
            textBox16.TextAlign = HorizontalAlignment.Right;
            textBox17.TextAlign = HorizontalAlignment.Right;
            textBox18.TextAlign = HorizontalAlignment.Right;

            comboBox1.SelectedIndex = -1;

            // 「用途地域」「道路」ブロック（敷地モードのみ表示）
            siteOnlyControls = new List<Control>
            {
                label12, comboBox1, label23, textBox10, label24,
                label17, textBox9, label18,
                label21, textBox11, label25, label26,
                label27, comboBox2, label30, textBox12, label32,
                label28, textBox14, label31,
                label29, textBox16, label33,
                label34,
                label13, textBox7, label14,
                label15, textBox8, label16
            };

            // 建物モードでは不要な項目（敷地_面積計算／面積合わせ／建蔽率）
            buildingHiddenControls = new List<Control>
            {
                button1, textBox1, label1,     // 敷地_面積計算
                button2, textBox13, label22,   // 敷地⇒面積合わせ
                label4, textBox3, label10,     // 建蔽率
                label5, textBox4, label11,     // [採用] 容積率
                label19                        // 上の区切り線
            };

            // 建物モードでのみ表示する項目
            buildingOnlyControls = new List<Control>
            {
                label35, textBox17, label36,
                label37, textBox18, label38,
                button3,
                button4, textBox19, label39
            };

            // 起動直後は敷地モード扱いにしておく
            SetTarget(TargetType.Site);


            //TESTCODE
           // label2.Text = "敷地面積【2】";
           // label21.Text = "敷地面積【21】";
            //label29.Text = "敷地面積【29】";
        }

        //==========================
        //  ターゲット切り替え
        //==========================
        /*   public void SetTarget(TargetType target)
           {
               bool isBuilding = target == TargetType.Building;

               foreach (var c in siteOnlyControls)
                   c.Visible = !isBuilding;

               foreach (var c in buildingHiddenControls)
                   c.Visible = !isBuilding;

               foreach (var c in buildingOnlyControls)
                   c.Visible = isBuilding;

               if (isBuilding)
               {
                   // 建物モード用の並び順（上に詰める）
                   // 敷地面積 → 許容建築面積 → 建築面積（実際） → 許容延床面積 → 延床面積（実際）
                   label2.Location = new Point(87, 40);
                   textBox2.Location = new Point(244, 40);
                   label3.Location = new Point(346, 34);

                   label6.Location = new Point(87, 73);
                   textBox5.Location = new Point(244, 73);
                   label8.Location = new Point(346, 67);

                   label35.Location = new Point(87, 106);
                   textBox17.Location = new Point(244, 106);
                   label36.Location = new Point(346, 100);

                   label7.Location = new Point(87, 139);
                   textBox6.Location = new Point(244, 139);
                   label9.Location = new Point(346, 133);

                   label37.Location = new Point(87, 172);
                   textBox18.Location = new Point(244, 172);
                   label38.Location = new Point(346, 166);
               }
               else
               {
                   // 敷地モードの元の並び順
                   label2.Location = new Point(87, 122);
                   textBox2.Location = new Point(244, 122);
                   label3.Location = new Point(346, 120);

                   label6.Location = new Point(87, 220);
                   textBox5.Location = new Point(244, 222);
                   label8.Location = new Point(346, 216);

                   label7.Location = new Point(87, 252);
                   textBox6.Location = new Point(244, 255);
                   label9.Location = new Point(346, 249);

                   label35.Location = new Point(87, 310);
                   textBox17.Location = new Point(244, 312);
                   label36.Location = new Point(346, 306);

                   label37.Location = new Point(87, 350);
                   textBox18.Location = new Point(244, 352);
                   label38.Location = new Point(346, 346);
               }
           }*/

        //==========================
        //  ターゲット切り替え
        //==========================
        public void SetTarget(TargetType target)
        {
            bool isBuilding = target == TargetType.Building;

            foreach (var c in siteOnlyControls)
                c.Visible = !isBuilding;

            foreach (var c in buildingHiddenControls)
                c.Visible = !isBuilding;

            foreach (var c in buildingOnlyControls)
                c.Visible = isBuilding;

            if (isBuilding)
            {
                // 建物モード用の並び順（上に詰める）
                label2.Location = new Point(30, 40);
                textBox2.Location = new Point(188, 40);
                label3.Location = new Point(295, 34);

                label6.Location = new Point(30, 73);
                textBox5.Location = new Point(188, 73);
                label8.Location = new Point(295, 67);

                label35.Location = new Point(30, 106);
                textBox17.Location = new Point(188, 106);
                label36.Location = new Point(295, 100);

                label7.Location = new Point(30, 139);
                textBox6.Location = new Point(188, 139);
                label9.Location = new Point(295, 133);

                label37.Location = new Point(30, 172);
                textBox18.Location = new Point(188, 172);
                label38.Location = new Point(295, 166);
            }
            else
            {
                // 敷地モードの並び順
                label2.Location = new Point(30, 122);
                textBox2.Location = new Point(188, 122);
                label3.Location = new Point(295, 120);

                label6.Location = new Point(30, 220);
                textBox5.Location = new Point(188, 222);
                label8.Location = new Point(295, 216);

                label7.Location = new Point(30, 252);
                textBox6.Location = new Point(188, 255);
                label9.Location = new Point(295, 249);

                label35.Location = new Point(30, 310);
                textBox17.Location = new Point(188, 312);
                label36.Location = new Point(295, 306);

                label37.Location = new Point(30, 350);
                textBox18.Location = new Point(188, 352);
                label38.Location = new Point(295, 346);
            }
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

        //------------------------------
        // ボタン3クリック（仮線作図ボタン）
        //------------------------------
        private void button3_Click(object sender, EventArgs e)
        {
            RequestKariSen?.Invoke();
        }
        private PointDec ScalePointFromCenter(PointDec p, PointDec center, decimal scale)
        {
            return new PointDec
            {
                x = center.x + (p.x - center.x) * scale,
                y = center.y + (p.y - center.y) * scale
            };
        }

        //==========================
        // 入力データ保存
        //==========================
        public void SaveInputDataTo(CadProjectData data)
        {
            // 上部
            data.CalculatedSiteArea = textBox1.Text;   // 敷地_面積計算
            data.TargetSiteArea = textBox13.Text;      // 敷地⇒面積合わせ

            // 全体計算
            data.SiteArea = textBox2.Text;                 // 敷地面積
            data.BuildingCoverageRatio = textBox3.Text;    // 建蔽率
            data.AdoptedFloorAreaRatio = textBox4.Text;    // [採用] 容積率
            data.AllowedBuildingArea = textBox5.Text;      // 許容建築面積
            data.AllowedFloorArea = textBox6.Text;         // 許容延床面積

            // 用途地域①
            data.Zoning1 = comboBox1.Text;                         // 用途地域①
            data.Zoning1BuildingCoverageRatio = textBox10.Text;    // 建蔽率
            data.Zoning1FloorAreaRatio = textBox9.Text;            // 容積率
            data.Zoning1SiteArea = textBox11.Text;                 // 敷地面積

            // 用途地域②
            data.Zoning2 = comboBox2.Text;                         // 用途地域②
            data.Zoning2BuildingCoverageRatio = textBox12.Text;    // 建蔽率
            data.Zoning2FloorAreaRatio = textBox14.Text;           // 容積率
            data.Zoning2SiteArea = textBox16.Text;                 // 敷地面積

            // 道路
            data.RoadWidth = textBox7.Text;                 // 道路幅員
            data.RoadFloorAreaRatio = textBox8.Text;        // 道路による容積率
        }

        //==========================
        // 入力データ読込
        //==========================
        public void LoadInputDataFrom(CadProjectData data)
        {
            // 上部
            textBox1.Text = data.CalculatedSiteArea;
            textBox13.Text = data.TargetSiteArea;

            // 全体計算
            textBox2.Text = data.SiteArea;
            textBox3.Text = data.BuildingCoverageRatio;
            textBox4.Text = data.AdoptedFloorAreaRatio;
            textBox5.Text = data.AllowedBuildingArea;
            textBox6.Text = data.AllowedFloorArea;

            // 用途地域①
            comboBox1.Text = data.Zoning1;
            textBox10.Text = data.Zoning1BuildingCoverageRatio;
            textBox9.Text = data.Zoning1FloorAreaRatio;
            textBox11.Text = data.Zoning1SiteArea;

            // 用途地域②
            comboBox2.Text = data.Zoning2;
            textBox12.Text = data.Zoning2BuildingCoverageRatio;
            textBox14.Text = data.Zoning2FloorAreaRatio;
            textBox16.Text = data.Zoning2SiteArea;

            // 道路
            textBox7.Text = data.RoadWidth;
            textBox8.Text = data.RoadFloorAreaRatio;
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {
            label20.Text = "";
            label20.BorderStyle = BorderStyle.Fixed3D;
            label20.Height = 2;
            label20.Width = 300;
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 建築面積＿計算
            try
            {
                var buildingLines =
                    lineManager.decFile
                        .Where(l => l.Layer == (int)LineManager.LayerType.Building)
                        .Select(l => (l.start, l.end))
                        .ToList();

                if (buildingLines.Count < 3)
                {
                    MessageBox.Show("建物の線が足りません。");
                    return;
                }

                decimal area = AreaCalculator.CalcAreaFromSegments(buildingLines);
                decimal areaM = area / 1000000m;
                areaM = Math.Round(areaM, 2, MidpointRounding.AwayFromZero);
                textBox19.Text = areaM.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "建築面積の計算に失敗しました。\n" + ex.Message,
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}