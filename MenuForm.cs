using lab1.Lr2;
using lab1.Lr3;
using lab1.Lr4;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void LR1Button_Click(object sender, EventArgs e)
        {
            Form1 Lr1 = new Form1();
            Lr1.Show();

            this.Hide();
        }

        private void LR2Button_Click(object sender, EventArgs e)
        {
            GradForm gradForm = new GradForm();
            gradForm.Show();

            this.Hide();
        }

        private void LR3Button_Click(object sender, EventArgs e)
        {
            BinForm binForm = new BinForm();
            binForm.Show();

            this.Hide();
        }

        private void LR4Button_Click(object sender, EventArgs e)
        {
            PrFilteringForm prFilteringForm = new PrFilteringForm();
            prFilteringForm.Show();

            this.Hide();
        }

        private void LR5Button_Click(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            using (Pen borderPen = new Pen(Color.FromArgb(224, 224, 224), 1))
            {
                e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, Width - 1, Height - 1));
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = Color.FromArgb(96, 96, 96);
            btn.FlatAppearance.BorderColor = Color.FromArgb(160, 160, 160);
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = Color.FromArgb(64, 64, 64);
            btn.FlatAppearance.BorderColor = Color.Silver;
        }
    }
}
