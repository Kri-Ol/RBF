using System;

namespace Ceres.Utilities
{
    //
    // Summary:
    //     Represents an x-, y-, and z-coordinate point in 3-D sace, floating point 
    //
    public struct Point3f
    {
        internal float _x; 
        internal float _y;
        internal float _z;

        public Point3f(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Point3f(Point3f other)
        {
            _x = other._x;
            _y = other._y;
            _z = other._z;
        }

        public float X { get { return _x; } set { _x = value; } }

        public float Y { get { return _y; } set { _y = value; } }

        public float Z { get { return _z; } set { _z = value; } }

#region Helpers
        public static Point3f operator -(Point3f a, Point3f b)
        {
            return new Point3f(a._x - b._x, a._y - b._y, a._z - b._z);
        }

        public static Point3f operator +(Point3f a, Point3f b)
        {
            return new Point3f(a._x + b._x, a._y + b._y, a._z + b._z);
        }

        public static bool operator ==(Point3f a, Point3f b)
        {
            return (a._z == b._z) && (a._x == b._x) && (a._y == b._y);
        }

        public static bool operator !=(Point3f a, Point3f b)
        {
            return !(a == b);
        }

        public static bool operator >(Point3f a, Point3f b)
        {
            return (a._z > b._z) && (a._x > b._x) && (a._y > b._y);
        }

        public static bool operator <(Point3f a, Point3f b)
        {
            return (a._z < b._z) && (a._x < b._x) && (a._y < b._y);
        }

        public static bool Equals(Point3f a, Point3f b)
        {
            return a.X.Equals(b.X) &&
                   a.Y.Equals(b.Y) &&
                   a.Z.Equals(b.Z);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Point3f))
            {
                return false;
            }

            return Point3f.Equals(this, (Point3f)o);
        }

        public bool Equals(Point3f value)
        {
            return Point3f.Equals(this, value);
        }

        public override int GetHashCode()
        {
            // field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode() ^
                   Z.GetHashCode();
        }

        public static float Distance2(Point3f a, Point3f b)
        {
            return (Utils.squared(a._x - b._x) + Utils.squared(a._y - b._y) + Utils.squared(a._z - b._z));
        }

        public static float Distance(Point3f a, Point3f b)
        {
            return (float)Math.Sqrt(Distance2(a, b));
        }

        public static float Norm2(Point3f a)
        {
            return (Utils.squared(a._x) + Utils.squared(a._y) + Utils.squared(a._z));
        }

        public static float Norm(Point3f a)
        {
            return (float)Math.Sqrt(Norm2(a));
        }
#endregion
    }
}
