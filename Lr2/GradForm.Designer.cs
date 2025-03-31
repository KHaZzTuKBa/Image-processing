namespace lab1.Lr2
{
    partial class GradForm
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
            this.btnReset = new System.Windows.Forms.Button();
            this.comboInterpolation = new System.Windows.Forms.ComboBox();
            this.panelCurve = new lab1.Lr2.CurveControl();
            this.pictureBoxResult = new System.Windows.Forms.PictureBox();
            this.pictureBoxHistogram = new System.Windows.Forms.PictureBox();
            this.MenuButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(32, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Сброс";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // comboInterpolation
            // 
            this.comboInterpolation.FormattingEnabled = true;
            this.comboInterpolation.Items.AddRange(new object[] {
            "Линейная интерполяция",
            "Кубическая интерполяция"});
            this.comboInterpolation.Location = new System.Drawing.Point(134, 4);
            this.comboInterpolation.Name = "comboInterpolation";
            this.comboInterpolation.Size = new System.Drawing.Size(121, 21);
            this.comboInterpolation.TabIndex = 4;
            this.comboInterpolation.SelectedIndexChanged += new System.EventHandler(this.comboInterpolation_SelectedIndexChanged);
            // 
            // panelCurve
            // 
            this.panelCurve.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCurve.Interpolator = null;
            this.panelCurve.Location = new System.Drawing.Point(32, 38);
            this.panelCurve.Name = "panelCurve";
            this.panelCurve.Size = new System.Drawing.Size(512, 512);
            this.panelCurve.TabIndex = 5;
            // 
            // pictureBoxResult
            // 
            this.pictureBoxResult.Location = new System.Drawing.Point(587, 38);
            this.pictureBoxResult.Name = "pictureBoxResult";
            this.pictureBoxResult.Size = new System.Drawing.Size(512, 512);
            this.pictureBoxResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxResult.TabIndex = 6;
            this.pictureBoxResult.TabStop = false;
            // 
            // pictureBoxHistogram
            // 
            this.pictureBoxHistogram.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureBoxHistogram.Location = new System.Drawing.Point(0, 556);
            this.pictureBoxHistogram.Name = "pictureBoxHistogram";
            this.pictureBoxHistogram.Size = new System.Drawing.Size(1127, 150);
            this.pictureBoxHistogram.TabIndex = 7;
            this.pictureBoxHistogram.TabStop = false;
            // 
            // MenuButton
            // 
            this.MenuButton.Location = new System.Drawing.Point(1040, 9);
            this.MenuButton.Name = "MenuButton";
            this.MenuButton.Size = new System.Drawing.Size(75, 23);
            this.MenuButton.TabIndex = 8;
            this.MenuButton.Text = "В меню";
            this.MenuButton.UseVisualStyleBackColor = true;
            this.MenuButton.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // GradForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 706);
            this.Controls.Add(this.MenuButton);
            this.Controls.Add(this.pictureBoxHistogram);
            this.Controls.Add(this.pictureBoxResult);
            this.Controls.Add(this.panelCurve);
            this.Controls.Add(this.comboInterpolation);
            this.Controls.Add(this.btnReset);
            this.Name = "GradForm";
            this.Text = "GradForm";
            this.Load += new System.EventHandler(this.GradForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistogram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ComboBox comboInterpolation;
        private lab1.Lr2.CurveControl panelCurve;
        private System.Windows.Forms.PictureBox pictureBoxResult;
        private System.Windows.Forms.PictureBox pictureBoxHistogram;
        private System.Windows.Forms.Button MenuButton;
    }
}