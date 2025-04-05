using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

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

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(BinForm_DragEnter);
            this.DragDrop += new DragEventHandler(BinForm_DragDrop);

            labelDragDrop = new Label();
            labelDragDrop.Text = "Перетащите изображение на форму";
            labelDragDrop.AutoSize = true;
            labelDragDrop.Location = new Point(pictureBox1.Left + 10, pictureBox1.Top + 10);
            this.Controls.Add(labelDragDrop);
            labelDragDrop.BringToFront();

            btnDeleteImage = new Button();
            btnDeleteImage.Text = "Удалить изображение";
            btnDeleteImage.Size = new Size(120, 30);
            btnDeleteImage.Location = new Point(comboBox1.Left, pictureBox1.Bottom - 30 - btnDeleteImage.Height);
            btnDeleteImage.Click += BtnDeleteImage_Click;
            btnDeleteImage.Enabled = false;
            btnDeleteImage.FlatStyle = FlatStyle.Flat;
            btnDeleteImage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDeleteImage.BackColor = Color.FromArgb(240, 128, 128);
            btnDeleteImage.ForeColor = Color.White;
            btnDeleteImage.FlatAppearance.BorderSize = 0;
            btnDeleteImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 70, 70);
            btnDeleteImage.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 60, 60);
            btnDeleteImage.EnabledChanged += (s, e) =>
            {
                btnDeleteImage.BackColor = btnDeleteImage.Enabled
                    ? Color.FromArgb(220, 80, 80)
                    : Color.FromArgb(240, 128, 128);
            };
            this.Controls.Add(btnDeleteImage);


            // Стилизация основной формы
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 9.75F);

            // Стилизация комбо-бокса
            comboBox1.Font = new Font("Segoe UI", 10F);
            comboBox1.FlatStyle = FlatStyle.Flat;

            // Стилизация labelDragDrop
            labelDragDrop.Font = new Font("Segoe UI", 11F, FontStyle.Italic);
            labelDragDrop.ForeColor = Color.FromArgb(150, 150, 150);

            MenuButton.BackColor = Color.FromArgb(173, 216, 230);
            MenuButton.Size = new Size(100, 35);

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

        private void BtnDeleteImage_Click(object sender, EventArgs e)
        {
            if (originalImage != null)
            {
                originalImage.Dispose();
                originalImage = null;
            }
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            labelDragDrop.Visible = true;
            btnDeleteImage.Enabled = false;
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
            buttonTransformGaussian.Location = new Point(0, 65);
            buttonTransformGaussian.Width = 150;
            buttonTransformGaussian.Height = 30;
            buttonTransformGaussian.BackColor = Color.FromArgb(173, 216, 230);
            buttonTransformGaussian.ForeColor = Color.White;
            gaussianPanel.Controls.Add(buttonTransformGaussian);
            buttonTransformGaussian.Click += (sender, e) => { ApplyTransformation(); };

            gaussianPanel.Visible = false;

            foreach (Control c in gaussianPanel.Controls)
            {
                if (c is TextBox tb)
                {
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    tb.Font = new Font("Segoe UI", 9F);
                    tb.BackColor = Color.FromArgb(245, 245, 245);
                }
            }

            this.Controls.Add(gaussianPanel);
        }

        private void ApplyTransformation()
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Линейная вручную":
                    ApplyLinearManualFilter();
                    break;
                case "Нелинейная вручную":
                    ApplyNonlinearManualFilter();
                    break;
                case "Линейная Гаусс":
                    ApplyGaussianFilter();
                    break;
                case "Нелинейная Гаусс":
                    ApplyNonlinearGaussianFilter();
                    break;
                default:
                    break;
            }
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

            textBoxMatrixHeight.TextChanged += (s, e) =>
            {
                int h, w;
                if (int.TryParse(textBoxMatrixHeight.Text, out h) && int.TryParse(textBoxMatrixWidth.Text, out w) && h > 0 && w > 0)
                {
                    UpdateMatrixGrid(h, w);
                }
            };

            textBoxMatrixWidth.TextChanged += (s, e) =>
            {
                int h, w;
                if (int.TryParse(textBoxMatrixHeight.Text, out h) && int.TryParse(textBoxMatrixWidth.Text, out w) && h > 0 && w > 0)
                {
                    UpdateMatrixGrid(h, w);
                }
            };     

            UpdateMatrixGrid(3, 3);

            Button buttonTransformManual = new Button();
            buttonTransformManual.Text = "Преобразовать";
            buttonTransformManual.Location = new Point(0, 260);
            buttonTransformManual.Height = 30;
            buttonTransformManual.Width = 150;
            buttonTransformManual.BackColor = Color.FromArgb(173, 216, 230);
            buttonTransformManual.ForeColor = Color.White;
            buttonTransformManual.Click += (sender, e) => { ApplyTransformation(); };
            manualPanel.Controls.Add(buttonTransformManual);

            manualPanel.Visible = false;
            this.Controls.Add(manualPanel);
        }

        private void UpdateMatrixGrid(int rows, int cols)
        {
            if (tableLayoutPanelMatrix != null)
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

            foreach (Control c in tableLayoutPanelMatrix.Controls)
            {
                if (c is TextBox tb)
                {
                    tb.BorderStyle = BorderStyle.None;
                    tb.BackColor = Color.White;
                    tb.Font = new Font("Segoe UI", 10F);
                    tb.Margin = new Padding(1);
                    tb.Enter += (s, e) => tb.BackColor = Color.FromArgb(240, 240, 240);
                    tb.Leave += (s, e) => tb.BackColor = Color.White;
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
            if (selected == "Линейная вручную" || selected == "Нелинейная вручную")
            {
                manualPanel.Visible = true;
                gaussianPanel.Visible = false;
            }
            else if (selected == "Линейная Гаусс" || selected == "Нелинейная Гаусс")
            {
                manualPanel.Visible = false;
                gaussianPanel.Visible = true;
            }
        }

        private void CheckManualFilter()
        {
            if (tableLayoutPanelMatrix.RowCount % 2 == 0 || tableLayoutPanelMatrix.ColumnCount % 2 == 0)
            {
                MessageBox.Show("Размеры матрицы должны быть нечетными!");
                return;
            }
            if (tableLayoutPanelMatrix.RowCount < 3 || tableLayoutPanelMatrix.ColumnCount < 3)
            {
                MessageBox.Show("Размеры матрицы должны быть не меньше 3x3!");
                return;
            }
        }

        private Bitmap ConvolveImage(Bitmap src, double[,] kernel)
        {
            int width = src.Width;
            int height = src.Height;
            int kernelRows = kernel.GetLength(0);
            int kernelCols = kernel.GetLength(1);
            int anchorX = kernelCols / 2;
            int anchorY = kernelRows / 2;

            Bitmap dest = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Bitmap bmp24 = src.PixelFormat == PixelFormat.Format24bppRgb ? src : new Bitmap(width, height, PixelFormat.Format24bppRgb);
            if(bmp24 != src)
            {
                using(Graphics g = Graphics.FromImage(bmp24))
                {
                    g.DrawImage(src, new Rectangle(0,0,width,height));
                }
            }
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData srcData = bmp24.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData destData = dest.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int stride = srcData.Stride;
            int bytes = stride * height;
            byte[] srcBuffer = new byte[bytes];
            byte[] destBuffer = new byte[bytes];
            Marshal.Copy(srcData.Scan0, srcBuffer, 0, bytes);
            bmp24.UnlockBits(srcData);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sumR = 0, sumG = 0, sumB = 0;
                    for (int ky = 0; ky < kernelRows; ky++)
                    {
                        for (int kx = 0; kx < kernelCols; kx++)
                        {
                            int pixelX = Math.Min(width - 1, Math.Max(0, x + kx - anchorX));
                            int pixelY = Math.Min(height - 1, Math.Max(0, y + ky - anchorY));
                            int index = pixelY * stride + pixelX * 3;
                            sumB += kernel[ky, kx] * srcBuffer[index + 0];
                            sumG += kernel[ky, kx] * srcBuffer[index + 1];
                            sumR += kernel[ky, kx] * srcBuffer[index + 2];
                        }
                    }
                    int destIndex = y * stride + x * 3;
                    destBuffer[destIndex + 0] = (byte)Math.Min(255, Math.Max(0, (int)sumB));
                    destBuffer[destIndex + 1] = (byte)Math.Min(255, Math.Max(0, (int)sumG));
                    destBuffer[destIndex + 2] = (byte)Math.Min(255, Math.Max(0, (int)sumR));
                }
            }
            Marshal.Copy(destBuffer, 0, destData.Scan0, bytes);
            dest.UnlockBits(destData);
            return dest;
        }

        private Bitmap MedianFilter(Bitmap src, int rows, int cols)
        {
            int width = src.Width;
            int height = src.Height;
            Bitmap dest = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Bitmap bmp24 = src.PixelFormat == PixelFormat.Format24bppRgb ? src : new Bitmap(width, height, PixelFormat.Format24bppRgb);
            if(bmp24 != src)
            {
                using(Graphics g = Graphics.FromImage(bmp24))
                {
                    g.DrawImage(src, new Rectangle(0,0,width,height));
                }
            }
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData srcData = bmp24.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData destData = dest.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int stride = srcData.Stride;
            int bytes = stride * height;
            byte[] srcBuffer = new byte[bytes];
            byte[] destBuffer = new byte[bytes];
            Marshal.Copy(srcData.Scan0, srcBuffer, 0, bytes);
            bmp24.UnlockBits(srcData);

            int anchorX = cols / 2;
            int anchorY = rows / 2;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    List<byte> listR = new List<byte>();
                    List<byte> listG = new List<byte>();
                    List<byte> listB = new List<byte>();
                    for (int ky = 0; ky < rows; ky++)
                    {
                        for (int kx = 0; kx < cols; kx++)
                        {
                            int pixelX = Math.Min(width - 1, Math.Max(0, x + kx - anchorX));
                            int pixelY = Math.Min(height - 1, Math.Max(0, y + ky - anchorY));
                            int index = pixelY * stride + pixelX * 3;
                            listB.Add(srcBuffer[index + 0]);
                            listG.Add(srcBuffer[index + 1]);
                            listR.Add(srcBuffer[index + 2]);
                        }
                    }
                    listB.Sort();
                    listG.Sort();
                    listR.Sort();
                    byte medianB = listB[listB.Count / 2];
                    byte medianG = listG[listG.Count / 2];
                    byte medianR = listR[listR.Count / 2];
                    int destIndex = y * stride + x * 3;
                    destBuffer[destIndex + 0] = medianB;
                    destBuffer[destIndex + 1] = medianG;
                    destBuffer[destIndex + 2] = medianR;
                }
            }
            Marshal.Copy(destBuffer, 0, destData.Scan0, bytes);
            dest.UnlockBits(destData);
            return dest;
        }

        private void ApplyLinearManualFilter()
        {
            if (originalImage == null)
            {
                MessageBox.Show("Нет исходного изображения!");
                return;
            }

            CheckManualFilter();

            int rows = tableLayoutPanelMatrix.RowCount;
            int cols = tableLayoutPanelMatrix.ColumnCount;
            double[,] kernel = new double[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Control ctrl = tableLayoutPanelMatrix.GetControlFromPosition(c, r);
                    if (ctrl is TextBox textBox)
                    {
                        if (string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            MessageBox.Show("Все значения матрицы должны быть заполнены!");
                            return;
                        }
                        double value = 0.0;
                        if (!double.TryParse(textBox.Text, out value))
                        {
                            MessageBox.Show("Некорректное значение матрицы!");
                            return;
                        }
                        kernel[r, c] = value;
                    }
                }
            }
            Bitmap src = new Bitmap(originalImage);
            Bitmap dest = ConvolveImage(src, kernel);
            pictureBox1.Image = dest;
        }

        private void ApplyGaussianFilter()
        {
            TextBox textBoxRadius = gaussianPanel.Controls.Find("textBoxRadius", false).FirstOrDefault() as TextBox;
            TextBox textBoxSigma = gaussianPanel.Controls.Find("textBoxSigma", false).FirstOrDefault() as TextBox;
            if (textBoxRadius == null || textBoxSigma == null)
            {
                MessageBox.Show("Не удалось найти элементы ввода радиуса или сигмы.");
                return;
            }

            int radius;
            if (!int.TryParse(textBoxRadius.Text, out radius))
            {
                MessageBox.Show("Некорректное значение радиуса!");
                return;
            }
            if (radius <= 0)
            {
                MessageBox.Show("Радиус должен быть больше 0!");
                return;
            }

            double sigma;
            if (!double.TryParse(textBoxSigma.Text, out sigma))
            {
                MessageBox.Show("Некорректное значение сигмы!");
                return;
            }
            if (sigma <= 0)
            {
                MessageBox.Show("Сигма должна быть больше 0!");
                return;
            }

            int size = 2 * radius + 1;
            double[,] kernel = new double[size, size];
            double total = 0.0;
            double sig_sqr = 2.0 * sigma * sigma;
            double pi_sig_sqr = sig_sqr * Math.PI;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    double g = 1.0 / pi_sig_sqr * Math.Exp(-(i * i + j * j) / sig_sqr);
                    kernel[i + radius, j + radius] = g;
                    total += g;
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= total;
                }
            }

            if (originalImage == null)
            {
                MessageBox.Show("Нет исходного изображения!");
                return;
            }
            Bitmap src = new Bitmap(originalImage);
            Bitmap dest = ConvolveImage(src, kernel);
            pictureBox1.Image = dest;
        }

        private void ApplyNonlinearManualFilter()
        {
            if (originalImage == null)
            {
                MessageBox.Show("Нет исходного изображения!");
                return;
            }

            CheckManualFilter();

            int rows = tableLayoutPanelMatrix.RowCount;
            int cols = tableLayoutPanelMatrix.ColumnCount;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Control ctrl = tableLayoutPanelMatrix.GetControlFromPosition(c, r);
                    if (ctrl is TextBox textBox)
                    {
                        if (string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            MessageBox.Show("Все значения матрицы должны быть заполнены!");
                            return;
                        }
                        double dummy;
                        if (!double.TryParse(textBox.Text, out dummy))
                        {
                            MessageBox.Show("Некорректное значение матрицы!");
                            return;
                        }
                    }
                }
            }
            Bitmap src = new Bitmap(originalImage);
            Bitmap dest = MedianFilter(src, rows, cols);
            pictureBox1.Image = dest;
        }

        private void ApplyNonlinearGaussianFilter()
        {
            TextBox textBoxRadius = gaussianPanel.Controls.Find("textBoxRadius", false).FirstOrDefault() as TextBox;
            TextBox textBoxSigma = gaussianPanel.Controls.Find("textBoxSigma", false).FirstOrDefault() as TextBox;
            if (textBoxRadius == null || textBoxSigma == null)
            {
                MessageBox.Show("Не удалось найти элементы ввода радиуса или сигмы.");
                return;
            }

            int radius;
            if (!int.TryParse(textBoxRadius.Text, out radius))
            {
                MessageBox.Show("Некорректное значение радиуса!");
                return;
            }
            if (radius <= 0)
            {
                MessageBox.Show("Радиус должен быть больше 0!");
                return;
            }

            double sigma;
            if (!double.TryParse(textBoxSigma.Text, out sigma))
            {
                MessageBox.Show("Некорректное значение сигмы!");
                return;
            }
            if (sigma <= 0)
            {
                MessageBox.Show("Сигма должна быть больше 0!");
                return;
            }

            int size = 2 * radius + 1;
            double[,] kernel = new double[size, size];
            double total = 0.0;
            double sig_sqr = 2.0 * sigma * sigma;
            double pi_sig_sqr = sig_sqr * Math.PI;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    double g = 1.0 / pi_sig_sqr * Math.Exp(-(i * i + j * j) / sig_sqr);
                    kernel[i + radius, j + radius] = g;
                    total += g;
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= total;
                }
            }

            if (originalImage == null)
            {
                MessageBox.Show("Нет исходного изображения!");
                return;
            }
            Bitmap src = new Bitmap(originalImage);
            Bitmap dest = ConvolveImage(src, kernel);
            pictureBox1.Image = dest;
        }
    }
}