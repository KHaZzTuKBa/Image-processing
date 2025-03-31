namespace lab1
{
    partial class MenuForm
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
            this.LR1Button = new System.Windows.Forms.Button();
            this.LR2Button = new System.Windows.Forms.Button();
            this.LR3Button = new System.Windows.Forms.Button();
            this.LR4Button = new System.Windows.Forms.Button();
            this.LR5Button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LR1Button
            // 
            this.LR1Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LR1Button.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.LR1Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LR1Button.Font = new System.Drawing.Font("Segoe UI Semibold", 8F, System.Drawing.FontStyle.Bold);
            this.LR1Button.ForeColor = System.Drawing.Color.White;
            this.LR1Button.Location = new System.Drawing.Point(40, 80);
            this.LR1Button.Margin = new System.Windows.Forms.Padding(10);
            this.LR1Button.Name = "LR1Button";
            this.LR1Button.Size = new System.Drawing.Size(200, 40);
            this.LR1Button.TabIndex = 0;
            this.LR1Button.Text = "ЛР1. Работа с изображениями";
            this.LR1Button.UseVisualStyleBackColor = false;
            this.LR1Button.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.LR1Button.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.LR1Button.Click += new System.EventHandler(this.LR1Button_Click);
            // 
            // LR2Button
            // 
            this.LR2Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LR2Button.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.LR2Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LR2Button.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.LR2Button.ForeColor = System.Drawing.Color.White;
            this.LR2Button.Location = new System.Drawing.Point(40, 140);
            this.LR2Button.Margin = new System.Windows.Forms.Padding(10);
            this.LR2Button.Name = "LR2Button";
            this.LR2Button.Size = new System.Drawing.Size(200, 40);
            this.LR2Button.TabIndex = 1;
            this.LR2Button.Text = "ЛР2. Градационные преобразования";
            this.LR2Button.UseVisualStyleBackColor = false;
            this.LR2Button.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.LR2Button.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.LR2Button.Click += new System.EventHandler(this.LR2Button_Click);
            // 
            // LR3Button
            // 
            this.LR3Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LR3Button.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.LR3Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LR3Button.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.LR3Button.ForeColor = System.Drawing.Color.White;
            this.LR3Button.Location = new System.Drawing.Point(40, 200);
            this.LR3Button.Margin = new System.Windows.Forms.Padding(10);
            this.LR3Button.Name = "LR3Button";
            this.LR3Button.Size = new System.Drawing.Size(200, 40);
            this.LR3Button.TabIndex = 2;
            this.LR3Button.Text = "ЛР3. Бинаризация изображений";
            this.LR3Button.UseVisualStyleBackColor = false;
            this.LR3Button.Click += new System.EventHandler(this.LR3Button_Click);
            this.LR3Button.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.LR3Button.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // LR4Button
            // 
            this.LR4Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LR4Button.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.LR4Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LR4Button.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.LR4Button.ForeColor = System.Drawing.Color.White;
            this.LR4Button.Location = new System.Drawing.Point(40, 260);
            this.LR4Button.Margin = new System.Windows.Forms.Padding(10);
            this.LR4Button.Name = "LR4Button";
            this.LR4Button.Size = new System.Drawing.Size(200, 40);
            this.LR4Button.TabIndex = 3;
            this.LR4Button.Text = "ЛР4. Пространственная фильтрация";
            this.LR4Button.UseVisualStyleBackColor = false;
            this.LR4Button.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.LR4Button.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.LR4Button.Click += new System.EventHandler(this.LR4Button_Click);
            // 
            // LR5Button
            // 
            this.LR5Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LR5Button.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.LR5Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LR5Button.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.LR5Button.ForeColor = System.Drawing.Color.White;
            this.LR5Button.Location = new System.Drawing.Point(40, 320);
            this.LR5Button.Margin = new System.Windows.Forms.Padding(10);
            this.LR5Button.Name = "LR5Button";
            this.LR5Button.Size = new System.Drawing.Size(200, 40);
            this.LR5Button.TabIndex = 4;
            this.LR5Button.Text = "ЛР5. Частотная фильтрация";
            this.LR5Button.UseVisualStyleBackColor = false;
            this.LR5Button.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.LR5Button.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this.LR5Button.Click += new System.EventHandler(this.LR5Button_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(40, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 40);
            this.label1.TabIndex = 5;
            this.label1.Text = "Меню ЛР";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 411);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LR5Button);
            this.Controls.Add(this.LR4Button);
            this.Controls.Add(this.LR3Button);
            this.Controls.Add(this.LR2Button);
            this.Controls.Add(this.LR1Button);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Лабораторные работы";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LR1Button;
        private System.Windows.Forms.Button LR2Button;
        private System.Windows.Forms.Button LR3Button;
        private System.Windows.Forms.Button LR4Button;
        private System.Windows.Forms.Button LR5Button;
        private System.Windows.Forms.Label label1;
    }
}