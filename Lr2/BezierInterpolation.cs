using System;
using System.Linq;

namespace lab1.Lr2
{
    // Интерполяция кривой Безье с использованием формулы Бернштейна.

    public class BezierInterpolation : IInterpolation
    {
        private double[] _xPoints;
        private double[] _yPoints;
        private int _n;

        /// Сортирует точки по X и сохраняет координаты в массивах.
        public void calc(MyPoint[] points)
        {
            var sorted = points.OrderBy(p => p.X).ToArray();
            _n = sorted.Length;
            _xPoints = new double[_n];
            _yPoints = new double[_n];
            for (int i = 0; i < _n; i++)
            {
                _xPoints[i] = sorted[i].X;
                _yPoints[i] = sorted[i].Y;
            }
        }

        // Вычисляет интерполированное значение для _x по полиному Безье.
        public double f(double _x)
        {
            if (_n == 0) return _x;
            if (_x <= _xPoints[0]) return _yPoints[0];
            if (_x >= _xPoints[_n - 1]) return _yPoints[_n - 1];
            
            // Нормализация _x в параметр t от 0 до 1
            double t = (_x - _xPoints[0]) / (_xPoints[_n - 1] - _xPoints[0]);
            double result = 0.0;
            int degree = _n - 1;
            double oneMinusT = 1.0 - t;
            double B = Math.Pow(oneMinusT, degree); // Начальное значение коэффициента Бернштейна
            result += B * _yPoints[0];
            for (int i = 1; i < _n; i++)
            {
                // Рекуррентное вычисление коэффициента Бернштейна
                B = B * ((double)(degree - i + 1) / i) * (t / oneMinusT);
                result += B * _yPoints[i];
            }
            return result;
        }

        // Возвращает новый экземпляр BezierInterpolation.
        public IInterpolation copy()
        {
            return new BezierInterpolation();
        }
    }
} 