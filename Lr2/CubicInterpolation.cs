using System.Linq;
using System;

namespace lab1.Lr2
{
    // Кубическая интерполяция

    public class CubicInterpolation : IInterpolation
    {
        private CubicSpline spline = new CubicSpline();

        // Выполняет интерполяцию, используя построенный сплайн.
        public double f(double x)
        {
            return Clamp(spline.Interpolate(x), 0, 255);
        }

        // Сортирует точки по X, формирует массивы x и y, и строит сплайн.
        public void calc(MyPoint[] points)
        {
            int n = points.Length;

            MyPoint[] sortedPoints = new MyPoint[n];
            Array.Copy(points, sortedPoints, n);
            Array.Sort(sortedPoints, (a, b) => a.X.CompareTo(b.X));

            double[] x = new double[n];
            double[] y = new double[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = sortedPoints[i].X;
                y[i] = sortedPoints[i].Y;
            }
            
            spline.BuildSpline(x, y, n);
        }


        // Возвращает новый экземпляр CubicInterpolation.
        public IInterpolation copy()
        {
            return new CubicInterpolation();
        }

        // Ограничивает значение val в пределах от min до max.
        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
