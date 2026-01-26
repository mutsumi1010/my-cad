using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp17
{
    public partial class Form5 : Form
    {
        private readonly LineManager lineManager;

        public Form5(LineManager lineManager)
        {
            InitializeComponent();
            this.lineManager = lineManager;
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

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
                areaM = Math.Floor(areaM * 100) / 100;
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
    }
}
