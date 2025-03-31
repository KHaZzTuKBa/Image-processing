using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace lab1.Lr3
{
    public partial class BinForm : Form
    {
        // Поле для панели с дополнительными параметрами
        private Panel panelOptions;
        private Label labelDragDrop;
        private Button btnDeleteImage;
        private Button btnBinarize;
        private Image originalImage;

        public BinForm()
        {
            InitializeComponent();

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(BinForm_DragEnter);
            this.DragDrop += new DragEventHandler(BinForm_DragDrop);

            labelDragDrop = new Label();
            labelDragDrop.Text = "Перетащите изображение на форму";
            //labelDragDrop.ForeColor = Color.White;
            labelDragDrop.AutoSize = true;
            labelDragDrop.Location = new Point(pictureBox1.Left + 10, pictureBox1.Top + 10);
            this.Controls.Add(labelDragDrop);
            labelDragDrop.BringToFront();

            btnDeleteImage = new Button();
            btnDeleteImage.Text = "Удалить изображение";
            btnDeleteImage.Size = new Size(120, 23);
            btnDeleteImage.Location = new Point(comboBox1.Left, pictureBox1.Bottom - 23);
            btnDeleteImage.Click += BtnDeleteImage_Click;
            btnDeleteImage.Enabled = false;
            this.Controls.Add(btnDeleteImage);

            btnBinarize = new Button();
            btnBinarize.Text = "Бинаризировать";
            btnBinarize.Size = new Size(120, 23);
            btnBinarize.Location = new Point(comboBox1.Left, comboBox1.Bottom + 10);
            btnBinarize.Click += BtnBinarize_Click;
            this.Controls.Add(btnBinarize);

            comboBox1.SelectedIndex = 0;
            InitializeOptionsPanel();

            this.DoubleBuffered = true;
            InitializeModernControls();
        }

        private void InitializeModernControls()
        {
            // Стилизация labelDragDrop
            labelDragDrop.Font = new Font("Segoe UI", 9.75F, FontStyle.Italic);
            labelDragDrop.ForeColor = Color.FromArgb(150, 150, 150);
            labelDragDrop.BackColor = Color.White;

            // Стилизация кнопок
            btnDeleteImage.FlatStyle = FlatStyle.Flat;
            btnDeleteImage.BackColor = Color.FromArgb(64, 64, 64);
            btnDeleteImage.ForeColor = Color.White;
            btnDeleteImage.FlatAppearance.BorderSize = 0;
            btnDeleteImage.Font = new Font("Segoe UI", 9.75F);

            btnBinarize.FlatStyle = FlatStyle.Flat;
            btnBinarize.BackColor = Color.FromArgb(64, 64, 64);
            btnBinarize.ForeColor = Color.White;
            btnBinarize.FlatAppearance.BorderSize = 0;
            btnBinarize.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnBinarize.Size = new Size(140, 35);

            // Стилизация панели параметров
            panelOptions.BackColor = Color.FromArgb(64, 64, 64);
            panelOptions.ForeColor = Color.White;
            panelOptions.Font = new Font("Segoe UI", 9F);
            panelOptions.Padding = new Padding(5);
        }

        // Инициализация панели с дополнительными параметрами
        private void InitializeOptionsPanel()
        {
            panelOptions = new Panel();
            panelOptions.Location = new Point(comboBox1.Location.X, comboBox1.Bottom + 10);
            panelOptions.Size = new Size(250, 80);
            panelOptions.BorderStyle = BorderStyle.FixedSingle;

            panelOptions.Visible = false;
            this.Controls.Add(panelOptions);
            
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            ComboBox1_SelectedIndexChanged(comboBox1, EventArgs.Empty);
        }

        // Обработчик изменения выбора в comboBox1
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMethod = comboBox1.SelectedItem.ToString();
            panelOptions.Controls.Clear();
            
            // Отображение дополнительной панели с необходимыми полями для некоторых методов
            if (selectedMethod == "Метод Ниблека")
            {
                panelOptions.Visible = true;

                Label lblWindowSize = new Label();
                lblWindowSize.Text = "Размер окна:";
                lblWindowSize.AutoSize = true;
                lblWindowSize.Location = new Point(10, 10);

                TextBox tbWindowSize = new TextBox();
                tbWindowSize.Location = new Point(lblWindowSize.Right + 10, 7);
                tbWindowSize.Width = 50;
                tbWindowSize.Text = "15";

                Label lblSensitivity = new Label();
                lblSensitivity.Text = "Чувствительность:";
                lblSensitivity.AutoSize = true;
                lblSensitivity.Location = new Point(10, lblWindowSize.Bottom + 10);

                TextBox tbSensitivity = new TextBox();
                tbSensitivity.Location = new Point(lblSensitivity.Right + 10, lblWindowSize.Bottom + 7);
                tbSensitivity.Width = 50;
                tbSensitivity.Text = "-0.2";

                panelOptions.Controls.Add(lblWindowSize);
                panelOptions.Controls.Add(tbWindowSize);
                panelOptions.Controls.Add(lblSensitivity);
                panelOptions.Controls.Add(tbSensitivity);
            }
            else if (selectedMethod == "Метод Сауволы")
            {
                panelOptions.Visible = true;

                Label lblWindowSize = new Label();
                lblWindowSize.Text = "Размер окна:";
                lblWindowSize.AutoSize = true;
                lblWindowSize.Location = new Point(10, 10);

                TextBox tbWindowSize = new TextBox();
                tbWindowSize.Location = new Point(lblWindowSize.Right + 10, 7);
                tbWindowSize.Width = 50;
                tbWindowSize.Text = "15";

                Label lblK = new Label();
                lblK.Text = "k:";
                lblK.AutoSize = true;
                lblK.Location = new Point(10, lblWindowSize.Bottom + 10);

                TextBox tbK = new TextBox();
                tbK.Location = new Point(lblK.Right + 10, lblWindowSize.Bottom + 7);
                tbK.Width = 50;
                tbK.Text = "0.2";

                panelOptions.Controls.Add(lblWindowSize);
                panelOptions.Controls.Add(tbWindowSize);
                panelOptions.Controls.Add(lblK);
                panelOptions.Controls.Add(tbK);
            }
            else if (selectedMethod == "Метод Вульфа")
            {
                panelOptions.Visible = true;

                Label lblWindowSize = new Label();
                lblWindowSize.Text = "Размер окна:";
                lblWindowSize.AutoSize = true;
                lblWindowSize.Location = new Point(10, 10);

                TextBox tbWindowSize = new TextBox();
                tbWindowSize.Location = new Point(lblWindowSize.Right + 10, 7);
                tbWindowSize.Width = 50;
                tbWindowSize.Text = "15";

                Label lblA = new Label();
                lblA.Text = "a:";
                lblA.AutoSize = true;
                lblA.Location = new Point(10, lblWindowSize.Bottom + 10);

                TextBox tbA = new TextBox();
                tbA.Location = new Point(lblA.Right + 10, lblWindowSize.Bottom + 7);
                tbA.Width = 50;
                tbA.Text = "0.5";

                panelOptions.Controls.Add(lblWindowSize);
                panelOptions.Controls.Add(tbWindowSize);
                panelOptions.Controls.Add(lblA);
                panelOptions.Controls.Add(tbA);
            }
            else if (selectedMethod == "Метод Бредли-Рота")
            {
                panelOptions.Visible = true;

                Label lblWindowSize = new Label();
                lblWindowSize.Text = "Размер окна:";
                lblWindowSize.AutoSize = true;
                lblWindowSize.Location = new Point(10, 10);

                TextBox tbWindowSize = new TextBox();
                tbWindowSize.Location = new Point(lblWindowSize.Right + 10, 7);
                tbWindowSize.Width = 50;
                tbWindowSize.Text = "15";

                Label lblT = new Label();
                lblT.Text = "t:";
                lblT.AutoSize = true;
                lblT.Location = new Point(10, lblWindowSize.Bottom + 10);

                TextBox tbT = new TextBox();
                tbT.Location = new Point(lblT.Right + 10, lblWindowSize.Bottom + 7);
                tbT.Width = 50;
                tbT.Text = "0.15";

                panelOptions.Controls.Add(lblWindowSize);
                panelOptions.Controls.Add(tbWindowSize);
                panelOptions.Controls.Add(lblT);
                panelOptions.Controls.Add(tbT);
            }
            else if (selectedMethod == "Метод Гаврилова")
            {
                panelOptions.Visible = false;
            }
            else if (selectedMethod == "Метод Отсу")
            {
                panelOptions.Visible = false;
            }
            else
            {
                panelOptions.Visible = false;
            }
            
            // Размещение панели
            if (panelOptions.Visible)
            {
                btnBinarize.Location = new Point(comboBox1.Left, panelOptions.Bottom + 10);
            }
            else
            {
                btnBinarize.Location = new Point(comboBox1.Left, comboBox1.Bottom + 10);
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuForm menuForm = new MenuForm();
            menuForm.Show();

            this.Close();
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

        private bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
        }

        // Удаление изображения
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

        // Конвертация изображения к градациям серого из методички
        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            int width = original.Width;
            int height = original.Height;
            Bitmap grayImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            var dataOrig = original.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var dataGray = grayImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int strideOrig = dataOrig.Stride;
            int strideGray = dataGray.Stride;
            unsafe
            {
                byte* ptrOrig = (byte*)dataOrig.Scan0.ToPointer();
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int indexOrig = y * strideOrig + x * 3;
                        byte B = ptrOrig[indexOrig];
                        byte G = ptrOrig[indexOrig + 1];
                        byte R = ptrOrig[indexOrig + 2];
                        int gray = (int)(R * 0.2125 + G * 0.7154 + B * 0.0721);
                        if (gray > 255) gray = 255;
                        if (gray < 0) gray = 0;
                        int indexGray = y * strideGray + x * 3;
                        ptrGray[indexGray] = (byte)gray;
                        ptrGray[indexGray + 1] = (byte)gray;
                        ptrGray[indexGray + 2] = (byte)gray;
                    }
                }
            }
            original.UnlockBits(dataOrig);
            grayImage.UnlockBits(dataGray);
            return grayImage;
        }

        // Реализация метода Гаврилова
        private void GavrMethod()
        {
            if (originalImage == null)
                return;

            Bitmap original = new Bitmap(originalImage);
            Bitmap grayImage = ConvertToGrayscale(original);
            original.Dispose();

            int width = grayImage.Width;
            int height = grayImage.Height;
            long total = 0;
            int count = 0;
            Rectangle rect = new Rectangle(0, 0, width, height);
            var dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int strideGray = dataGray.Stride;
            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * strideGray + x * 3;
                        byte pixelVal = ptrGray[index];
                        total += pixelVal;
                        count++;
                    }
                }
            }
            grayImage.UnlockBits(dataGray);

            double threshold = (double)total / count;
            Bitmap binaryImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Rectangle rectBinary = new Rectangle(0, 0, width, height);
            var dataBinary = binaryImage.LockBits(rectBinary, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            strideGray = dataGray.Stride;
            int strideBinary = dataBinary.Stride;

            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                byte* ptrBinary = (byte*)dataBinary.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int indexGray = y * strideGray + x * 3;
                        byte intensity = ptrGray[indexGray];
                        byte color = (intensity < threshold) ? (byte)0 : (byte)255;
                        int indexBinary = y * strideBinary + x * 3;
                        ptrBinary[indexBinary] = color;
                        ptrBinary[indexBinary + 1] = color;
                        ptrBinary[indexBinary + 2] = color;
                    }
                }
            }
            grayImage.UnlockBits(dataGray);
            binaryImage.UnlockBits(dataBinary);
            grayImage.Dispose();

            pictureBox1.Image = binaryImage;
            pictureBox1.Refresh();
        }

        // Реализация метода Отсу
        private void OtsuMethod()
        {
            if (originalImage == null)
                return;

            Bitmap original = new Bitmap(originalImage);
            Bitmap grayImage = ConvertToGrayscale(original);
            original.Dispose();

            int width = grayImage.Width;
            int height = grayImage.Height;
            int totalPixels = width * height;
            int[] histogram = new int[256];

            Rectangle rect = new Rectangle(0, 0, width, height);
            var dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int strideGray = dataGray.Stride;

            // Делаем гистограмму цветов
            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * strideGray + x * 3;
                        byte intensity = ptrGray[index];
                        histogram[intensity]++;
                    }
                }
            }
            grayImage.UnlockBits(dataGray);

            double sumAll = 0;
            for (int i = 0; i < 256; i++)
                sumAll += i * histogram[i];

            // Вычисление локального порога
            double sumB = 0;
            int wB = 0;
            double maxVariance = 0;
            int threshold = 0;
            for (int i = 0; i < 256; i++)
            {
                wB += histogram[i];
                if (wB == 0)
                    continue;
                int wF = totalPixels - wB;
                if (wF == 0)
                    break;
                sumB += i * histogram[i];
                double mB = sumB / wB;
                double mF = (sumAll - sumB) / wF;
                double varianceBetween = wB * wF * (mB - mF) * (mB - mF);
                if (varianceBetween > maxVariance)
                {
                    maxVariance = varianceBetween;
                    threshold = i;
                }
            }

            // Бинаризация
            Bitmap binaryImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Rectangle rectBinary = new Rectangle(0, 0, width, height);
            var dataBinary = binaryImage.LockBits(rectBinary, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            strideGray = dataGray.Stride;
            int strideBinary = dataBinary.Stride;

            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                byte* ptrBinary = (byte*)dataBinary.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int indexGray = y * strideGray + x * 3;
                        byte intensity = ptrGray[indexGray];
                        byte color = (intensity < threshold) ? (byte)0 : (byte)255;
                        int indexBinary = y * strideBinary + x * 3;
                        ptrBinary[indexBinary] = color;
                        ptrBinary[indexBinary + 1] = color;
                        ptrBinary[indexBinary + 2] = color;
                    }
                }
            }
            grayImage.UnlockBits(dataGray);
            binaryImage.UnlockBits(dataBinary);
            grayImage.Dispose();

            pictureBox1.Image = binaryImage;
            pictureBox1.Refresh();
        }

        // Реализация метода Ниблека
        private void NiblackMethod()
        {
            if (originalImage == null)
                return;

            Bitmap original = new Bitmap(originalImage);
            Bitmap grayImage = ConvertToGrayscale(original);
            original.Dispose();

            int width = grayImage.Width;
            int height = grayImage.Height;

            int windowSize = 15;
            double k = -0.2;

            // Попытка получить значения для размера окна и чувствительности из полей панели
            if (panelOptions != null && panelOptions.Controls.Count >= 4)
            {
                TextBox tbWindowSize = panelOptions.Controls[1] as TextBox;
                TextBox tbSensitivity = panelOptions.Controls[3] as TextBox;
                if (tbWindowSize != null)
                    int.TryParse(tbWindowSize.Text, out windowSize);
                if (tbSensitivity != null)
                    double.TryParse(tbSensitivity.Text, out k);
            }

            int halfWindow = windowSize / 2;
            Bitmap binaryImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataBinary = binaryImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int strideGray = dataGray.Stride;
            int strideBinary = dataBinary.Stride;

            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                byte* ptrBinary = (byte*)dataBinary.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int xMin = x - halfWindow;
                        int xMax = x + halfWindow;
                        int yMin = y - halfWindow;
                        int yMax = y + halfWindow;

                        if (xMin < 0) xMin = 0;
                        if (yMin < 0) yMin = 0;
                        if (xMax >= width) xMax = width - 1;
                        if (yMax >= height) yMax = height - 1;

                        int count = 0;
                        double sum = 0;
                        double sumSq = 0;

                        // Работа с окном соседних пикселей
                        for (int j = yMin; j <= yMax; j++)
                        {
                            for (int i = xMin; i <= xMax; i++)
                            {
                                int idx = j * strideGray + i * 3;
                                byte intensity = ptrGray[idx];
                                count++;
                                sum += intensity;
                                sumSq += intensity * intensity;
                            }
                        }

                        // Вычисление локального порога по формуле из методички
                        double mean = sum / count;
                        double variance = (sumSq / count) - (mean * mean);
                        double std = Math.Sqrt(variance);
                        double localThreshold = mean + k * std;

                        // Бинаризация
                        int index = y * strideGray + x * 3;
                        byte pixelIntensity = ptrGray[index];
                        byte color = (pixelIntensity <= localThreshold) ? (byte)0 : (byte)255;

                        int indexBinary = y * strideBinary + x * 3;
                        ptrBinary[indexBinary] = color;
                        ptrBinary[indexBinary + 1] = color;
                        ptrBinary[indexBinary + 2] = color;
                    }
                }
            }

            grayImage.UnlockBits(dataGray);
            binaryImage.UnlockBits(dataBinary);
            grayImage.Dispose();

            pictureBox1.Image = binaryImage;
            pictureBox1.Refresh();
        }

        // Реализация метода Сауволы
        private void SauvolaMethod()
        {
            if (originalImage == null)
                return;

            Bitmap original = new Bitmap(originalImage);
            Bitmap grayImage = ConvertToGrayscale(original);
            original.Dispose();

            int width = grayImage.Width;
            int height = grayImage.Height;

            int windowSize = 15;
            double k = 0.2;

            // Попытка получить значения для размера окна и чувствительности из полей панели
            if (panelOptions != null && panelOptions.Controls.Count >= 4)
            {
                TextBox tbWindowSize = panelOptions.Controls[1] as TextBox;
                TextBox tbK = panelOptions.Controls[3] as TextBox;
                if (tbWindowSize != null)
                    int.TryParse(tbWindowSize.Text, out windowSize);
                if (tbK != null)
                    double.TryParse(tbK.Text, out k);
            }

            int halfWindow = windowSize / 2;
            Bitmap binaryImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataBinary = binaryImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int strideGray = dataGray.Stride;
            int strideBinary = dataBinary.Stride;

            // Динамический диапазон для 8-битных изображений из методички
            const double R = 128.0;

            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                byte* ptrBinary = (byte*)dataBinary.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int xMin = x - halfWindow;
                        int xMax = x + halfWindow;
                        int yMin = y - halfWindow;
                        int yMax = y + halfWindow;

                        if (xMin < 0) xMin = 0;
                        if (yMin < 0) yMin = 0;
                        if (xMax >= width) xMax = width - 1;
                        if (yMax >= height) yMax = height - 1;

                        int count = 0;
                        double sum = 0;
                        double sumSq = 0;

                        // Работа с окном соседних пикселей
                        for (int j = yMin; j <= yMax; j++)
                        {
                            for (int i = xMin; i <= xMax; i++)
                            {
                                int idx = j * strideGray + i * 3;
                                byte intensity = ptrGray[idx];
                                count++;
                                sum += intensity;
                                sumSq += intensity * intensity;
                            }
                        }

                        double mean = sum / count;
                        double variance = (sumSq / count) - (mean * mean);
                        double std = Math.Sqrt(variance);

                        // Вычисление локального значения для пикселя по указанной формуле
                        double localThreshold = mean * (1 + k * ((std / R) - 1));

                        // Бинаризация
                        int index = y * strideGray + x * 3;
                        byte pixelIntensity = ptrGray[index];
                        byte color = (pixelIntensity <= localThreshold) ? (byte)0 : (byte)255;

                        int indexBinary = y * strideBinary + x * 3;
                        ptrBinary[indexBinary] = color;
                        ptrBinary[indexBinary + 1] = color;
                        ptrBinary[indexBinary + 2] = color;
                    }
                }
            }

            grayImage.UnlockBits(dataGray);
            binaryImage.UnlockBits(dataBinary);
            grayImage.Dispose();

            pictureBox1.Image = binaryImage;
            pictureBox1.Refresh();
        }

        // Реализация метода Вульфа
        private void WolfMethod()
        {
            if (originalImage == null)
                return;

            Bitmap original = new Bitmap(originalImage);
            Bitmap grayImage = ConvertToGrayscale(original);
            original.Dispose();

            int width = grayImage.Width;
            int height = grayImage.Height;

            int windowSize = 15;
            double a = 0.5;

            // Попытка получить значения для размера окна и чувствительности из полей панели
            if (panelOptions != null && panelOptions.Controls.Count >= 4)
            {
                TextBox tbWindowSize = panelOptions.Controls[1] as TextBox;
                TextBox tbA = panelOptions.Controls[3] as TextBox;
                if (tbWindowSize != null)
                    int.TryParse(tbWindowSize.Text, out windowSize);
                if (tbA != null)
                    double.TryParse(tbA.Text, out a);
            }

            int halfWindow = windowSize / 2;

            // Первый проход - глобальное максимальное стандартное отклонение по всем окнам на изображении(s_max)
            double globalSMax = 0.0;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int strideGray = dataGray.Stride;
            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int xMin = x - halfWindow;
                        int xMax = x + halfWindow;
                        int yMin = y - halfWindow;
                        int yMax = y + halfWindow;
                        if (xMin < 0) xMin = 0;
                        if (yMin < 0) yMin = 0;
                        if (xMax >= width) xMax = width - 1;
                        if (yMax >= height) yMax = height - 1;

                        int count = 0;
                        double sum = 0;
                        double sumSq = 0;

                        // Работа с окном соседних пикселей
                        for (int j = yMin; j <= yMax; j++)
                        {
                            for (int i = xMin; i <= xMax; i++)
                            {
                                int idx = j * strideGray + i * 3;
                                byte intensity = ptrGray[idx];
                                count++;
                                sum += intensity;
                                sumSq += intensity * intensity;
                            }
                        }

                        double mean = sum / count;
                        double variance = (sumSq / count) - (mean * mean);
                        double std = variance > 0 ? Math.Sqrt(variance) : 0;
                        if (std > globalSMax)
                            globalSMax = std;
                    }
                }
            }
            grayImage.UnlockBits(dataGray);

            // Второй проход - бинаризация с использованием метода Вульфа
            Bitmap binaryImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataBinary = binaryImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int strideBinary = dataBinary.Stride;
            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                byte* ptrBinary = (byte*)dataBinary.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int xMin = x - halfWindow;
                        int xMax = x + halfWindow;
                        int yMin = y - halfWindow;
                        int yMax = y + halfWindow;
                        if (xMin < 0) xMin = 0;
                        if (yMin < 0) yMin = 0;
                        if (xMax >= width) xMax = width - 1;
                        if (yMax >= height) yMax = height - 1;

                        int count = 0;
                        double sum = 0;
                        double sumSq = 0;

                        // Работа с окном соседних пикселей
                        for (int j = yMin; j <= yMax; j++)
                        {
                            for (int i = xMin; i <= xMax; i++)
                            {
                                int idx = j * strideGray + i * 3;
                                byte intensity = ptrGray[idx];
                                count++;
                                sum += intensity;
                                sumSq += intensity * intensity;
                            }
                        }

                        double mean = sum / count;
                        double variance = (sumSq / count) - (mean * mean);
                        double std = variance > 0 ? Math.Sqrt(variance) : 0;

                        // Вычисление локального значения для пикселя по указанной формуле
                        double localThreshold = globalSMax > 0 ? mean + a * ((std) / globalSMax) * (255 - mean) : mean;

                        // Бинаризация
                        int index = y * strideGray + x * 3;
                        byte pixelIntensity = ptrGray[index];
                        byte color = (pixelIntensity <= localThreshold) ? (byte)0 : (byte)255;

                        int indexBinary = y * strideBinary + x * 3;
                        ptrBinary[indexBinary] = color;
                        ptrBinary[indexBinary + 1] = color;
                        ptrBinary[indexBinary + 2] = color;
                    }
                }
            }
            grayImage.UnlockBits(dataGray);
            binaryImage.UnlockBits(dataBinary);
            grayImage.Dispose();

            pictureBox1.Image = binaryImage;
            pictureBox1.Refresh();
        }

        // Реализация методом Бредли-Рота
        private void BradleyRothMethod()
        {
            if (originalImage == null)
                return;

            Bitmap original = new Bitmap(originalImage);
            Bitmap grayImage = ConvertToGrayscale(original);
            original.Dispose();

            int width = grayImage.Width;
            int height = grayImage.Height;

            int windowSize = 15;
            double t = 0.15;

            // Попытка получить значения для размера окна и чувствительности из полей панели
            if (panelOptions != null && panelOptions.Controls.Count >= 4)
            {
                TextBox tbWindowSize = panelOptions.Controls[1] as TextBox;
                TextBox tbT = panelOptions.Controls[3] as TextBox;
                if (tbWindowSize != null)
                    int.TryParse(tbWindowSize.Text, out windowSize);
                if (tbT != null)
                    double.TryParse(tbT.Text, out t);
            }

            int halfWindow = windowSize / 2;

            // Первый проход - вычисление интегрального изображения
            long[,] integral = new long[width, height];
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int strideGray = dataGray.Stride;
            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * strideGray + x * 3;
                        int intensity = ptrGray[index];
                        if (x == 0 && y == 0)
                            integral[x, y] = intensity;
                        else if (x == 0)
                            integral[x, y] = intensity + integral[x, y - 1];
                        else if (y == 0)
                            integral[x, y] = intensity + integral[x - 1, y];
                        else
                            integral[x, y] = intensity + integral[x - 1, y] + integral[x, y - 1] - integral[x - 1, y - 1];
                    }
                }
            }
            grayImage.UnlockBits(dataGray);

            // Второй проход - пороговая обработка с использованием интегрального изображения
            Bitmap binaryImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            dataGray = grayImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataBinary = binaryImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            strideGray = dataGray.Stride;
            int strideBinary = dataBinary.Stride;
            unsafe
            {
                byte* ptrGray = (byte*)dataGray.Scan0.ToPointer();
                byte* ptrBinary = (byte*)dataBinary.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int x1 = x - halfWindow;
                        int y1 = y - halfWindow;
                        int x2 = x + halfWindow;
                        int y2 = y + halfWindow;

                        if (x1 < 0) x1 = 0;
                        if (y1 < 0) y1 = 0;
                        if (x2 >= width) x2 = width - 1;
                        if (y2 >= height) y2 = height - 1;

                        int area = (x2 - x1 + 1) * (y2 - y1 + 1);
                        long sum = integral[x2, y2];
                        if (x1 > 0) sum -= integral[x1 - 1, y2];
                        if (y1 > 0) sum -= integral[x2, y1 - 1];
                        if (x1 > 0 && y1 > 0) sum += integral[x1 - 1, y1 - 1];

                        double average = (double)sum / area;
                        double threshold = average * (1.0 - t);

                        int index = y * strideGray + x * 3;
                        int intensity = ptrGray[index];
                        byte color = (byte)((intensity < threshold) ? 0 : 255);

                        int indexBinary = y * strideBinary + x * 3;
                        ptrBinary[indexBinary] = color;
                        ptrBinary[indexBinary + 1] = color;
                        ptrBinary[indexBinary + 2] = color;
                    }
                }
            }
            grayImage.UnlockBits(dataGray);
            binaryImage.UnlockBits(dataBinary);
            grayImage.Dispose();

            pictureBox1.Image = binaryImage;
            pictureBox1.Refresh();
        }

        // Бинаризация изображения по выбранному методу
        private void BtnBinarize_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Изображение не загружено", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string selectedMethod = comboBox1.SelectedItem.ToString();
            if (selectedMethod == "Метод Гаврилова")
            {
                GavrMethod();
            }
            else if (selectedMethod == "Метод Ниблека")
            {
                NiblackMethod();
            }
            else if (selectedMethod == "Метод Сауволы")
            {
                SauvolaMethod();
            }
            else if (selectedMethod == "Метод Вульфа")
            {
                WolfMethod();
            }
            else if (selectedMethod == "Метод Бредли-Рота")
            {
                BradleyRothMethod();
            }
            else if (selectedMethod == "Метод Отсу")
            {
                OtsuMethod();
            }
            else
            {
                MessageBox.Show("Выбран неизвестный метод", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}