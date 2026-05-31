namespace WinFormsApp17
{
    partial class FormRoad
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textWidth1 = new TextBox();
            textWidth2 = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ControlLight;
            label1.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label1.Location = new Point(53, 29);
            label1.Name = "label1";
            label1.Size = new Size(345, 31);
            label1.TabIndex = 0;
            label1.Text = "道路境界線をクリックしてください(m)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.ControlLight;
            label2.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label2.Location = new Point(60, 144);
            label2.Name = "label2";
            label2.Size = new Size(77, 28);
            label2.TabIndex = 1;
            label2.Text = "幅員 １";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = SystemColors.ControlLight;
            label3.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label3.Location = new Point(299, 144);
            label3.Name = "label3";
            label3.Size = new Size(77, 28);
            label3.TabIndex = 1;
            label3.Text = "幅員 ２";
            //label3.Click += this.label3_Click;
            // 
            // textWidth1
            // 
            textWidth1.Location = new Point(143, 144);
            textWidth1.Name = "textWidth1";
            textWidth1.Size = new Size(125, 27);
            textWidth1.TabIndex = 2;
            textWidth1.Click += textWidth1_Click;
            textWidth1.TextChanged += textWidth1_TextChanged;
            // 
            // textWidth2
            // 
            textWidth2.Location = new Point(382, 145);
            textWidth2.Name = "textWidth2";
            textWidth2.Size = new Size(125, 27);
            textWidth2.TabIndex = 2;
            textWidth2.Click += textWidth2_Click;
            // 
            // FormRoad
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textWidth2);
            Controls.Add(textWidth1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "FormRoad";
            Text = "FormRoad";
            Load += FormRoad_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textWidth1;
        private TextBox textWidth2;
    }
}