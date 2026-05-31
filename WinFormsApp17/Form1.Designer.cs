namespace WinFormsApp17
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelRoadMessage = new Label();
            roadlabel2 = new Label();
            Rlabel1 = new Label();
            RtextBox1 = new TextBox();
            Rlabel2 = new Label();
            RtextBox2 = new TextBox();
            menuStrip1 = new MenuStrip();
            ファイルToolStripMenuItem = new ToolStripMenuItem();
            開くToolStripMenuItem = new ToolStripMenuItem();
            上書保存ToolStripMenuItem = new ToolStripMenuItem();
            名前を付けて保存ToolStripMenuItem = new ToolStripMenuItem();
            pDF読込ToolStripMenuItem = new ToolStripMenuItem();
            ツールToolStripMenuItem = new ToolStripMenuItem();
            座標ファイルToolStripMenuItem = new ToolStripMenuItem();
            textBoxScale = new TextBox();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // labelRoadMessage
            // 
            labelRoadMessage.AutoSize = true;
            labelRoadMessage.BackColor = SystemColors.ControlLight;
            labelRoadMessage.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            labelRoadMessage.Location = new Point(12, 9);
            labelRoadMessage.Name = "labelRoadMessage";
            labelRoadMessage.Size = new Size(311, 31);
            labelRoadMessage.TabIndex = 0;
            labelRoadMessage.Text = "道路境界線をクリックしてください";
            // 
            // roadlabel2
            // 
            roadlabel2.AutoSize = true;
            roadlabel2.BackColor = SystemColors.ControlLight;
            roadlabel2.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            roadlabel2.Location = new Point(12, 58);
            roadlabel2.Name = "roadlabel2";
            roadlabel2.Size = new Size(306, 31);
            roadlabel2.TabIndex = 0;
            roadlabel2.Text = "道路幅員を入力してください(ｍ)";
            // 
            // Rlabel1
            // 
            Rlabel1.AutoSize = true;
            Rlabel1.BackColor = SystemColors.ControlLight;
            Rlabel1.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Rlabel1.Location = new Point(12, 99);
            Rlabel1.Name = "Rlabel1";
            Rlabel1.Size = new Size(112, 28);
            Rlabel1.TabIndex = 0;
            Rlabel1.Text = "端点１幅員";
            Rlabel1.Click += label1_Click;
            // 
            // RtextBox1
            // 
            RtextBox1.Font = new Font("Yu Gothic UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            RtextBox1.Location = new Point(130, 99);
            RtextBox1.Name = "RtextBox1";
            RtextBox1.Size = new Size(125, 31);
            RtextBox1.TabIndex = 1;
            // 
            // Rlabel2
            // 
            Rlabel2.AutoSize = true;
            Rlabel2.BackColor = SystemColors.ControlLight;
            Rlabel2.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Rlabel2.Location = new Point(277, 102);
            Rlabel2.Name = "Rlabel2";
            Rlabel2.Size = new Size(112, 28);
            Rlabel2.TabIndex = 0;
            Rlabel2.Text = "端点２幅員";
            Rlabel2.Click += label1_Click;
            // 
            // RtextBox2
            // 
            RtextBox2.Font = new Font("Yu Gothic UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            RtextBox2.Location = new Point(395, 102);
            RtextBox2.Name = "RtextBox2";
            RtextBox2.Size = new Size(125, 31);
            RtextBox2.TabIndex = 1;
            // 
            // menuStrip1
            // 
            menuStrip1.Font = new Font("Yu Gothic UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { ファイルToolStripMenuItem, ツールToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1092, 36);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            ファイルToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 開くToolStripMenuItem, 上書保存ToolStripMenuItem, 名前を付けて保存ToolStripMenuItem, pDF読込ToolStripMenuItem });
            ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            ファイルToolStripMenuItem.Size = new Size(83, 32);
            ファイルToolStripMenuItem.Text = "ファイル";
            // 
            // 開くToolStripMenuItem
            // 
            開くToolStripMenuItem.Name = "開くToolStripMenuItem";
            開くToolStripMenuItem.Size = new Size(262, 32);
            開くToolStripMenuItem.Text = "開く &O";
            開くToolStripMenuItem.Click += 開くToolStripMenuItem_Click;
            // 
            // 上書保存ToolStripMenuItem
            // 
            上書保存ToolStripMenuItem.Name = "上書保存ToolStripMenuItem";
            上書保存ToolStripMenuItem.Size = new Size(262, 32);
            上書保存ToolStripMenuItem.Text = "上書保存 &S";
            上書保存ToolStripMenuItem.Click += 上書保存ToolStripMenuItem_Click;
            // 
            // 名前を付けて保存ToolStripMenuItem
            // 
            名前を付けて保存ToolStripMenuItem.Name = "名前を付けて保存ToolStripMenuItem";
            名前を付けて保存ToolStripMenuItem.Size = new Size(262, 32);
            名前を付けて保存ToolStripMenuItem.Text = "名前を付けて保存 &A";
            名前を付けて保存ToolStripMenuItem.Click += 名前を付けて保存ToolStripMenuItem_Click;
            // 
            // pDF読込ToolStripMenuItem
            // 
            pDF読込ToolStripMenuItem.Name = "pDF読込ToolStripMenuItem";
            pDF読込ToolStripMenuItem.Size = new Size(262, 32);
            pDF読込ToolStripMenuItem.Text = "JPEG読込";
            pDF読込ToolStripMenuItem.Click += pDF読込ToolStripMenuItem_Click;
            // 
            // ツールToolStripMenuItem
            // 
            ツールToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 座標ファイルToolStripMenuItem });
            ツールToolStripMenuItem.Name = "ツールToolStripMenuItem";
            ツールToolStripMenuItem.Size = new Size(71, 32);
            ツールToolStripMenuItem.Text = "ツール";
            // 
            // 座標ファイルToolStripMenuItem
            // 
            座標ファイルToolStripMenuItem.Name = "座標ファイルToolStripMenuItem";
            座標ファイルToolStripMenuItem.Size = new Size(195, 32);
            座標ファイルToolStripMenuItem.Text = "座標ファイル";
            座標ファイルToolStripMenuItem.Click += 座標ファイルToolStripMenuItem_Click;
            // 
            // textBoxScale
            // 
            textBoxScale.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            textBoxScale.BackColor = SystemColors.ControlLight;
            textBoxScale.Location = new Point(130, 195);
            textBoxScale.Name = "textBoxScale";
            textBoxScale.Size = new Size(125, 27);
            textBoxScale.TabIndex = 4;
            textBoxScale.TextChanged += textBoxScale_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1092, 655);
            Controls.Add(textBoxScale);
            Controls.Add(menuStrip1);
            Controls.Add(RtextBox2);
            Controls.Add(RtextBox1);
            Controls.Add(Rlabel2);
            Controls.Add(Rlabel1);
            Controls.Add(roadlabel2);
            Controls.Add(labelRoadMessage);
            Name = "Form1";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Label labelRoadMessage;
        public Label roadlabel2;
        public Label Rlabel1;
        public TextBox RtextBox1;
        public Label Rlabel2;
        public TextBox RtextBox2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem ファイルToolStripMenuItem;
        private ToolStripMenuItem pDF読込ToolStripMenuItem;
        private ToolStripMenuItem ツールToolStripMenuItem;
        private ToolStripMenuItem 座標ファイルToolStripMenuItem;
        private ToolStripMenuItem 開くToolStripMenuItem;
        private ToolStripMenuItem 上書保存ToolStripMenuItem;
        private ToolStripMenuItem 名前を付けて保存ToolStripMenuItem;
        private TextBox textBoxScale;
    }
}
