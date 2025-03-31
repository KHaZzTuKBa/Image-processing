using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1.Lr2
{
    public class CurveControl : Panel
    {
        public List<PointF> _points = new List<PointF>();
        private int _selectedIndex = -1;
        private const int PointRadius = 5;

        public event Action CurveChanged;

        public IInterpolation Interpolator { get; set; }

        public CurveControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            _points.Add(new PointF(0, 0));
            _points.Add(new PointF(255, 255));

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
        }

        // Отрисовка элементов
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawGrid(e.Graphics);
            DrawCurve(e.Graphics);
            DrawPoints(e.Graphics);
        }

        private void DrawGrid(Graphics g)
        {
            using (Pen pen = new Pen(Color.LightGray))
            {
                for (int i = 0; i <= 10; i++)
                {
                    float x = Width * i / 10f;
                    float y = Height * i / 10f;
                    g.DrawLine(pen, x, 0, x, Height);
                    g.DrawLine(pen, 0, y, Width, y);
                }
            }
        }

        private void DrawCurve(Graphics g)
        {
            if (_points.Count < 2)  return;
            
            // Если задан интерполятор, используем его для отрисовки гладкого сплайна
            if (Interpolator != null)
            {
                // Для корректной интерполяции инвертирую Y координаты, чтобы 0 была внизу, как требуется для вычислений
                var myPoints = _points.Select(p => new MyPoint(p.X, 255 - p.Y)).OrderBy(p => p.X).ToArray();
                Interpolator.calc(myPoints);
                
                // Будем отрисовывать кривую, пробегая x от 0 до 255 с шагом, зависящим от ширины панели (пиксель на шаг)
                int steps = this.Width; // число шагов = ширина панели
                PointF? prevPoint = null;
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    PointF[] curvePoints = new PointF[steps + 1];
                    for (int i = 0; i <= steps; i++)
                    {
                        double logicalX = 255.0 * i / steps;
                        double interpY = Interpolator.f(logicalX);
                        // Clamp значение interpY в диапазоне [0,255]
                        interpY = Math.Min(255, Math.Max(0, interpY));
                        // Преобразуем: так как TranslatePoint инвертирует Y, используем 255 - interpY
                        PointF logicalPoint = new PointF((float)logicalX, (float)(255 - interpY));
                        curvePoints[i] = TranslatePoint(logicalPoint);
                    }
                    g.DrawLines(pen, curvePoints);
                }
            }
            else
            {
                // Если интерполятор не задан, рисуем линейное соединение контрольных точек
                var sortedPoints = _points.OrderBy(p => p.X).ToList();
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    PointF[] linePoints = sortedPoints.Select(p => TranslatePoint(p)).ToArray();
                    g.DrawLines(pen, linePoints);
                }
            }
        }

        private void DrawPoints(Graphics g)
        {
            foreach (var point in _points)
            {
                PointF screenPoint = TranslatePoint(point);
                g.FillEllipse(Brushes.Red, screenPoint.X - PointRadius, screenPoint.Y - PointRadius, PointRadius * 2, PointRadius * 2);
            }
        }

        // Преобразование координат
        private PointF TranslatePoint(PointF point)
        {
            return new PointF(
                point.X * Width / 255f,
                Height - (point.Y * Height / 255f) // Инвертирование оси Y
            );
        }

        // Обработчики событий мыши
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                var screenPoint = TranslatePoint(_points[i]);
                if (Distance(e.Location, screenPoint) < PointRadius * 2)
                {
                    // Если точка первая (0,0) или последняя (255,255) — запрещаем выбор
                    if (i == 0 || i == _points.Count - 1)
                    {
                        _selectedIndex = -1;
                        return;
                    }
                    _selectedIndex = i;
                    break;
                }
            }

            // Добавление новых точек (запрещено рядом с краями)
            if (_selectedIndex == -1 && e.Button == MouseButtons.Left)
            {
                var newPoint = TranslateToLogicPoint(e.Location);

                // Запрет добавления точек слишком близко к краям
                if (newPoint.X < 10 || newPoint.X > 245)
                {
                    return;
                }

                _points.Add(newPoint);
                _points = _points.OrderBy(p => p.X).ToList();
                _selectedIndex = _points.IndexOf(newPoint);
            }

            Invalidate();
            CurveChanged?.Invoke();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

            if(e.Button == MouseButtons.Left)
            {
                if (_selectedIndex == -1) return;

                // Если выбрана первая (0,0) или последняя (255,255) точка — игнорируем
                if (_selectedIndex == 0 || _selectedIndex == _points.Count - 1)
                {
                    _selectedIndex = -1;
                    return;
                }

                var newPoint = TranslateToLogicPoint(e.Location);
                newPoint.X = Clamp(newPoint.X, 0, 255);
                newPoint.Y = Clamp(newPoint.Y, 0, 255);

                _points[_selectedIndex] = newPoint;
                Invalidate();
                CurveChanged?.Invoke();
            }
            if (e.Button == MouseButtons.Right)
            {
                // Ищем точку под курсором
                for (int i = 0; i < _points.Count; i++)
                {
                    var screenPoint = TranslatePoint(_points[i]);
                    if (Distance(e.Location, screenPoint) < PointRadius * 2)
                    {
                        // Запрещаем удаление крайних точек
                        if (i == 0 || i == _points.Count - 1)
                        {
                            return;
                        }

                        // Удаляем точку и обновляем
                        _points.RemoveAt(i);
                        _points = _points.OrderBy(p => p.X).ToList();
                        Invalidate();
                        CurveChanged?.Invoke();
                        return;
                    }
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _selectedIndex = -1;
        }

        private PointF TranslateToLogicPoint(Point screenPoint)
        {
            return new PointF(
                screenPoint.X * 255f / Width,
                (Height - screenPoint.Y) * 255f / Height // Инвертирование оси Y
            );
        }

        private float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
