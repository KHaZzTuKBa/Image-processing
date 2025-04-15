namespace lab1.Lr5
{
    partial class ChFilteringForm
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
            this.MenuButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MenuButton
            // 
            this.MenuButton.Location = new System.Drawing.Point(12, 689);
            this.MenuButton.Name = "MenuButton";
            this.MenuButton.Size = new System.Drawing.Size(75, 23);
            this.MenuButton.TabIndex = 1;
            this.MenuButton.Text = "В меню";
            this.MenuButton.UseVisualStyleBackColor = true;
            // 
            // ChFilteringForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 711);
            this.Controls.Add(this.MenuButton);
            this.Name = "ChFilteringForm";
            this.Text = "ChFilteringForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button MenuButton;
    }
}