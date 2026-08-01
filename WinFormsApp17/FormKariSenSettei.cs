namespace WinFormsApp17
{
    public partial class FormKariSenSettei : Form
    {
        private readonly MakeKariSen makeKariSen;

        public FormKariSenSettei(MakeKariSen makeKariSen)
        {
            InitializeComponent();
            this.makeKariSen = makeKariSen;
        }

        private void FormKariSenSettei_Load(object sender, EventArgs e)
        {
            // 前回の値を表示
            textBox1.Text = makeKariSen.SetbackFromSite.ToString();
            textBox2.Text = makeKariSen.SetbackFromRoad.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox1.Text, out decimal siteSetback))
            {
                MessageBox.Show("隣地境界線からの離れを数値で入力してください。");
                return;
            }

            if (!decimal.TryParse(textBox2.Text, out decimal roadSetback))
            {
                MessageBox.Show("道路境界線からの離れを数値で入力してください。");
                return;
            }

            makeKariSen.SetbackFromSite = siteSetback;
            makeKariSen.SetbackFromRoad = roadSetback;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}