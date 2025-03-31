using System.Linq;
using System;

namespace lab1.Lr2
{
    public class linearInterpolation : IInterpolation
    {
        // Массивы для хранения отсортированных координат
        private double[] x = new double[0];
        private double[] y = new double[0];

        // Рассчитывает и сортирует точки для линейной интерполяции
        public void calc(MyPoint[] points)
        {
            int n = points.Length;
            MyPoint[] sorted = new MyPoint[n];
            Array.Copy(points, sorted, n);
            Array.Sort(sorted, (a, b) => a.X.CompareTo(b.X));
            
            x = new double[n];
            y = new double[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = sorted[i].X;
                y[i] = sorted[i].Y;
            }
        }

        // Вычисляет интерполированное значение для заданного _x, используя бинарный поиск.
        public double f(double _x)
        {
            if (x.Length < 2) return _x;
            int n = x.Length;
            if (_x <= x[0]) return y[0];
            if (_x >= x[n - 1]) return y[n - 1];

            int low = 0, high = n - 1;
            while (high - low > 1)
            {
                int mid = low + ((high - low) >> 1);
                if (x[mid] > _x)
                    high = mid;
                else
                    low = mid;
            }
            double t = (_x - x[low]) / (x[high] - x[low]);
            return y[low] * (1 - t) + y[high] * t;
        }

        // Возвращает новый экземпляр linearInterpolation.
        public IInterpolation copy()
        {
            return new linearInterpolation();
        }
    }
}
