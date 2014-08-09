using System;
using System.Diagnostics;

namespace Ceres.Utilities
{
    public struct BoundingBox
    {
        internal Point3f _min;
        internal Point3f _max;

        public BoundingBox(Point3f min, Point3f max)
        {
            _min = min;
            _max = max;
            Debug.Assert(Invariant());
        }

        public BoundingBox(float minx, float miny, float minz,
                           float maxx, float maxy, float maxz)
        {
            _min = new Point3f(minx, miny, minz);
            _max = new Point3f(maxx, maxy, maxz);
            Debug.Assert(Invariant());
        }

        public Point3f Max { get { return _max; } set { _max = value; } }
        public Point3f Min { get { return _min; } set { _min = value; } }

        public bool Invariant()
        {
            return _max > _min;
        }

        public void Clear()
        {
            _min = new Point3f(Single.MaxValue, Single.MaxValue, Single.MaxValue);
            _max = new Point3f(Single.MinValue, Single.MinValue, Single.MinValue);
        }

        public static bool operator ==(BoundingBox a, BoundingBox b)
        {
            return (a._min == b._min) && (a._max == b._max);
        }

        public static bool operator !=(BoundingBox a, BoundingBox b)
        {
            return !(a == b);
        }

        public static bool Equals(BoundingBox a, BoundingBox b)
        {
            return a.Min.Equals(b.Min) &&
                   a.Max.Equals(b.Max);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is BoundingBox))
            {
                return false;
            }

            return BoundingBox.Equals(this, (BoundingBox)o);
        }

        public bool Equals(BoundingBox value)
        {
            return BoundingBox.Equals(this, value);
        }

        public override int GetHashCode()
        {
            // field-by-field XOR of HashCodes
            return _min.GetHashCode() ^
                   _max.GetHashCode();
        }
    }
}
