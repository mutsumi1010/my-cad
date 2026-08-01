namespace WinFormsApp17
{
    partial class FormKariSenSettei
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            textBox2 = new TextBox();
            label4 = new Label();
            buttonOK = new Button();
            SuspendLayout();
            // 
            // label1 (隣地境界線からの離れ)
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic UI", 12F);
            label1.Location = new Point(20, 25);
            label1.Name = "label1";
            label1.Size = new Size(180, 28);
            label1.TabIndex = 0;
            label1.Text = "隣地境界線からの離れ";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Yu Gothic UI", 12F);
            textBox1.Location = new Point(210, 22);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(90, 32);
            textBox1.TabIndex = 1;
            textBox1.TextAlign = HorizontalAlignment.Right;
            // 
            // label2 (mm)
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI", 12F);
            label2.Location = new Point(308, 25);
            label2.Name = "label2";
            label2.Size = new Size(42, 28);
            label2.TabIndex = 2;
            label2.Text = "mm";
            // 
            // label3 (道路境界線からの離れ)
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 12F);
            label3.Location = new Point(20, 70);
            label3.Name = "label3";
            label3.Size = new Size(180, 28);
            label3.TabIndex = 3;
            label3.Text = "道路境界線からの離れ";
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Yu Gothic UI", 12F);
            textBox2.Location = new Point(210, 67);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(90, 32);
            textBox2.TabIndex = 4;
            textBox2.TextAlign = HorizontalAlignment.Right;
            // 
            // label4 (mm)
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Yu Gothic UI", 12F);
            label4.Location = new Point(308, 70);
            label4.Name = "label4";
            label4.Size = new Size(42, 28);
            label4.TabIndex = 5;
            label4.Text = "mm";
            // 
            // buttonOK
            // 
            buttonOK.Font = new Font("Yu Gothic UI", 12F);
            buttonOK.Location = new Point(210, 115);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(90, 40);
            buttonOK.TabIndex = 6;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // FormKariSenSettei
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(380, 180);
            Controls.Add(buttonOK);
            Controls.Add(label4);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormKariSenSettei";
            StartPosition = FormStartPosition.CenterParent;
            Text = "仮線設定";
            Load += FormKariSenSettei_Load;   // ← 追加
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private Label label3;
        private TextBox textBox2;
        private Label label4;
        private Button buttonOK;
    }
}