using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
        }


        // Лист с изображениями и их параметрами
        private List<ImageLayer> imageLayers = new List<ImageLayer>(0);

       

        // Функция Drag'n'Drop
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".jpg" ||
                    Path.GetExtension(file).ToLower() == ".jpeg" ||
                    Path.GetExtension(file).ToLower() == ".png" ||
                    Path.GetExtension(file).ToLower() == ".bmp" ||
                    Path.GetExtension(file).ToLower() == ".gif")
                {
                    AddImage(file);
                }
            }
        }


        // Добавление изображения в форму, лист, добавление в панель и работа с ним
        private void AddImage(string filePath)
        {
            Bitmap img = new Bitmap(filePath);

            // Добавление изображения в лист
            ImageLayer layer = new ImageLayer(img);
            imageLayers.Add(layer);

            // Миниатюра изображения в панели
            PictureBox pbThumb = new PictureBox
            {
                Image = new Bitmap(img, new Size(80, 80)),
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 80,
                Height = 80,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = layer
            };

            // Название файла в панели
            Label lblName = new Label
            {
                Text = Path.GetFileName(filePath),
                AutoSize = true,
                MaximumSize = new Size(80, 0)
            };

            // Разрешение изображения в панели
            Label lblResolution = new Label
            {
                Text = $"{img.Width}x{img.Height}",
                AutoSize = true,
            };

            // Чекбоксы для цветовых каналов в панели
            CheckBox cbRed = new CheckBox { Text = "R", Checked = true, AutoSize = true };
            CheckBox cbGreen = new CheckBox { Text = "G", Checked = true, AutoSize = true };
            CheckBox cbBlue = new CheckBox { Text = "B", Checked = true, AutoSize = true };

            // Обновление значения каналов цветов картинки через события
            cbRed.CheckedChanged += (s, e) =>
            {
                layer.IsRedChannel = cbRed.Checked;
                UpdateResultImage();
            };
            cbGreen.CheckedChanged += (s, e) =>
            {
                layer.IsGreenChannel = cbGreen.Checked;
                UpdateResultImage();
            };
            cbBlue.CheckedChanged += (s, e) =>
            {
                layer.IsBlueChannel = cbBlue.Checked;
                UpdateResultImage();
            };

            // Контейнер для чекбоксов каналов в панели
            FlowLayoutPanel pnlChannels = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };
            pnlChannels.Controls.Add(cbRed);
            pnlChannels.Controls.Add(cbGreen);
            pnlChannels.Controls.Add(cbBlue);

            // Полоска для прозрачности в панели
            TrackBar tbOpacity = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                Width = 80
            };

            // Обновление значения прозрачности картинки через событие
            tbOpacity.Scroll += (s, e) =>
            {
                layer.Opacity = tbOpacity.Value;
                UpdateResultImage();
            };

            // Список операций в панели
            ComboBox cbOperations = new ComboBox
            {
                Width = 60,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbOperations.Items.AddRange(new string[] { "Normal", "Sum", "Sub", "Mult", "Avg", "Max" });
            cbOperations.SelectedIndex = 0;

            // Обновление значения операции картинки через события
            cbOperations.SelectedIndexChanged += (s, e) =>
            {
                layer.Operation = cbOperations.SelectedItem.ToString();
                UpdateResultImage();
            };

            // Общий контейнер для изображения и элементов в панели
            FlowLayoutPanel container = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                Height = 150
            };

            // Кнопка Удалить в панели
            Button btnDelete = new Button
            {
                Text = "X",
                Width = 35,
                Height = 35
            };

            // Удаление картинки из листа через событие
            btnDelete.Click += (s, e) =>
            {
                pnlImages.Controls.Remove(container);
                var layerToDelete = imageLayers.FirstOrDefault(x => x.Image == img);
                if (layerToDelete != null)
                {
                    layerToDelete.Image.Dispose(); // Освобождаем ресурсы
                    imageLayers.Remove(layerToDelete);
                }
                UpdateResultImage();
            };

            // Кнопка Вверх в панели
            Button btnUp = new Button
            {
                Text = "↑",
                Width = 35,
                Height = 35
            };

            // Изменение порядка картинки из листа через событие и на панели
            btnUp.Click += (s, e) =>
            {
                int index = pnlImages.Controls.IndexOf(container);
                int listIndex = imageLayers.IndexOf(layer);

                if (index > 0)
                {
                    pnlImages.Controls.SetChildIndex(container, index - 1);

                    var temp = imageLayers[listIndex];
                    imageLayers[listIndex] = imageLayers[listIndex - 1];
                    imageLayers[listIndex - 1] = temp;
                }
                UpdateResultImage();
            };

            // Кнопка Вниз в панели
            Button btnDown = new Button
            {
                Text = "↓",
                Width = 35,
                Height = 35
            };

            // Изменение порядка картинки из листа через событие и на панели
            btnDown.Click += (s, e) =>
            {
                int index = pnlImages.Controls.IndexOf(container);
                int listIndex = imageLayers.IndexOf(layer);

                if (index < pnlImages.Controls.Count - 1)
                {
                    pnlImages.Controls.SetChildIndex(container, index + 1);

                    var temp = imageLayers[listIndex];
                    imageLayers[listIndex] = imageLayers[listIndex + 1];
                    imageLayers[listIndex + 1] = temp;
                }
                UpdateResultImage();
            };

            // Добавление кнопок и миниатюры в общий контейнер
            FlowLayoutPanel pnlButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true
            };
            pnlButtons.Controls.Add(btnUp);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnDown);
            container.Controls.Add(pbThumb);
            container.Controls.Add(lblName);
            container.Controls.Add(lblResolution);
            container.Controls.Add(pnlChannels);
            container.Controls.Add(new Label { Text = "Opacity:", AutoSize = true });
            container.Controls.Add(tbOpacity);
            container.Controls.Add(new Label { Text = "Operation:", AutoSize = true });
            container.Controls.Add(cbOperations);
            container.Controls.Add(pnlButtons);

            // Добавление контейнера на форму
            pnlImages.Controls.Add(container);

            // Обновление PictureBox'а
            UpdateResultImage();
        }


        // Обновление результирующей картинки в PictureBox'е
        private void UpdateResultImage()
        {
            if (imageLayers.Count == 0)
            {
                pbMain.Image = null;
                return;
            }

            // Во всех местах обработка изображения происходит через копии, чтобы изначальный массив не затрагивался и не изменялся
            // В противном случае будет невозможно вернуть исходное состояние картинки

            // Временное рабочее изображение
            using (var tempResult = new Bitmap(pbMain.Width, pbMain.Height))
            //using (var tempResult = new Bitmap(GetMaxImageWidth(imageLayers), GetMaxImageHeight(imageLayers)))
            {
                using (var g = Graphics.FromImage(tempResult))
                {
                    // Заливка фона прозрачным
                    g.Clear(Color.Transparent);

                    // Обработка всех картинок и параметров из листа
                    foreach (var layer in imageLayers)
                    {
                        // Временная копия текущей картики с учетом параметров слоя для обработки
                        using (var layerImage = ProcessLayer(layer))
                        {
                            if (layerImage != null)
                            {
                                // Применение операции с слоем, наложение с учетом выбранной операции на уже имеющийся результат
                                ApplyBlendOperation(g, layerImage, layer.Operation);
                            }
                        }
                    }
                }

                // Обновление состояния PictureBox'а
                UpdateMainImage(tempResult);
            }
        }


        // Наложение выбранных параметров на текущий слой
        private Bitmap ProcessLayer(ImageLayer layer)
        {
            // Копия текущей картинки
            var image = new Bitmap(layer.Image);

            // Применение цветовых каналов к слою
            ApplyChannels(image, layer);

            // Применение прозрачности к слою
            ApplyOpacity(image, layer.Opacity);

            return image;
        }


        // Наложение цветовых каналов на слой
        private void ApplyChannels(Bitmap image, ImageLayer layer)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    var newColor = Color.FromArgb(
                        pixel.A,
                        layer.IsRedChannel ? pixel.R : 0,
                        layer.IsGreenChannel ? pixel.G : 0,
                        layer.IsBlueChannel ? pixel.B : 0
                    );
                    image.SetPixel(x, y, newColor);
                }
            }
        }


        // Наложение прозрачности на слой
        private void ApplyOpacity(Bitmap image, int opacity)
        {
            float alpha = opacity / 100f;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    var newColor = Color.FromArgb(
                        (int)(pixel.A * alpha),
                        pixel.R, pixel.G, pixel.B
                    );
                    image.SetPixel(x, y, newColor);
                }
            }
        }


        // Обновление результатирующей картинки непосредственно в PictureBox'е
        private void UpdateMainImage(Bitmap newImage)
        {
            // Обновление без возможной потери старого изображения
            var oldImage = pbMain.Image;
            pbMain.Image = new Bitmap(newImage);

            // Очистка старого изображения из памяти
            oldImage?.Dispose();
        }


        // Промежуточная функция для осуществления выбранной операции
        private void ApplyBlendOperation(Graphics g, Bitmap layerImage, string operation)
        {
            switch (operation)
            {
                case "Normal":
                    OperationNormal(g, layerImage);
                    break;
                case "Sum":
                    OperationSum(g, layerImage);
                    break;
                case "Sub":
                    OperationSub(g, layerImage);
                    break;
                case "Mult":
                    OperationMult(g, layerImage);
                    break;
                case "Avg":
                    OperationAvg(g, layerImage);
                    break;
                case "Max":
                    OperationMax(g, layerImage);
                    break;
                default:
                    break;
            }
        }


        // Реализация операций над слоями

        // Операция наложения
        private void OperationNormal(Graphics g, Bitmap layer)
        {
            g.DrawImage(layer, Point.Empty);
        }

        // Операция сложения
        /*private void OperationSum(Graphics g, Bitmap layer)
        {
            // Копия имующегося результирующего изображения
            using (var currentResult = new Bitmap(pbMain.Width, pbMain.Height))

            using (var currentLayer = Graphics.FromImage(currentResult))
            {
                // Копия текущеего состояния для изменения пикселей
                currentLayer.DrawImage(pbMain.Image ?? new Bitmap(pbMain.Width, pbMain.Height),
                                     new Rectangle(0, 0, pbMain.Width, pbMain.Height));

                // Обработка пикселей
                for (int y = 0; y < Math.Min(pbMain.Height, layer.Height); y++)
                {
                    for (int x = 0; x < Math.Min(pbMain.Width, layer.Width); x++)
                    {
                        Color resultPixel = currentResult.GetPixel(x, y);
                        Color layerPixel = layer.GetPixel(x, y);

                        // Проверка на прозрачность результата и текущего слоя
                        if (resultPixel.A != 0 && layerPixel.A != 0)
                        {
                            currentResult.SetPixel(x, y, Color.FromArgb(Clamp(resultPixel.A + layerPixel.A, 0, 255), 
                                                                        Clamp(resultPixel.R + layerPixel.R, 0, 255), 
                                                                        Clamp(resultPixel.G + layerPixel.G, 0, 255),
                                                                        Clamp(resultPixel.B + layerPixel.B, 0, 255)));
                        }
                        else if (resultPixel.A == 0)
                        {
                            currentResult.SetPixel(x, y, Color.FromArgb(layerPixel.A, layerPixel.R, layerPixel.G, layerPixel.B));
                        }
                        else
                        {
                            currentResult.SetPixel(x, y, Color.FromArgb(resultPixel.A, resultPixel.R, resultPixel.G, resultPixel.B));
                        }
                    }
                }

                // Изменение текущей результирующей картинки
                g.DrawImage(currentResult, Point.Empty);
            }
        }*/

        private void OperationSum(Graphics g, Bitmap layer)
        {
            using (var currentResult = new Bitmap(pbMain.Width, pbMain.Height))
            {
                using (var currentLayer = Graphics.FromImage(currentResult))
                {
                    currentLayer.DrawImage(pbMain.Image ?? new Bitmap(pbMain.Width, pbMain.Height),
                                         new Rectangle(0, 0, pbMain.Width, pbMain.Height));
                }

                var resultData = currentResult.LockBits(
                    new Rectangle(0, 0, currentResult.Width, currentResult.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                var layerData = layer.LockBits(
                    new Rectangle(0, 0, layer.Width, layer.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                unsafe
                {
                    int width = Math.Min(currentResult.Width, layer.Width);
                    int height = Math.Min(currentResult.Height, layer.Height);

                    byte* resultPtr = (byte*)resultData.Scan0.ToPointer();
                    byte* layerPtr = (byte*)layerData.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        int resultYOffset = y * resultData.Stride;
                        int layerYOffset = y * layerData.Stride;

                        for (int x = 0; x < width; x++)
                        {
                            int resultOffset = resultYOffset + x * 4;
                            int layerOffset = layerYOffset + x * 4;

                            byte resultA = resultPtr[resultOffset + 3];
                            byte resultR = resultPtr[resultOffset + 2];
                            byte resultG = resultPtr[resultOffset + 1];
                            byte resultB = resultPtr[resultOffset];

                            byte layerA = layerPtr[layerOffset + 3];
                            byte layerR = layerPtr[layerOffset + 2];
                            byte layerG = layerPtr[layerOffset + 1];
                            byte layerB = layerPtr[layerOffset];

                            if (resultA != 0 && layerA != 0)
                            {
                                resultPtr[resultOffset + 3] = ClampBytes(resultA + layerA);
                                resultPtr[resultOffset + 2] = ClampBytes(resultR + layerR);
                                resultPtr[resultOffset + 1] = ClampBytes(resultG + layerG);
                                resultPtr[resultOffset] = ClampBytes(resultB + layerB);
                            }
                            else if (resultA == 0)
                            {
                                resultPtr[resultOffset + 3] = layerA;
                                resultPtr[resultOffset + 2] = layerR;
                                resultPtr[resultOffset + 1] = layerG;
                                resultPtr[resultOffset] = layerB;
                            }
                            else
                            {
                                resultPtr[resultOffset + 3] = resultA;
                                resultPtr[resultOffset + 2] = resultR;
                                resultPtr[resultOffset + 1] = resultG;
                                resultPtr[resultOffset] = resultB;
                            }
                        }
                    }
                }

                currentResult.UnlockBits(resultData);
                layer.UnlockBits(layerData);

                g.DrawImage(currentResult, Point.Empty);
            }
        }

        // Операция вычитания
        private void OperationSub(Graphics g, Bitmap layer)
        {
            using (var currentResult = new Bitmap(pbMain.Width, pbMain.Height))
            {
                using (var currentLayer = Graphics.FromImage(currentResult))
                {
                    currentLayer.DrawImage(pbMain.Image ?? new Bitmap(pbMain.Width, pbMain.Height),
                                         new Rectangle(0, 0, pbMain.Width, pbMain.Height));
                }

                var resultData = currentResult.LockBits(
                    new Rectangle(0, 0, currentResult.Width, currentResult.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                var layerData = layer.LockBits(
                    new Rectangle(0, 0, layer.Width, layer.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                unsafe
                {
                    int width = Math.Min(currentResult.Width, layer.Width);
                    int height = Math.Min(currentResult.Height, layer.Height);

                    byte* resultPtr = (byte*)resultData.Scan0.ToPointer();
                    byte* layerPtr = (byte*)layerData.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        int resultYOffset = y * resultData.Stride;
                        int layerYOffset = y * layerData.Stride;

                        for (int x = 0; x < width; x++)
                        {
                            int resultOffset = resultYOffset + x * 4;
                            int layerOffset = layerYOffset + x * 4;

                            byte resultA = resultPtr[resultOffset + 3];
                            byte resultR = resultPtr[resultOffset + 2];
                            byte resultG = resultPtr[resultOffset + 1];
                            byte resultB = resultPtr[resultOffset];

                            byte layerA = layerPtr[layerOffset + 3];
                            byte layerR = layerPtr[layerOffset + 2];
                            byte layerG = layerPtr[layerOffset + 1];
                            byte layerB = layerPtr[layerOffset];

                            if (resultA != 0 && layerA != 0)
                            {
                                resultPtr[resultOffset + 3] = ClampBytes(resultA - layerA);
                                resultPtr[resultOffset + 2] = ClampBytes(resultR - layerR);
                                resultPtr[resultOffset + 1] = ClampBytes(resultG - layerG);
                                resultPtr[resultOffset] = ClampBytes(resultB - layerB);
                            }
                            else if (resultA == 0)
                            {
                                resultPtr[resultOffset + 3] = layerA;
                                resultPtr[resultOffset + 2] = layerR;
                                resultPtr[resultOffset + 1] = layerG;
                                resultPtr[resultOffset] = layerB;
                            }
                            else
                            {
                                resultPtr[resultOffset + 3] = resultA;
                                resultPtr[resultOffset + 2] = resultR;
                                resultPtr[resultOffset + 1] = resultG;
                                resultPtr[resultOffset] = resultB;
                            }
                        }
                    }
                }

                currentResult.UnlockBits(resultData);
                layer.UnlockBits(layerData);

                g.DrawImage(currentResult, Point.Empty);
            }
        }

        // Операция умножения
        private void OperationMult(Graphics g, Bitmap layer)
        {
            using (var currentResult = new Bitmap(pbMain.Width, pbMain.Height))
            {
                using (var currentLayer = Graphics.FromImage(currentResult))
                {
                    currentLayer.DrawImage(pbMain.Image ?? new Bitmap(pbMain.Width, pbMain.Height),
                                         new Rectangle(0, 0, pbMain.Width, pbMain.Height));
                }

                var resultData = currentResult.LockBits(
                    new Rectangle(0, 0, currentResult.Width, currentResult.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                var layerData = layer.LockBits(
                    new Rectangle(0, 0, layer.Width, layer.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                unsafe
                {
                    int width = Math.Min(currentResult.Width, layer.Width);
                    int height = Math.Min(currentResult.Height, layer.Height);

                    byte* resultPtr = (byte*)resultData.Scan0.ToPointer();
                    byte* layerPtr = (byte*)layerData.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        int resultYOffset = y * resultData.Stride;
                        int layerYOffset = y * layerData.Stride;

                        for (int x = 0; x < width; x++)
                        {
                            int resultOffset = resultYOffset + x * 4;
                            int layerOffset = layerYOffset + x * 4;

                            byte resultA = resultPtr[resultOffset + 3];
                            byte resultR = resultPtr[resultOffset + 2];
                            byte resultG = resultPtr[resultOffset + 1];
                            byte resultB = resultPtr[resultOffset];

                            byte layerA = layerPtr[layerOffset + 3];
                            byte layerR = layerPtr[layerOffset + 2];
                            byte layerG = layerPtr[layerOffset + 1];
                            byte layerB = layerPtr[layerOffset];

                            if (resultA != 0 && layerA != 0)
                            {
                                resultPtr[resultOffset + 3] = ClampBytes(resultA * layerA);
                                resultPtr[resultOffset + 2] = ClampBytes(resultR * layerR);
                                resultPtr[resultOffset + 1] = ClampBytes(resultG * layerG);
                                resultPtr[resultOffset] = ClampBytes(resultB * layerB);
                            }
                            else if (resultA == 0)
                            {
                                resultPtr[resultOffset + 3] = layerA;
                                resultPtr[resultOffset + 2] = layerR;
                                resultPtr[resultOffset + 1] = layerG;
                                resultPtr[resultOffset] = layerB;
                            }
                            else
                            {
                                resultPtr[resultOffset + 3] = resultA;
                                resultPtr[resultOffset + 2] = resultR;
                                resultPtr[resultOffset + 1] = resultG;
                                resultPtr[resultOffset] = resultB;
                            }
                        }
                    }
                }

                currentResult.UnlockBits(resultData);
                layer.UnlockBits(layerData);

                g.DrawImage(currentResult, Point.Empty);
            }
        }

        // Операция среднего
        private void OperationAvg(Graphics g, Bitmap layer)
        {
            using (var currentResult = new Bitmap(pbMain.Width, pbMain.Height))
            {
                using (var currentLayer = Graphics.FromImage(currentResult))
                {
                    currentLayer.DrawImage(pbMain.Image ?? new Bitmap(pbMain.Width, pbMain.Height),
                                         new Rectangle(0, 0, pbMain.Width, pbMain.Height));
                }

                var resultData = currentResult.LockBits(
                    new Rectangle(0, 0, currentResult.Width, currentResult.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                var layerData = layer.LockBits(
                    new Rectangle(0, 0, layer.Width, layer.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                unsafe
                {
                    int width = Math.Min(currentResult.Width, layer.Width);
                    int height = Math.Min(currentResult.Height, layer.Height);

                    byte* resultPtr = (byte*)resultData.Scan0.ToPointer();
                    byte* layerPtr = (byte*)layerData.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        int resultYOffset = y * resultData.Stride;
                        int layerYOffset = y * layerData.Stride;

                        for (int x = 0; x < width; x++)
                        {
                            int resultOffset = resultYOffset + x * 4;
                            int layerOffset = layerYOffset + x * 4;

                            byte resultA = resultPtr[resultOffset + 3];
                            byte resultR = resultPtr[resultOffset + 2];
                            byte resultG = resultPtr[resultOffset + 1];
                            byte resultB = resultPtr[resultOffset];

                            byte layerA = layerPtr[layerOffset + 3];
                            byte layerR = layerPtr[layerOffset + 2];
                            byte layerG = layerPtr[layerOffset + 1];
                            byte layerB = layerPtr[layerOffset];

                            if (resultA != 0 && layerA != 0)
                            {
                                resultPtr[resultOffset + 3] = ClampBytes((resultA + layerA) / 2);
                                resultPtr[resultOffset + 2] = ClampBytes((resultR + layerR) / 2);
                                resultPtr[resultOffset + 1] = ClampBytes((resultG + layerG) / 2);
                                resultPtr[resultOffset] = ClampBytes((resultB + layerB) / 2);
                            }
                            else if (resultA == 0)
                            {
                                resultPtr[resultOffset + 3] = layerA;
                                resultPtr[resultOffset + 2] = layerR;
                                resultPtr[resultOffset + 1] = layerG;
                                resultPtr[resultOffset] = layerB;
                            }
                            else
                            {
                                resultPtr[resultOffset + 3] = resultA;
                                resultPtr[resultOffset + 2] = resultR;
                                resultPtr[resultOffset + 1] = resultG;
                                resultPtr[resultOffset] = resultB;
                            }
                        }
                    }
                }

                currentResult.UnlockBits(resultData);
                layer.UnlockBits(layerData);

                g.DrawImage(currentResult, Point.Empty);
            }
        }

        // Операция максимума
        private void OperationMax(Graphics g, Bitmap layer)
        {
            using (var currentResult = new Bitmap(pbMain.Width, pbMain.Height))
            {
                using (var currentLayer = Graphics.FromImage(currentResult))
                {
                    currentLayer.DrawImage(pbMain.Image ?? new Bitmap(pbMain.Width, pbMain.Height),
                                         new Rectangle(0, 0, pbMain.Width, pbMain.Height));
                }

                var resultData = currentResult.LockBits(
                    new Rectangle(0, 0, currentResult.Width, currentResult.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                var layerData = layer.LockBits(
                    new Rectangle(0, 0, layer.Width, layer.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                unsafe
                {
                    int width = Math.Min(currentResult.Width, layer.Width);
                    int height = Math.Min(currentResult.Height, layer.Height);

                    byte* resultPtr = (byte*)resultData.Scan0.ToPointer();
                    byte* layerPtr = (byte*)layerData.Scan0.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        int resultYOffset = y * resultData.Stride;
                        int layerYOffset = y * layerData.Stride;

                        for (int x = 0; x < width; x++)
                        {
                            int resultOffset = resultYOffset + x * 4;
                            int layerOffset = layerYOffset + x * 4;

                            byte resultA = resultPtr[resultOffset + 3];
                            byte resultR = resultPtr[resultOffset + 2];
                            byte resultG = resultPtr[resultOffset + 1];
                            byte resultB = resultPtr[resultOffset];

                            byte layerA = layerPtr[layerOffset + 3];
                            byte layerR = layerPtr[layerOffset + 2];
                            byte layerG = layerPtr[layerOffset + 1];
                            byte layerB = layerPtr[layerOffset];

                            if (resultA != 0 && layerA != 0)
                            {
                                resultPtr[resultOffset + 3] = ClampBytes(Math.Max(resultA, layerA));
                                resultPtr[resultOffset + 2] = ClampBytes(Math.Max(resultR, layerR));
                                resultPtr[resultOffset + 1] = ClampBytes(Math.Max(resultG, layerG));
                                resultPtr[resultOffset] = ClampBytes(Math.Max(resultB, layerB));
                            }
                            else if (resultA == 0)
                            {
                                resultPtr[resultOffset + 3] = layerA;
                                resultPtr[resultOffset + 2] = layerR;
                                resultPtr[resultOffset + 1] = layerG;
                                resultPtr[resultOffset] = layerB;
                            }
                            else
                            {
                                resultPtr[resultOffset + 3] = resultA;
                                resultPtr[resultOffset + 2] = resultR;
                                resultPtr[resultOffset + 1] = resultG;
                                resultPtr[resultOffset] = resultB;
                            }
                        }
                    }
                }

                currentResult.UnlockBits(resultData);
                layer.UnlockBits(layerData);

                g.DrawImage(currentResult, Point.Empty);
            }
        }


        private byte ClampBytes(int value) => (byte)Clamp(value, 0, 255);
        public T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            else if (value.CompareTo(max) > 0) return max;
            else return value;
        }

        private int GetMaxImageHeight(List<ImageLayer> imageLayers)
        {
            return imageLayers
                .Where(layer => layer?.Image != null)
                .Select(layer => layer.Image.Height)
                .DefaultIfEmpty(0)
                .Max();
        }

        private int GetMaxImageWidth(List<ImageLayer> imageLayers)
        {
            return imageLayers
                .Where(layer => layer?.Image != null)
                .Select(layer => layer.Image.Width)
                .DefaultIfEmpty(0)
                .Max();
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuForm menu = new MenuForm();
            menu.Show();

            this.Close();
        }
    }
}
