using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.IO;
using MathNet.Numerics.IntegralTransforms;

namespace lab1.Lr5
{
    public partial class ChFilteringForm : Form
    {
        private Button buttonLoadImage;
        private Button buttonShowFourier;
        private ComboBox comboFilterType;
        private NumericUpDown numericRadius1;
        private NumericUpDown numericRadius2;
        private Button buttonApplyFilter;
        private Button buttonInverse;
        private Button buttonSaveResult;
        private PictureBox pictureBoxOriginal;
        private PictureBox pictureBoxFourier;
        private PictureBox pictureBoxFilterMask;
        private PictureBox pictureBoxResult;
        private Label labelDragDrop;
        private Button btnDeleteImage;

        private Bitmap originalImage;

        private Complex[,] dftRed;
        private Complex[,] dftGreen;
        private Complex[,] dftBlue;

        private int width, height;

        private bool fourierComputed = false;
        private bool filterApplied = false;
        private bool usedCooleyTukeyForForwardDFT = false;

        public ChFilteringForm()
        {
            InitializeComponent();

            InitializeComponents();

            btnDeleteImage.Enabled = false;
            comboFilterType.Enabled = false;
            numericRadius1.Enabled = false;
            numericRadius2.Enabled = false;
            buttonApplyFilter.Enabled = false;
            buttonShowFourier.Enabled = false;            
            buttonInverse.Enabled = false;

            labelDragDrop.BringToFront();
        }

