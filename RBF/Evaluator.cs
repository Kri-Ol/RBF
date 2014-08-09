using System;
using System.Diagnostics;

using Ceres.Utilities;

namespace Ceres.RBF
{
    public abstract class Evaluator
    {
        public enum InOut // move it to Utilities?
        {
            OUT = -1,
            BND =  0,
            IN  = +1
        }

#region Data
        protected BoundingBox       _bbox;
        protected Point3f[]         _points = null;
        protected InOut[]           _inout  = null;
        protected int               _len    = -1;
#endregion

        public Evaluator(Point3f[] points, InOut[] inout)
        {
            _points = points;
            _len    = _points.Length;
            Debug.Assert(_len > 3); // should also check for non-coplanarity

            _inout = inout;
            Debug.Assert(_len <= _inout.Length);

            _bbox = ComputeBBox();
        }

        protected BoundingBox ComputeBBox()
        {
            float minX = Single.MaxValue;
            float minY = Single.MaxValue;
            float minZ = Single.MaxValue;
            float maxX = Single.MinValue;
            float maxY = Single.MinValue;
            float maxZ = Single.MinValue;

            for (int k = 0; k != _len; ++k)
            {
                float x = _points[k].X;
                float y = _points[k].Y;
                float z = _points[k].Z;

                if (minX > x)
                    minX = x;
                if (maxX < x)
                    maxX = x;

                if (minY > y)
                    minY = y;
                if (maxY < y)
                    maxY = y;

                if (minZ > z)
                    minZ = z;
                if (maxZ < z)
                    maxZ = z;
            }
            return new BoundingBox(new Point3f(minX, minY, minZ), new Point3f(maxX, maxY, maxZ));
        }

        public abstract float Evaluate(Point3f pt);

        protected abstract void Compute();

        protected virtual void Invalidate()
        {
            _bbox.Clear();
            _points = null;
            _inout  = null;
            _len    = -1;
        }
    }
}
