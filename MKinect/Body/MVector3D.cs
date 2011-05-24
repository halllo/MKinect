using System;

namespace MKinect.Body
{
    public class MVector2D
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        internal MVector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static MVector2D Projection(double x, double y)
        {
            return new MVector2D(x, y);
        }
    }

    public class MVector3D
    {
        internal MVector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public static MVector3D operator -(MVector3D first, MVector3D second)
        {
            return new MVector3D(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        public static double Distance(MVector3D first, MVector3D second)
        {
            return Math.Sqrt(
                Square(first.X - second.X) +
                Square(first.Y - second.Y) +
                Square(first.Z - second.Z));
        }

        public static double Distance2D(MVector3D first, MVector3D second)
        {
            return Math.Sqrt(
                Square(first.X - second.X) +
                Square(first.Y - second.Y));
        }

        public static double Distance(MVector3D first, MVector3D second, Func<MVector3D, double> selector)
        {
            return Math.Abs(selector(first) - selector(second));
        }

        private static double Square(double v)
        {
            return v * v;
        }
    }
}
