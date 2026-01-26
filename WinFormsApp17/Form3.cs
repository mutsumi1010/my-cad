using System;
using System.Windows.Forms;


namespace WinFormsApp17
{
    public partial class Form3 : Form
    {
        public decimal OffsetValue { get; private set; }

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out decimal v))
            {
                OffsetValue = v;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("数値を入力してください");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
