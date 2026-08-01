namespace WinFormsApp17
{
    partial class FormSelectIntersection
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
            btnRoadIntersection = new Button();
            panelCross = new Panel();
            panelTHorizontal = new Panel();
            panelTVertical = new Panel();
            panelLShape = new Panel();
            SuspendLayout();
            // 
            // btnRoadIntersection
            // 
            btnRoadIntersection.Font = new Font("Yu Gothic UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnRoadIntersection.Location = new Point(11, 21);
            btnRoadIntersection.Name = "btnRoadIntersection";
            btnRoadIntersection.Size = new Size(104, 39);
            btnRoadIntersection.TabIndex = 0;
            btnRoadIntersection.Text = "道路交点";
            btnRoadIntersection.UseVisualStyleBackColor = true;
            // 
            // panelCross
            // 
            panelCross.BorderStyle = BorderStyle.FixedSingle;
            panelCross.Location = new Point(11, 67);
            panelCross.Name = "panelCross";
            panelCross.Size = new Size(85, 99);
            panelCross.TabIndex = 2;
            panelCross.Paint += panelCross_Paint;
            // 
            // panelTHorizontal
            // 
            panelTHorizontal.BorderStyle = BorderStyle.FixedSingle;
            panelTHorizontal.Location = new Point(11, 172);
            panelTHorizontal.Name = "panelTHorizontal";
            panelTHorizontal.Size = new Size(85, 99);
            panelTHorizontal.TabIndex = 2;
            panelTHorizontal.Paint += panelTHorizontal_Paint;
            // 
            // panelTVertical
            // 
            panelTVertical.BorderStyle = BorderStyle.FixedSingle;
            panelTVertical.Location = new Point(11, 277);
            panelTVertical.Name = "panelTVertical";
            panelTVertical.Size = new Size(85, 99);
            panelTVertical.TabIndex = 2;
            panelTVertical.Paint += panelTVertical_Paint;
            // 
            // panelLShape
            // 
            panelLShape.BorderStyle = BorderStyle.FixedSingle;
            panelLShape.Location = new Point(11, 382);
            panelLShape.Name = "panelLShape";
            panelLShape.Size = new Size(85, 99);
            panelLShape.TabIndex = 2;
            panelLShape.Paint += panelLShape_Paint;
            // 
            // FormRoad02
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(169, 800);
            Controls.Add(panelLShape);
            Controls.Add(panelTVertical);
            Controls.Add(panelTHorizontal);
            Controls.Add(panelCross);
            Controls.Add(btnRoadIntersection);
            Name = "FormRoad02";
            Text = "FormRoad02";
            ResumeLayout(false);
        }

        #endregion

        private Button btnRoadIntersection;
        private Panel panelCross;
        private Panel panelTHorizontal;
        private Panel panelTVertical;
        private Panel panelLShape;
    }
}