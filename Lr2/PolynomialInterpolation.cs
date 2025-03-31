using System;
using System.Linq;

namespace lab1.Lr2
{

    //  интерполяцию с использованием интерполяционного полинома Лагранжа.
    public class PolynomialInterpolation : IInterpolation
    {
        private double[] _xPoints;
        private double[] _yPoints;
        private int _n;

        // Сортирует точки по X, сохраняет координаты в массивах.
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

        // Вычисляет интерполированное значение по полиному Лагранжа.
        public double f(double _x)
        {
            if (_n == 0) return _x;
            if (_x <= _xPoints[0]) return _yPoints[0];
            if (_x >= _xPoints[_n - 1]) return _yPoints[_n - 1];

            double result = 0.0;
            // Вычисление интерполяционного полинома Лагранжа
            for (int i = 0; i < _n; i++)
            {
                double term = _yPoints[i];
                for (int j = 0; j < _n; j++)
                {
                    if (j != i)
                    {
                        term *= (_x - _xPoints[j]) / (_xPoints[i] - _xPoints[j]);
                    }
                }
                result += term;
            }
            return result;
        }

        // Возвращает новый экземпляр PolynomialInterpolation.
        public IInterpolation copy()
        {
            return new PolynomialInterpolation();
        }
    }
} 