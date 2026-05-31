namespace WinFormsApp17
{
    partial class Form5
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
            button1 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            textBox2 = new TextBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            textBox4 = new TextBox();
            label6 = new Label();
            label7 = new Label();
            textBox5 = new TextBox();
            textBox6 = new TextBox();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            comboBox1 = new ComboBox();
            label13 = new Label();
            textBox7 = new TextBox();
            label14 = new Label();
            label15 = new Label();
            textBox8 = new TextBox();
            label16 = new Label();
            label17 = new Label();
            textBox9 = new TextBox();
            label18 = new Label();
            textBox3 = new TextBox();
            button2 = new Button();
            textBox13 = new TextBox();
            label22 = new Label();
            label23 = new Label();
            textBox10 = new TextBox();
            label24 = new Label();
            label19 = new Label();
            label20 = new Label();
            label21 = new Label();
            textBox11 = new TextBox();
            label25 = new Label();
            label26 = new Label();
            label27 = new Label();
            label28 = new Label();
            label29 = new Label();
            label30 = new Label();
            comboBox2 = new ComboBox();
            textBox12 = new TextBox();
            textBox14 = new TextBox();
            textBox16 = new TextBox();
            label31 = new Label();
            label32 = new Label();
            label33 = new Label();
            label34 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Yu Gothic UI", 12F);
            button1.Location = new Point(84, 22);
            button1.Name = "button1";
            button1.Size = new Size(154, 38);
            button1.TabIndex = 0;
            button1.Text = "敷地_面積計算";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.Control;
            textBox1.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 128);
            textBox1.Location = new Point(244, 22);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(96, 38);
            textBox1.TabIndex = 1;
            textBox1.TabStop = false;
            textBox1.TextAlign = HorizontalAlignment.Right;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label1.Location = new Point(346, 28);
            label1.Name = "label1";
            label1.Size = new Size(37, 31);
            label1.TabIndex = 2;
            label1.Text = "㎡";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Yu Gothic UI", 12F);
            label2.Location = new Point(87, 122);
            label2.Name = "label2";
            label2.Size = new Size(92, 28);
            label2.TabIndex = 3;
            label2.Text = "敷地面積";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(244, 122);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(96, 27);
            textBox2.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label3.Location = new Point(346, 120);
            label3.Name = "label3";
            label3.Size = new Size(37, 31);
            label3.TabIndex = 2;
            label3.Text = "㎡";
            label3.Click += label1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Yu Gothic UI", 12F);
            label4.Location = new Point(87, 155);
            label4.Name = "label4";
            label4.Size = new Size(72, 28);
            label4.TabIndex = 3;
            label4.Text = "建蔽率";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Yu Gothic UI", 12F);
            label5.Location = new Point(87, 187);
            label5.Name = "label5";
            label5.Size = new Size(129, 28);
            label5.TabIndex = 3;
            label5.Text = "[採用] 容積率";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(244, 188);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(96, 27);
            textBox4.TabIndex = 4;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Yu Gothic UI", 12F);
            label6.Location = new Point(87, 220);
            label6.Name = "label6";
            label6.Size = new Size(132, 28);
            label6.TabIndex = 3;
            label6.Text = "許容建築面積";
            label6.Click += label6_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Yu Gothic UI", 12F);
            label7.Location = new Point(87, 252);
            label7.Name = "label7";
            label7.Size = new Size(132, 28);
            label7.TabIndex = 3;
            label7.Text = "許容延床面積";
            label7.Click += label6_Click;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(244, 222);
            textBox5.Name = "textBox5";
            textBox5.ReadOnly = true;
            textBox5.Size = new Size(96, 27);
            textBox5.TabIndex = 4;
            textBox5.TextChanged += textBox5_TextChanged;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(244, 255);
            textBox6.Name = "textBox6";
            textBox6.ReadOnly = true;
            textBox6.Size = new Size(96, 27);
            textBox6.TabIndex = 4;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label8.Location = new Point(346, 216);
            label8.Name = "label8";
            label8.Size = new Size(37, 31);
            label8.TabIndex = 2;
            label8.Text = "㎡";
            label8.Click += label1_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label9.Location = new Point(346, 249);
            label9.Name = "label9";
            label9.Size = new Size(37, 31);
            label9.TabIndex = 2;
            label9.Text = "㎡";
            label9.Click += label1_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label10.Location = new Point(346, 156);
            label10.Name = "label10";
            label10.Size = new Size(37, 31);
            label10.TabIndex = 2;
            label10.Text = "％";
            label10.Click += label1_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label11.Location = new Point(346, 187);
            label11.Name = "label11";
            label11.Size = new Size(37, 31);
            label11.TabIndex = 2;
            label11.Text = "％";
            label11.Click += label1_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Yu Gothic UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label12.Location = new Point(87, 310);
            label12.Name = "label12";
            label12.Size = new Size(102, 25);
            label12.TabIndex = 3;
            label12.Text = "用途地域①";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "第一種低層住居専用地域", "第二種低層住居専用地域", "第一種中高層住居専用地域", "第二種中高層住居専用地域", "第一種住居地域", "第二種住居地域", "準住居地域", "田園住居地域", "近隣商業地域", "商業地域", "準工業地域", "工業地域", "工業専用地域" });
            comboBox1.Location = new Point(189, 310);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(194, 28);
            comboBox1.TabIndex = 5;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged_1;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Yu Gothic UI", 10.8F);
            label13.Location = new Point(87, 561);
            label13.Name = "label13";
            label13.Size = new Size(84, 25);
            label13.TabIndex = 3;
            label13.Text = "道路幅員";
            label13.Click += label12_Click;
            // 
            // textBox7
            // 
            textBox7.Location = new Point(244, 561);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(96, 27);
            textBox7.TabIndex = 4;
            textBox7.TextAlign = HorizontalAlignment.Right;
            textBox7.TextChanged += textBox7_TextChanged;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label14.Location = new Point(337, 561);
            label14.Name = "label14";
            label14.Size = new Size(37, 31);
            label14.TabIndex = 2;
            label14.Text = "ｍ";
            label14.Click += label1_Click;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Yu Gothic UI", 10.8F);
            label15.Location = new Point(87, 586);
            label15.Name = "label15";
            label15.Size = new Size(144, 25);
            label15.TabIndex = 3;
            label15.Text = "道路による容積率";
            label15.Click += label12_Click;
            // 
            // textBox8
            // 
            textBox8.Location = new Point(243, 588);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(96, 27);
            textBox8.TabIndex = 4;
            textBox8.TextAlign = HorizontalAlignment.Right;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label16.Location = new Point(337, 586);
            label16.Name = "label16";
            label16.Size = new Size(37, 31);
            label16.TabIndex = 2;
            label16.Text = "％";
            label16.Click += label1_Click;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Yu Gothic UI", 10.8F);
            label17.Location = new Point(95, 366);
            label17.Name = "label17";
            label17.Size = new Size(66, 25);
            label17.TabIndex = 3;
            label17.Text = "容積率\r\n";
            label17.Click += label12_Click;
            // 
            // textBox9
            // 
            textBox9.Location = new Point(241, 368);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(96, 27);
            textBox9.TabIndex = 4;
            textBox9.TextAlign = HorizontalAlignment.Right;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label18.Location = new Point(346, 366);
            label18.Name = "label18";
            label18.Size = new Size(37, 31);
            label18.TabIndex = 2;
            label18.Text = "％";
            label18.Click += label1_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(244, 155);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(96, 27);
            textBox3.TabIndex = 4;
            textBox3.TextAlign = HorizontalAlignment.Right;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // button2
            // 
            button2.Font = new Font("Yu Gothic UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 128);
            button2.Location = new Point(84, 66);
            button2.Name = "button2";
            button2.Size = new Size(154, 38);
            button2.TabIndex = 0;
            button2.Text = "敷地⇒面積合わせ";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox13
            // 
            textBox13.Location = new Point(244, 73);
            textBox13.Name = "textBox13";
            textBox13.Size = new Size(96, 27);
            textBox13.TabIndex = 4;
            textBox13.TextAlign = HorizontalAlignment.Right;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label22.Location = new Point(346, 73);
            label22.Name = "label22";
            label22.Size = new Size(37, 31);
            label22.TabIndex = 2;
            label22.Text = "㎡";
            label22.Click += label1_Click;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new Font("Yu Gothic UI", 10.8F);
            label23.Location = new Point(95, 341);
            label23.Name = "label23";
            label23.Size = new Size(66, 25);
            label23.TabIndex = 3;
            label23.Text = "建蔽率";
            label23.Click += label12_Click;
            // 
            // textBox10
            // 
            textBox10.Location = new Point(241, 341);
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(96, 27);
            textBox10.TabIndex = 6;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label24.Location = new Point(346, 339);
            label24.Name = "label24";
            label24.Size = new Size(37, 31);
            label24.TabIndex = 2;
            label24.Text = "％";
            label24.Click += label1_Click;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Yu Gothic UI", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 128);
            label19.Location = new Point(84, 107);
            label19.Name = "label19";
            label19.Size = new Size(283, 17);
            label19.TabIndex = 7;
            label19.Text = "-------------------------------------------------------";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Yu Gothic UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label20.Location = new Point(87, 293);
            label20.Name = "label20";
            label20.Size = new Size(278, 17);
            label20.TabIndex = 7;
            label20.Text = "------------------------------------------------------";
            label20.Click += label20_Click;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Yu Gothic UI", 10.8F);
            label21.Location = new Point(95, 394);
            label21.Name = "label21";
            label21.Size = new Size(84, 25);
            label21.TabIndex = 3;
            label21.Text = "敷地面積";
            label21.Click += label12_Click;
            // 
            // textBox11
            // 
            textBox11.Location = new Point(241, 395);
            textBox11.Name = "textBox11";
            textBox11.Size = new Size(98, 27);
            textBox11.TabIndex = 8;
            textBox11.TextChanged += textBox11_TextChanged;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label25.Location = new Point(346, 397);
            label25.Name = "label25";
            label25.Size = new Size(37, 31);
            label25.TabIndex = 2;
            label25.Text = "㎡";
            label25.Click += label1_Click;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label26.Location = new Point(346, 392);
            label26.Name = "label26";
            label26.Size = new Size(37, 31);
            label26.TabIndex = 2;
            label26.Text = "㎡";
            label26.Click += label1_Click;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Font = new Font("Yu Gothic UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label27.Location = new Point(87, 426);
            label27.Name = "label27";
            label27.Size = new Size(102, 25);
            label27.TabIndex = 3;
            label27.Text = "用途地域②";
            label27.Click += label27_Click;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Font = new Font("Yu Gothic UI", 10.8F);
            label28.Location = new Point(95, 486);
            label28.Name = "label28";
            label28.Size = new Size(66, 25);
            label28.TabIndex = 3;
            label28.Text = "容積率\r\n";
            label28.Click += label12_Click;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Font = new Font("Yu Gothic UI", 10.8F);
            label29.Location = new Point(95, 511);
            label29.Name = "label29";
            label29.Size = new Size(84, 25);
            label29.TabIndex = 3;
            label29.Text = "敷地面積";
            label29.Click += label12_Click;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Font = new Font("Yu Gothic UI", 10.8F);
            label30.Location = new Point(95, 461);
            label30.Name = "label30";
            label30.Size = new Size(66, 25);
            label30.TabIndex = 3;
            label30.Text = "建蔽率";
            label30.Click += label12_Click;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "第一種低層住居専用地域", "第二種低層住居専用地域", "第一種中高層住居専用地域", "第二種中高層住居専用地域", "第一種住居地域", "第二種住居地域", "準住居地域", "田園住居地域", "近隣商業地域", "商業地域", "準工業地域", "工業地域", "工業専用地域" });
            comboBox2.Location = new Point(189, 425);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(194, 28);
            comboBox2.TabIndex = 9;
            // 
            // textBox12
            // 
            textBox12.Location = new Point(241, 459);
            textBox12.Name = "textBox12";
            textBox12.Size = new Size(95, 27);
            textBox12.TabIndex = 10;
            // 
            // textBox14
            // 
            textBox14.Location = new Point(241, 486);
            textBox14.Name = "textBox14";
            textBox14.Size = new Size(95, 27);
            textBox14.TabIndex = 11;
            // 
            // textBox16
            // 
            textBox16.Location = new Point(241, 513);
            textBox16.Name = "textBox16";
            textBox16.Size = new Size(95, 27);
            textBox16.TabIndex = 13;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label31.Location = new Point(346, 487);
            label31.Name = "label31";
            label31.Size = new Size(37, 31);
            label31.TabIndex = 2;
            label31.Text = "％";
            label31.Click += label1_Click;
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label32.Location = new Point(346, 460);
            label32.Name = "label32";
            label32.Size = new Size(37, 31);
            label32.TabIndex = 2;
            label32.Text = "％";
            label32.Click += label1_Click;
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Font = new Font("Yu Gothic UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label33.Location = new Point(346, 513);
            label33.Name = "label33";
            label33.Size = new Size(37, 31);
            label33.TabIndex = 2;
            label33.Text = "㎡";
            label33.Click += label1_Click;
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Font = new Font("Yu Gothic UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label34.Location = new Point(87, 544);
            label34.Name = "label34";
            label34.Size = new Size(278, 17);
            label34.TabIndex = 7;
            label34.Text = "------------------------------------------------------";
            label34.Click += label20_Click;
            // 
            // Form5
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(395, 757);
            Controls.Add(textBox16);
            Controls.Add(textBox14);
            Controls.Add(textBox12);
            Controls.Add(comboBox2);
            Controls.Add(textBox11);
            Controls.Add(label34);
            Controls.Add(label20);
            Controls.Add(label19);
            Controls.Add(textBox10);
            Controls.Add(comboBox1);
            Controls.Add(textBox6);
            Controls.Add(textBox5);
            Controls.Add(textBox13);
            Controls.Add(textBox9);
            Controls.Add(textBox8);
            Controls.Add(textBox7);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label30);
            Controls.Add(label23);
            Controls.Add(label29);
            Controls.Add(label28);
            Controls.Add(label21);
            Controls.Add(label17);
            Controls.Add(label15);
            Controls.Add(label13);
            Controls.Add(label27);
            Controls.Add(label12);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label14);
            Controls.Add(label33);
            Controls.Add(label26);
            Controls.Add(label25);
            Controls.Add(label9);
            Controls.Add(label32);
            Controls.Add(label8);
            Controls.Add(label31);
            Controls.Add(label24);
            Controls.Add(label18);
            Controls.Add(label16);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label3);
            Controls.Add(label22);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form5";
            Text = "敷地";
            Load += Form5_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private TextBox textBox2;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox textBox4;
        private Label label6;
        private Label label7;
        private TextBox textBox5;
        private TextBox textBox6;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private ComboBox comboBox1;
        private Label label13;
        private TextBox textBox7;
        private Label label14;
        private Label label15;
        private TextBox textBox8;
        private Label label16;
        private Label label17;
        private TextBox textBox9;
        private Label label18;
        private TextBox textBox3;
        private Label label19;
        private Label label20;
        private Label label21;
        private TextBox textBox11;
        private TextBox textBox12;
        private Button button2;
        private TextBox textBox13;
        private Label label22;
        private Label label23;
        private TextBox textBox10;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label29;
        private Label label30;
        private ComboBox comboBox2;
        private TextBox textBox14;
        private TextBox textBox16;
        private Label label31;
        private Label label32;
        private Label label33;
        private Label label34;
    }
}