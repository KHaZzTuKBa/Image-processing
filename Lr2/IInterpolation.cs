namespace lab1.Lr2
{
    public interface IInterpolation
    {
        double f(double x);
        void calc(MyPoint[] points);
        IInterpolation copy();
    }
}
