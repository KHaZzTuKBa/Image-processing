using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1.Lr4
{
    public partial class PrFilteringForm : Form
    {
        private Button btnDeleteImage;
        private Button btnBinarize;
        private Image originalImage;
        private Label labelDragDrop;
        private Panel manualPanel;
        private Panel gaussianPanel;
        private TableLayoutPanel tableLayoutPanelMatrix;

        public PrFilteringForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            InitManualPanel();
            InitGaussianPanel();
            UpdatePanelsVisibility();
        }

        private bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
        }

        private void BinForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (pictureBox1.Image == null)
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0 && IsImageFile(files[0]))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void BinForm_DragDrop(object sender, DragEventArgs e)
        {
            if (originalImage == null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsImageFile(files[0]))
                {
                    try
                    {
                        Image img;
                        using (var fs = new FileStream(files[0], FileMode.Open, FileAccess.Read))
                        {
                            Image tmp = Image.FromStream(fs);
                            img = new Bitmap(tmp);
                        }
                        // Масштабирование при загрузке больших изображений
                        const int maxDimension = 512;
                        if (img.Width > maxDimension || img.Height > maxDimension)
                        {
                            double scale = Math.Min((double)maxDimension / img.Width, (double)maxDimension / img.Height);
                            int newWidth = (int)(img.Width * scale);
                            int newHeight = (int)(img.Height * scale);
                            Bitmap resized = new Bitmap(newWidth, newHeight);
                            using (Graphics g = Graphics.FromImage(resized))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.DrawImage(img, 0, 0, newWidth, newHeight);
                            }
                            img.Dispose();
                            img = resized;
                        }
                        originalImage = img;
                        pictureBox1.Image = img;
                        labelDragDrop.Visible = false;
                        btnDeleteImage.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                    }
                }
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuForm menuForm = new MenuForm();
            menuForm.Show();

            this.Close();
        }

        private void InitGaussianPanel()
        {
            gaussianPanel = new Panel();
            gaussianPanel.Name = "gaussianPanel";
            gaussianPanel.Location = new Point(comboBox1.Left, comboBox1.Bottom + 10);
            gaussianPanel.Size = new Size(300, 100);

            Label labelRadius = new Label();
            labelRadius.Text = "Радиус:";
            labelRadius.Location = new Point(0, 0);
            gaussianPanel.Controls.Add(labelRadius);

            TextBox textBoxRadius = new TextBox();
            textBoxRadius.Name = "textBoxRadius";
            textBoxRadius.Location = new Point(120, 0);
            gaussianPanel.Controls.Add(textBoxRadius);

            Label labelSigma = new Label();
            labelSigma.Text = "Сигма:";
            labelSigma.Location = new Point(0, 30);
            gaussianPanel.Controls.Add(labelSigma);

            TextBox textBoxSigma = new TextBox();
            textBoxSigma.Name = "textBoxSigma";
            textBoxSigma.Location = new Point(120, 30);
            gaussianPanel.Controls.Add(textBoxSigma);

            Button buttonTransformGaussian = new Button();
            buttonTransformGaussian.Text = "Преобразовать";
            buttonTransformGaussian.Location = new Point(0, 60);
            gaussianPanel.Controls.Add(buttonTransformGaussian);

            gaussianPanel.Visible = false;

            this.Controls.Add(gaussianPanel);
        }

        private void InitManualPanel()
        {
            manualPanel = new Panel();
            manualPanel.Name = "manualPanel";
            manualPanel.Location = new Point(comboBox1.Left, comboBox1.Bottom + 10);
            manualPanel.Size = new Size(350, 300);

            Label labelHeight = new Label();
            labelHeight.Text = "Высота матрицы:";
            labelHeight.Location = new Point(0, 0);
            manualPanel.Controls.Add(labelHeight);

            TextBox textBoxMatrixHeight = new TextBox();
            textBoxMatrixHeight.Name = "textBoxMatrixHeight";
            textBoxMatrixHeight.Location = new Point(120, 0);
            textBoxMatrixHeight.Text = "3";
            textBoxMatrixHeight.Width = 50;
            manualPanel.Controls.Add(textBoxMatrixHeight);

            Label labelWidth = new Label();
            labelWidth.Text = "Ширина матрицы:";
            labelWidth.Location = new Point(0, 30);
            manualPanel.Controls.Add(labelWidth);

            TextBox textBoxMatrixWidth = new TextBox();
            textBoxMatrixWidth.Name = "textBoxMatrixWidth";
            textBoxMatrixWidth.Location = new Point(120, 30);
            textBoxMatrixWidth.Text = "3";
            textBoxMatrixWidth.Width = 50;
            manualPanel.Controls.Add(textBoxMatrixWidth);

            textBoxMatrixHeight.TextChanged += (s, e) => {
                int h, w;
                if(int.TryParse(textBoxMatrixHeight.Text, out h) && int.TryParse(textBoxMatrixWidth.Text, out w) && h > 0 && w > 0)
                {
                    UpdateMatrixGrid(h, w);
                }
            };

            textBoxMatrixWidth.TextChanged += (s, e) => {
                int h, w;
                if(int.TryParse(textBoxMatrixHeight.Text, out h) && int.TryParse(textBoxMatrixWidth.Text, out w) && h > 0 && w > 0)
                {
                    UpdateMatrixGrid(h, w);
                }
            };

            UpdateMatrixGrid(3, 3);

            Button buttonTransformManual = new Button();
            buttonTransformManual.Text = "Преобразовать";
            buttonTransformManual.Location = new Point(0, 260);
            manualPanel.Controls.Add(buttonTransformManual);

            manualPanel.Visible = false;
            this.Controls.Add(manualPanel);
        }

        private void UpdateMatrixGrid(int rows, int cols)
        {
            if(tableLayoutPanelMatrix != null)
            {
                manualPanel.Controls.Remove(tableLayoutPanelMatrix);
            }
            tableLayoutPanelMatrix = new TableLayoutPanel();
            tableLayoutPanelMatrix.AutoSize = true;
            tableLayoutPanelMatrix.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelMatrix.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanelMatrix.Location = new Point(0, 60);
            tableLayoutPanelMatrix.RowCount = rows;
            tableLayoutPanelMatrix.ColumnCount = cols;

            tableLayoutPanelMatrix.ColumnStyles.Clear();
            for (int c = 0; c < cols; c++)
            {
                 tableLayoutPanelMatrix.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
            }
            tableLayoutPanelMatrix.RowStyles.Clear();
            for (int r = 0; r < rows; r++)
            {
                 tableLayoutPanelMatrix.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            tableLayoutPanelMatrix.Controls.Clear();
            for (int r = 0; r < rows; r++)
            {
                 for (int c = 0; c < cols; c++)
                 {
                     TextBox cellBox = new TextBox();
                     cellBox.Dock = DockStyle.Fill;
                     cellBox.Text = "";
                     tableLayoutPanelMatrix.Controls.Add(cellBox, c, r);
                 }
            }
            manualPanel.Controls.Add(tableLayoutPanelMatrix);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePanelsVisibility();
        }

        private void UpdatePanelsVisibility()
        {
            string selected = comboBox1.SelectedItem.ToString();
            if(selected == "Линейная вручную" || selected == "Нелинейная вручную")
            {
                manualPanel.Visible = true;
                gaussianPanel.Visible = false;
            }
            else if(selected == "Линейная Гаусс" || selected == "Нелинейная Гаусс")
            {
                manualPanel.Visible = false;
                gaussianPanel.Visible = true;
            }
        }
    }
}