        private void InitializeComponents()
        {
            this.Text = "Частотная фильтрация";
            this.ClientSize = new Size(900, 720);

            comboFilterType = new ComboBox() { Left = 20, Top = 20, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            numericRadius1 = new NumericUpDown() { Left = 20, Top = 50, Width = 60, Minimum = 0, Maximum = 10000, Value = 30 };
            numericRadius2 = new NumericUpDown() { Left = 110, Top = 50, Width = 60, Minimum = 0, Maximum = 10000, Value = 60 };
            buttonShowFourier = new Button() { Text = "DFT", Left = 20, Top = 100, Width = 150 };           
            buttonApplyFilter = new Button() { Text = "Фильтр", Left = 20, Top = 150, Width = 150 };
            buttonInverse = new Button() { Text = "IDFT", Left = 20, Top = 200, Width = 150 };
            btnDeleteImage = new Button() { Text = "Удалить изображение", Left = 20, Top = 250, Width = 150 };

            pictureBoxOriginal = new PictureBox() { BorderStyle = BorderStyle.FixedSingle, Left = 200, Top = 20, Width = 300, Height = 300, SizeMode = PictureBoxSizeMode.Zoom };
            pictureBoxFourier = new PictureBox() { BorderStyle = BorderStyle.FixedSingle, Left = 200, Top = 370, Width = 300, Height = 300, SizeMode = PictureBoxSizeMode.Zoom };
            pictureBoxFilterMask = new PictureBox() { BorderStyle = BorderStyle.FixedSingle, Left = 550, Top = 370, Width = 300, Height = 300, SizeMode = PictureBoxSizeMode.Zoom };
            pictureBoxResult = new PictureBox() { BorderStyle = BorderStyle.FixedSingle, Left = 550, Top = 20, Width = 300, Height = 300, SizeMode = PictureBoxSizeMode.Zoom };          
            
            labelDragDrop = new Label();
            labelDragDrop.Text = "Перетащите изображение на форму";
            labelDragDrop.AutoSize = true;
            labelDragDrop.Location = new Point(pictureBoxOriginal.Left + 10, pictureBoxOriginal.Top + 10);
            this.Controls.Add(labelDragDrop);

            comboFilterType.Items.Add("Низкочастотный");
            comboFilterType.Items.Add("Высокочастотный");
            comboFilterType.Items.Add("Режекторный");
            comboFilterType.Items.Add("Полосовой");
            comboFilterType.Items.Add("Узкополосный режекторный");
            comboFilterType.Items.Add("Узкополосный полосовой");
            comboFilterType.SelectedIndex = 0;

            buttonShowFourier.Click += ButtonShowFourier_Click;
            buttonApplyFilter.Click += ButtonApplyFilter_Click;
            buttonInverse.Click += ButtonInverse_Click;
            btnDeleteImage.Click += BtnDeleteImage_Click;
            comboFilterType.SelectedIndexChanged += comboFilterType_SelectedIndexChanged;
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(BinForm_DragEnter);
            this.DragDrop += new DragEventHandler(BinForm_DragDrop);

            this.Controls.Add(buttonShowFourier);
            this.Controls.Add(comboFilterType);
            this.Controls.Add(numericRadius1);
            this.Controls.Add(numericRadius2);
            this.Controls.Add(buttonApplyFilter);
            this.Controls.Add(buttonInverse);
            this.Controls.Add(pictureBoxOriginal);
            this.Controls.Add(pictureBoxFourier);
            this.Controls.Add(pictureBoxFilterMask);
            this.Controls.Add(pictureBoxResult);
            this.Controls.Add(btnDeleteImage);
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
                if (pictureBoxOriginal.Image == null)
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
                        const int maxDimension = 300;
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
                        originalImage = new Bitmap(img);
                        pictureBoxOriginal.Image = img;
                        labelDragDrop.Visible = false;
                        btnDeleteImage.Enabled = true;
                        buttonShowFourier.Enabled = true;
                        fourierComputed = false;
                        filterApplied = false;
                        pictureBoxFourier.Image = null;
                        pictureBoxFilterMask.Image = null;
                        pictureBoxResult.Image = null;
                        width = originalImage.Width;
                        height = originalImage.Height;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                    }
                }
            }
        }

        private void comboFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pictureBoxFourier.Image != null)
            {
                pictureBoxFourier.Image.Dispose();
                pictureBoxFourier.Image = null;
            }
            if (pictureBoxFilterMask.Image != null)
            {
                pictureBoxFilterMask.Image.Dispose();
                pictureBoxFilterMask.Image = null;
            }
            if (pictureBoxResult.Image != null)
            {
                pictureBoxResult.Image.Dispose();
                pictureBoxResult.Image = null;
            }

            fourierComputed = false;
            filterApplied = false;
            usedCooleyTukeyForForwardDFT = false;

            comboFilterType.Enabled = false;
            buttonApplyFilter.Enabled = false;
            buttonInverse.Enabled = false;
            numericRadius1.Enabled = false;
            numericRadius2.Enabled = false;
        }

        private void BtnDeleteImage_Click(object sender, EventArgs e)
        {
            if (originalImage != null)
            {
                originalImage.Dispose();
                originalImage = null;
            }
            if (pictureBoxOriginal.Image != null)
            {
                pictureBoxOriginal.Image.Dispose();
                pictureBoxOriginal.Image = null;             
            }
            if (pictureBoxFilterMask.Image != null)
            {
                pictureBoxFilterMask.Image.Dispose();
                pictureBoxFilterMask.Image = null;
            }
            if (pictureBoxResult.Image != null)
            {
                pictureBoxResult.Image.Dispose();
                pictureBoxResult.Image = null;
            }
            if (pictureBoxFourier.Image != null)
            {
                pictureBoxFourier.Image.Dispose();
                pictureBoxFourier.Image = null;
            }

            labelDragDrop.Visible = true;
            btnDeleteImage.Enabled = false;
            btnDeleteImage.Enabled = false;
            comboFilterType.Enabled = false;
            numericRadius1.Enabled = false;
            numericRadius2.Enabled = false;
            buttonApplyFilter.Enabled = false;
            buttonShowFourier.Enabled = false;
            buttonInverse.Enabled = false;

            // Сброс состояния
            fourierComputed = false;
            filterApplied = false;
            usedCooleyTukeyForForwardDFT = false;
            dftRed = null;
            dftGreen = null;
            dftBlue = null;
            width = 0;
            height = 0;
        }

        private void ButtonShowFourier_Click(object sender, EventArgs e)
        {
            if (originalImage == null) 
            {
                MessageBox.Show("Сначала загрузите изображение");
                return;
            }

            double[,] redChannel = new double[height, width];
            double[,] greenChannel = new double[height, width];
            double[,] blueChannel = new double[height, width];

            BitmapData imgData = originalImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, originalImage.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(originalImage.PixelFormat) / 8;
            unsafe {
                byte* scan0 = (byte*)imgData.Scan0;
                for (int y = 0; y < height; y++) {
                    byte* row = scan0 + y * imgData.Stride;
                    for (int x = 0; x < width; x++) {
                        int pos = x * bytesPerPixel;
                        byte b = row[pos];
                        byte g = row[pos + 1];
                        byte r = row[pos + 2];
                        double sign = ((x + y) % 2 == 0) ? 1.0 : -1.0;
                        redChannel[y, x] = r * sign;
                        greenChannel[y, x] = g * sign;
                        blueChannel[y, x] = b * sign;
                    }
                }
            }
            originalImage.UnlockBits(imgData);

            bool canUseCooleyTukey = IsPowerOfTwo(width) && IsPowerOfTwo(height);

            if (canUseCooleyTukey)
            {
                MessageBox.Show("Используется алгоритм Кули-Тьюки");
                dftRed = DFT2D_CooleyTukey(redChannel);
                dftGreen = DFT2D_CooleyTukey(greenChannel);
                dftBlue = DFT2D_CooleyTukey(blueChannel);
                usedCooleyTukeyForForwardDFT = true;
            }
            else
            {
                MessageBox.Show("Используется обычный алгоритм Фурье");
                dftRed = DFT2D(redChannel);
                dftGreen = DFT2D(greenChannel);
                dftBlue = DFT2D(blueChannel);
                usedCooleyTukeyForForwardDFT = false;
            }

            Bitmap fourierMap = BuildMagnitudeImage(dftRed, dftGreen, dftBlue);

            pictureBoxFourier.Image?.Dispose();
            pictureBoxFourier.Image = fourierMap;

            fourierComputed = true;
            filterApplied = false;
            
            pictureBoxFilterMask.Image?.Dispose();
            pictureBoxFilterMask.Image = null;

            pictureBoxResult.Image?.Dispose();
            pictureBoxResult.Image = null;

            numericRadius1.Enabled = true;
            numericRadius2.Enabled = true;
            comboFilterType.Enabled = true;
            buttonApplyFilter.Enabled = true;
        }

        private void ButtonApplyFilter_Click(object sender, EventArgs e)
        {
            if (!fourierComputed)
            {
                MessageBox.Show("Сначала вычислите DFT");
            }

            double r1 = (double)numericRadius1.Value;
            double r2 = (double)numericRadius2.Value;

            if ((comboFilterType.SelectedIndex == 2 || comboFilterType.SelectedIndex == 3 ||
                 comboFilterType.SelectedIndex == 4 || comboFilterType.SelectedIndex == 5) && r1 == r2)
            {
                MessageBox.Show("Для режекторных и полосовых фильтров радиусы r1 и r2 должны отличаться");
                return;
            }

            int filterType = comboFilterType.SelectedIndex;

            (bool[,] maskRed, bool[,] maskGreen, bool[,] maskBlue) =
                ApplyFilter(dftRed, dftGreen, dftBlue, filterType, r1, r2);

            Bitmap maskImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData maskData = maskImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            unsafe {
                byte* maskPtr = (byte*)maskData.Scan0;
                for (int y = 0; y < height; y++) {
                    byte* row = maskPtr + y * maskData.Stride;
                    for (int x = 0; x < width; x++) {
                        int pos = x * 3;
                        bool white = maskRed[y, x] || maskGreen[y, x] || maskBlue[y, x];
                        byte value = white ? (byte)255 : (byte)0;
                        row[pos] = value;
                        row[pos + 1] = value;
                        row[pos + 2] = value;
                    }
                }
            }
            maskImage.UnlockBits(maskData);
            pictureBoxFilterMask.Image?.Dispose();
            pictureBoxFilterMask.Image = maskImage;

            Bitmap fourierMap = BuildMagnitudeImage(dftRed, dftGreen, dftBlue);
            pictureBoxFourier.Image?.Dispose();
            pictureBoxFourier.Image = fourierMap;

            filterApplied = true;

            pictureBoxResult.Image?.Dispose();
            pictureBoxResult.Image = null;

            buttonInverse.Enabled = true;
        }

        private void ButtonInverse_Click(object sender, EventArgs e)
        {
            if (!fourierComputed || !filterApplied)
            {
                MessageBox.Show("Сначала нужно вычислить ДПФ и фильтры");
                return;
            }

            double[,] redChannel, greenChannel, blueChannel;

            if (usedCooleyTukeyForForwardDFT)
            {
                redChannel = IDFT2D_CooleyTukey(dftRed);
                greenChannel = IDFT2D_CooleyTukey(dftGreen);
                blueChannel = IDFT2D_CooleyTukey(dftBlue);
            }
            else
            {
                redChannel = IDFT2D(dftRed);
                greenChannel = IDFT2D(dftGreen);
                blueChannel = IDFT2D(dftBlue);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sign = ((x + y) % 2 == 0) ? 1.0 : -1.0;
                    redChannel[y, x] *= sign;
                    greenChannel[y, x] *= sign;
                    blueChannel[y, x] *= sign;
                }
            }

            Bitmap result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            unsafe {
                byte* resPtr = (byte*)resultData.Scan0;
                for (int y = 0; y < height; y++) 
                {
                    byte* row = resPtr + y * resultData.Stride;
                    for (int x = 0; x < width; x++) 
                    {
                        int R = (int)Math.Round(redChannel[y, x]);
                        int G = (int)Math.Round(greenChannel[y, x]);
                        int B = (int)Math.Round(blueChannel[y, x]);
                        R = Math.Min(255, Math.Max(0, R));
                        G = Math.Min(255, Math.Max(0, G));
                        B = Math.Min(255, Math.Max(0, B));
                        int pos = x * 3;
                        row[pos] = (byte)B;
                        row[pos + 1] = (byte)G;
                        row[pos + 2] = (byte)R;
                    }
                }
            }
            result.UnlockBits(resultData);

            //result.RotateFlip(RotateFlipType.Rotate180FlipNone);
            pictureBoxResult.Image?.Dispose();
            pictureBoxResult.Image = result;
        }

        private Complex[,] DFT2D(double[,] data)
        {
            int H = data.GetLength(0);
            int W = data.GetLength(1);
            Complex[,] output = new Complex[H, W];

            Complex[,] temp = new Complex[H, W];
            for (int y = 0; y < H; y++)
            {
                Complex[] rowIn = new Complex[W];
                for (int x = 0; x < W; x++)
                {
                    rowIn[x] = new Complex(data[y, x], 0);
                }

                Complex[] rowOut = DFT1D(rowIn);

                for (int x = 0; x < W; x++)
                {
                    temp[y, x] = rowOut[x];
                }
            }


            for (int x = 0; x < W; x++)
            {

                Complex[] colIn = new Complex[H];
                for (int y = 0; y < H; y++)
                {
                    colIn[y] = temp[y, x];
                }

                Complex[] colOut = DFT1D(colIn);

                for (int y = 0; y < H; y++)
                {
                    output[y, x] = colOut[y];
                }
            }

            return output;
        }

        private Complex[] DFT1D(Complex[] input)
        {

            // int N = input.Length;
            // Complex[] output = new Complex[N];
            // for (int u = 0; u < N; u++)
            // {
            //     Complex sum = Complex.Zero;
            //     for (int k = 0; k < N; k++)
            //     {
            //         double angle = -2.0 * Math.PI * u * k / N;
            //         // e^(-i*angle) = cos(angle) - i sin(angle)
            //         Complex w = new Complex(Math.Cos(angle), Math.Sin(angle) * -1.0);
            //         sum += input[k] * w;
            //     }
            //     output[u] = sum; // без нормировки
            // }
            // return output;

            Complex[] result = (Complex[])input.Clone();
            Fourier.Forward(result, FourierOptions.NoScaling);
            return result;
        }

        private double[,] IDFT2D(Complex[,] data)
        {
            int H = data.GetLength(0);
            int W = data.GetLength(1);

            Complex[,] temp = new Complex[H, W];
            for (int y = 0; y < H; y++)
            {
                Complex[] rowIn = new Complex[W];
                for (int x = 0; x < W; x++)
                {
                    rowIn[x] = data[y, x];
                }

                Complex[] rowOut = IDFT1D(rowIn);
                for (int x = 0; x < W; x++)
                {
                    temp[y, x] = rowOut[x];
                }
            }


            double[,] output = new double[H, W];
            for (int x = 0; x < W; x++)
            {

                Complex[] colIn = new Complex[H];
                for (int y = 0; y < H; y++)
                {
                    colIn[y] = temp[y, x];
                }

                Complex[] colOut = IDFT1D(colIn);

                for (int y = 0; y < H; y++)
                {
                    output[y, x] = colOut[y].Real;
                }
            }

            double norm = (double)(W * H);
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    output[y, x] /= norm;
                }
            }

            return output;
        }

        private Complex[] IDFT1D(Complex[] input)
        {

            // int N = input.Length;
            // Complex[] output = new Complex[N];
            // for (int u = 0; u < N; u++)
            // {
            //     Complex sum = Complex.Zero;
            //     for (int k = 0; k < N; k++)
            //     {
            //         double angle = 2.0 * Math.PI * u * k / N; // отличие от прямого в знаке
            //         // e^(i*angle) = cos(angle) + i sin(angle)
            //         Complex w = new Complex(Math.Cos(angle), Math.Sin(angle));
            //         sum += input[k] * w;
            //     }
            //     output[u] = sum; // без деления на N (делим потом скопом)
            // }
            // return output;

            Complex[] result = (Complex[])input.Clone();
            Fourier.Inverse(result, FourierOptions.NoScaling);
            return result;
        }

        private Bitmap BuildMagnitudeImage(Complex[,] dftR, Complex[,] dftG, Complex[,] dftB)
        {
            int H = dftR.GetLength(0);
            int W = dftR.GetLength(1);
            Bitmap bmp = new Bitmap(W, H, PixelFormat.Format24bppRgb);

            // Найдём максимум для логарифмического отображения
            double maxVal = 0.0;
            double[,] magnitudeMap = new double[H, W];

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    // Берём модуль (R,G,B) и усредняем
                    double magR = dftR[y, x].Magnitude;
                    double magG = dftG[y, x].Magnitude;
                    double magB = dftB[y, x].Magnitude;
                    double mag = (magR + magG + magB) / 3.0;
                    magnitudeMap[y, x] = mag;
                    if (mag > maxVal) maxVal = mag;
                }
            }

            double cmax = Math.Log(1 + maxVal);
            BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            unsafe {
                byte* ptr = (byte*)bmd.Scan0;
                for (int y = 0; y < H; y++) {
                    byte* row = ptr + y * bmd.Stride;
                    for (int x = 0; x < W; x++) {
                        double val = magnitudeMap[y, x];
                        double c = Math.Log(1 + val);
                        double gray = c / cmax * 255.0;
                        int g = (int)Math.Round(gray);
                        g = Math.Max(0, Math.Min(255, g));
                        int pos = x * 3;
                        row[pos] = (byte)g;      // Blue
                        row[pos + 1] = (byte)g;  // Green
                        row[pos + 2] = (byte)g;  // Red
                    }
                }
            }
            bmp.UnlockBits(bmd);

            return bmp;
        }

        private (bool[,], bool[,], bool[,]) ApplyFilter(Complex[,] dftR, Complex[,] dftG, Complex[,] dftB,
            int filterType, double r1, double r2)
        {
            int H = dftR.GetLength(0);
            int W = dftR.GetLength(1);

            // Центр частот
            double cy = H / 2.0;
            double cx = W / 2.0;

            bool[,] maskR = new bool[H, W];
            bool[,] maskG = new bool[H, W];
            bool[,] maskB = new bool[H, W];

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    double dy = (y - cy);
                    double dx = (x - cx);
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    bool zeroIt = false;

                    switch (filterType)
                    {
                        case 0: // Low Pass – обнуляем, если dist > r1
                            if (dist > r1) zeroIt = true;
                            break;
                        case 1: // High Pass – обнуляем, если dist < r1
                            if (dist < r1) zeroIt = true;
                            break;
                        case 2: // Notch (режекторный) – обнуляем, если r1 < dist < r2
                            if (dist > r1 && dist < r2) zeroIt = true;
                            break;
                        case 3: // Band Pass (полосовой) – обнуляем, если dist < r1 или dist > r2
                            if (dist < r1 || dist > r2) zeroIt = true;
                            break;
                        case 4: // Узкополосный режекторный (Narrow Notch): 
                                // условно, как у Notch, но считаем, что r1 и r2 – &laquo;узкие&raquo; границы. 
                            if (dist > r1 && dist < r2) zeroIt = true;
                            break;
                        case 5: // Узкополосный полосовой (Narrow Band): 
                                // как Band Pass, но опять же r1..r2 – узкая полоска
                            if (dist < r1 || dist > r2) zeroIt = true;
                            break;
                    }

                    if (zeroIt)
                    {
                        dftR[y, x] = Complex.Zero;
                        dftG[y, x] = Complex.Zero;
                        dftB[y, x] = Complex.Zero;

                        maskR[y, x] = true;
                        maskG[y, x] = true;
                        maskB[y, x] = true;
                    }
                    else
                    {
                        maskR[y, x] = false;
                        maskG[y, x] = false;
                        maskB[y, x] = false;
                    }
                }
            }

            return (maskR, maskG, maskB);
        }

        private bool IsPowerOfTwo(int n)
        {
            return (n & (n - 1)) == 0;
        }

        private Complex[] CooleyTukeyFFT(Complex[] input)
        {
            int N = input.Length;
            if (N <= 1) return input;
            
            Complex[] even = new Complex[N / 2];
            Complex[] odd = new Complex[N / 2];
            for (int i = 0; i < N / 2; i++)
            {
                even[i] = input[2 * i];
                odd[i] = input[2 * i + 1];
            }
            
            Complex[] Feven = CooleyTukeyFFT(even);
            Complex[] Fodd = CooleyTukeyFFT(odd);
            
            Complex[] output = new Complex[N];
            for (int k = 0; k < N / 2; k++)
            {
                double angle = -2 * Math.PI * k / N;
                Complex twiddle = new Complex(Math.Cos(angle), Math.Sin(angle));
                output[k] = Feven[k] + twiddle * Fodd[k];
                output[k + N / 2] = Feven[k] - twiddle * Fodd[k];
            }
            return output;
        }

        private Complex[] CooleyTukeyIFFT(Complex[] input)
        {
            int N = input.Length;
            Complex[] conjugated = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                conjugated[i] = Complex.Conjugate(input[i]);
            }
            
            Complex[] fft = CooleyTukeyFFT(conjugated);
            Complex[] output = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                output[i] = Complex.Conjugate(fft[i]) / N;
            }
            return output;
        }

        private Complex[,] DFT2D_CooleyTukey(double[,] data)
        {
            int H = data.GetLength(0);
            int W = data.GetLength(1);
            Complex[,] output = new Complex[H, W];
            Complex[,] temp = new Complex[H, W];
            for (int y = 0; y < H; y++)
            {
                Complex[] rowIn = new Complex[W];
                for (int x = 0; x < W; x++)
                {
                    rowIn[x] = new Complex(data[y, x], 0);
                }
                Complex[] rowOut = CooleyTukeyFFT(rowIn);
                for (int x = 0; x < W; x++)
                {
                    temp[y, x] = rowOut[x];
                }
            }
            for (int x = 0; x < W; x++)
            {
                Complex[] colIn = new Complex[H];
                for (int y = 0; y < H; y++)
                {
                    colIn[y] = temp[y, x];
                }
                Complex[] colOut = CooleyTukeyFFT(colIn);
                for (int y = 0; y < H; y++)
                {
                    output[y, x] = colOut[y];
                }
            }
            return output;
        }

        private double[,] IDFT2D_CooleyTukey(Complex[,] data)
        {
            int H = data.GetLength(0);
            int W = data.GetLength(1);
            Complex[,] temp = new Complex[H, W];
            for (int y = 0; y < H; y++)
            {
                Complex[] rowIn = new Complex[W];
                for (int x = 0; x < W; x++)
                {
                    rowIn[x] = data[y, x];
                }
                Complex[] rowOut = CooleyTukeyIFFT(rowIn);
                for (int x = 0; x < W; x++)
                {
                    temp[y, x] = rowOut[x];
                }
            }
            double[,] output = new double[H, W];
            for (int x = 0; x < W; x++)
            {
                Complex[] colIn = new Complex[H];
                for (int y = 0; y < H; y++)
                {
                    colIn[y] = temp[y, x];
                }
                Complex[] colOut = CooleyTukeyIFFT(colIn);
                for (int y = 0; y < H; y++)
                {
                    output[y, x] = colOut[y].Real;
                }
            }
            return output;
        }
    }
}