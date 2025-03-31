using System;

namespace lab1.Lr2
{
    public class CubicSpline
    {
        private struct SplineTuple
        {
            public double a, b, c, d, x;
        }

        private SplineTuple[] _splines;

        public void BuildSpline(double[] x, double[] y, int n)
        {
            if (x.Length != n || y.Length != n)
                throw new ArgumentException("Количество точек должно совпадать с n.");

            if (n == 2)
            {
                _splines = new SplineTuple[2];
                _splines[0].x = x[0];
                _splines[0].a = y[0];
                _splines[0].c = 0.0;

                _splines[1].x = x[1];
                _splines[1].a = y[1];
                _splines[1].c = 0.0;
                double h = x[1] - x[0];
                _splines[1].b = (y[1] - y[0]) / h;
                _splines[1].d = 0.0;
                return;
            }

            _splines = new SplineTuple[n];
            for (int i = 0; i < n; i++)
            {
                _splines[i].x = x[i];
                _splines[i].a = y[i];
            }

            // Прямой ход метода прогонки
            double[] alpha = new double[n - 1];
            double[] beta = new double[n - 1];
            alpha[0] = beta[0] = 0.0;

            for (int i = 1; i < n - 1; i++)
            {
                double hi = x[i] - x[i - 1];
                double hi1 = x[i + 1] - x[i];
                double A = hi;
                double C = 2.0 * (hi + hi1);
                double B = hi1;
                double F = 6.0 * ((y[i + 1] - y[i]) / hi1 - (y[i] - y[i - 1]) / hi);
                double z = A * alpha[i - 1] + C;
                alpha[i] = -B / z;
                beta[i] = (F - A * beta[i - 1]) / z;
            }

            // Обратный ход
            _splines[n - 1].c = 0.0;
            for (int i = n - 2; i > 0; i--)
                _splines[i].c = alpha[i] * _splines[i + 1].c + beta[i];

            // Расчёт коэффициентов
            for (int i = n - 1; i > 0; i--)
            {
                double hi = x[i] - x[i - 1];
                _splines[i].d = (_splines[i].c - _splines[i - 1].c) / hi;
                _splines[i].b = hi * (2.0 * _splines[i].c + _splines[i - 1].c) / 6.0 + (y[i] - y[i - 1]) / hi;
            }
        }

        public double Interpolate(double x)
        {
            if (_splines == null)
                throw new InvalidOperationException("Сплайн не построен.");

            int n = _splines.Length;
            if (x <= _splines[0].x)
                return _splines[0].a;
            if (x >= _splines[n - 1].x)
                return _splines[n - 1].a;

            // Бинарный поиск
            int low = 0, high = n - 1;
            while (low + 1 < high)
            {
                int mid = (low + high) / 2;
                if (x <= _splines[mid].x)
                    high = mid;
                else
                    low = mid;
            }

            SplineTuple s = _splines[high];
            double dx = x - s.x;
            return s.a + (s.b + (s.c / 2.0 + s.d * dx / 6.0) * dx) * dx;
        }
    }

}
