using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace lab1.Lr2
{
    public partial class GradForm : Form
    {
        // Таймер для обновления изображения
        private Timer transformTimer;
        // Флаг для отслеживания необходимости обновления
        private bool pendingUpdate = false;
        // Исходное изображение
        private Bitmap _originalImage;

        public GradForm()
        {
            InitializeComponent();
            comboInterpolation.Items.Clear();
            comboInterpolation.Items.Add("Линейная");
            comboInterpolation.Items.Add("Кубическая");
            comboInterpolation.Items.Add("Кривая Безье");
            comboInterpolation.Items.Add("Полиномиальная");
            comboInterpolation.SelectedIndex = 0;
            btnReset.Click += btnReset_Click;
            panelCurve.CurveChanged += () => ApplyTransformation();
            comboInterpolation.SelectedIndexChanged += comboInterpolation_SelectedIndexChanged;
        }

        // Обработчик изменения выбранного типа интерполяции
        private void comboInterpolation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string interpolationType = comboInterpolation.SelectedItem.ToString();
            MessageBox.Show($"Выбрана интерполяция: {interpolationType}");
            ApplyTransformation();      
        }

        // Исходное изображение и обновление гистограммы
        private void GradForm_Load(object sender, EventArgs e)
        {
            _originalImage = new Bitmap("../../Assets/stankinimg.jpg");
            pictureBoxResult.Image = _originalImage;
            UpdateHistogram(_originalImage);
        }

        // Обновление гистограммы изображения
        private void UpdateHistogram(Bitmap image)
        {
            int[] histogram = new int[256];
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int bytes = Math.Abs(data.Stride) * image.Height;
                for (int i = 0; i < bytes; i += 3)
                {
                    int brightness = (ptr[i] + ptr[i + 1] + ptr[i + 2]) / 3;
                    histogram[brightness]++;
                }
            }
            image.UnlockBits(data);
            DrawHistogram(histogram);
        }

        // Рисует гистограмму на основе данных
        private void DrawHistogram(int[] histogram)
        {
            int fullWidth = this.ClientSize.Width;
            int histHeight = pictureBoxHistogram.Height;
            Bitmap bmp = new Bitmap(fullWidth, histHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                int max = histogram.Max();
                if (max == 0) max = 1;
                float scaleX = (float)fullWidth / 256;
                float barWidth = scaleX * 2f;
                using (Brush brush = new SolidBrush(Color.Blue))
                {
                    for (int i = 0; i < 256; i++)
                    {
                        float x = i * scaleX;
                        float heightLine = histogram[i] * histHeight / (float)max;

                        g.FillRectangle(brush, x, histHeight - heightLine, barWidth, heightLine);
                    }
                }
            }
            pictureBoxHistogram.Image = bmp;
        }

        // Применяем выбранную интерполяцию к изображению
        private void ApplyTransformation()
        {
            if (_originalImage == null) return;

            IInterpolation interpolator;
            switch(comboInterpolation.SelectedIndex)
            {
                case 0:
                    interpolator = new linearInterpolation();
                    break;
                case 1:
                    interpolator = new CubicInterpolation();
                    break;
                case 2:
                    interpolator = new BezierInterpolation();
                    break;
                case 3:
                    interpolator = new PolynomialInterpolation();
                    break;
                default:
                    interpolator = new linearInterpolation();
                    break;
            }

            // Интерполятор в panelCurve для отрисовки сплайна
            panelCurve.Interpolator = interpolator;

            // Точки кривой без инвертирования Y
            var points = panelCurve._points
                .OrderBy(p => p.X)
                .Select(p => new MyPoint(p.X, p.Y))
                .ToArray();

            // Интерполяция
            interpolator.calc(points);

            // Обновление панель для перерисовки кривой
            panelCurve.Invalidate();

            // Преобразование к изображению с использованием интерполяторной функции
            Bitmap result = ProcessImage(_originalImage, interpolator);
            pictureBoxResult.Image = result;
            UpdateHistogram(result);
        }

        // Обработчик загрузки изображения
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _originalImage = new Bitmap(openFileDialog.FileName);
                pictureBoxResult.Image = _originalImage;
                UpdateHistogram(_originalImage);
            }
        }

        // Обработчик кнопки сброса.
        private void btnReset_Click(object sender, EventArgs e)
        {
            panelCurve._points.Clear();
            panelCurve._points.Add(new PointF(0, 0));     // Фиксированная точка (0,0)
            panelCurve._points.Add(new PointF(255, 255)); // Фиксированная точка (255,255)
            panelCurve.Invalidate();

            // Сброс изображения и гистограммы
            if (_originalImage != null)
            {
                pictureBoxResult.Image = _originalImage;
                UpdateHistogram(_originalImage);
            }

            ApplyTransformation();
        }

        // Обрабатывает изображение, применяя интерполяцию.
        private Bitmap ProcessImage(Bitmap original, IInterpolation interpolator)
        {
            int width = original.Width;
            int height = original.Height;
            Bitmap result = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // Предварительное вычисление таблицы соответствий для значений от 0 до 255
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                lut[i] = (byte)Math.Min(255, Math.Max(0, (int)Math.Round(interpolator.f(i))));
            }

            BitmapData origData = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* origPtr = (byte*)origData.Scan0;
                byte* resultPtr = (byte*)resultData.Scan0;
                int stride = origData.Stride;
                System.Threading.Tasks.Parallel.For(0, height, y => {
                    byte* srcRow = origPtr + y * stride;
                    byte* dstRow = resultPtr + y * stride;
                    int numPixels = width;
                    int limit = (numPixels / 4) * 4;
                    int x = 0;
                    // Развертка цикла: обрабатываем 4 пикселя за итерацию
                    for (; x < limit; x += 4)
                    {
                        dstRow[0] = lut[srcRow[0]];
                        dstRow[1] = lut[srcRow[1]];
                        dstRow[2] = lut[srcRow[2]];

                        dstRow[3] = lut[srcRow[3]];
                        dstRow[4] = lut[srcRow[4]];
                        dstRow[5] = lut[srcRow[5]];

                        dstRow[6] = lut[srcRow[6]];
                        dstRow[7] = lut[srcRow[7]];
                        dstRow[8] = lut[srcRow[8]];

                        dstRow[9] = lut[srcRow[9]];
                        dstRow[10] = lut[srcRow[10]];
                        dstRow[11] = lut[srcRow[11]];

                        srcRow += 12;
                        dstRow += 12;
                    }
                    // Обработка оставшихся пикселей
                    for (; x < numPixels; x++)
                    {
                        dstRow[0] = lut[srcRow[0]];
                        dstRow[1] = lut[srcRow[1]];
                        dstRow[2] = lut[srcRow[2]];
                        srcRow += 3;
                        dstRow += 3;
                    }
                });
            }

            original.UnlockBits(origData);
            result.UnlockBits(resultData);
            return result;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuForm menu = new MenuForm();
            menu.Show();

            this.Close();
        }
    }
}
