using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

using Ceres.Utilities;

namespace Ceres.RBF
{
    //
    // code based upon paper G.Turk, J O'Brien "Shape Transformation Using Variational Implicit Functions"
    //
    public class EvaluatorRBF : Evaluator
    {
#region Data
        private float[] _weights = null;
#endregion

        // normal constructor
        public EvaluatorRBF(Point3f[] points, InOut[] inout):
            base(points, inout)
        {
        }

        // testing constructor, with precomputed weights
        public EvaluatorRBF(Point3f[] points, InOut[] inout, float[] weights):
            base(points, inout)
        {
            _weights = weights;
        }

        // requires weights recomputation
        protected override void Invalidate()
        {
            _weights = null;
            base.Invalidate();
        }

        // our RBF function
        protected float BaseFunction(Point3f a, Point3f p)
        {
            /*
             * this is a base function which is recommended in the paper             * 
            float d = Point3f.Distance(a, p);
            return (d == 0.0f) ? 0.0f : d*d*(float)Math.Log(d);
            */

            // we use cubed distance
            return Utils.cubed(Point3f.Distance(a, p));
        }

        // evaluate RBF at a given point,
        // return negative if outside,
        // zero if on the boundary and 
        // positive for an inside point
        public override float Evaluate(Point3f pt)
        {
            if (_weights == null)
                Compute();

            float sum = 0.0f;
            for (int k = 0; k != _len; ++k)
            {
                float t = BaseFunction(pt, _points[k]);
                sum += _weights[k] * t;
            }

            // add linear parts responsible for the shift and constant part
            sum += _weights[_len] + _weights[_len + 1] * pt.X + _weights[_len + 2] * pt.Y + _weights[_len + 3] * pt.Z;

            return sum;
        }

        // compute variational matrix
        protected float[,] ComputeMatrix()
        {
            int n = _len + 4;

            float[,] matrix = new float[n, n];

            // fill out diagonal
            for(int k = 0; k != n; ++k)
            {
                matrix[k, k] = 0.0f;
            }

            // top triangle part
            for (int r = 0; r != _len; ++r) // row loop
            {
                for (int c = r+1; c != _len; ++c) // column loop
                {
                    matrix[r, c] = BaseFunction(_points[r], _points[c]); 
                }
            }

            // column with ones
            for(int r = 0; r != _len; ++r)
            {
                matrix[r, _len] = 1.0f;
            }

            // last block with point coordinates
            for (int r = 0; r != _len; ++r)
            {
                matrix[r, _len + 1] = _points[r].X;
                matrix[r, _len + 2] = _points[r].Y;
                matrix[r, _len + 3] = _points[r].Z;
            }

            // zero filled block at the bottom
            for (int r = _len; r != n; ++r)
            {
                matrix[r, _len + 1] = 0.0f;
                matrix[r, _len + 2] = 0.0f;
                matrix[r, _len + 3] = 0.0f;
            }

            // make symmetric matrix
            // NB: might not be necessary if we use decomposition method
            // which relies on matrix symmetry
            for (int r = 0; r != n; ++r) // row loop
            {
                for (int c = r + 1; c != n; ++c) // column loop
                {
                    matrix[c, r] = matrix[r, c];
                }
            }

            return matrix;
        }

        // compute Right Hand Side
        protected float[] ComputeRHS()
        {
            int n = _len + 4;
            float[] rhs = new float[n];

            // fill most parts with inout array
            for(int k = 0; k != _len; ++k)
            {
                rhs[k] = (float)_inout[k];
            }

            // last 4 with zeroes
            for (int k = _len; k != n; ++k)
            {
                rhs[k] = 0.0f;
            }
            return rhs;
        }

        // altogether, compute matrix, RHS and find vector of weights
        protected override void Compute()
        {
            float[,] matrix = ComputeMatrix();
            float[] RHS     = ComputeRHS();

            var mtx = DenseMatrix.OfArray(matrix);
            var vec = new DenseVector(RHS);

            // We better use something which
            // takes into account symmetry of the matrix.
            // Unfortunately, it is not positively-defined matrix,
            // but positive-semidefinite, thus there are e.v. zeros,
            // and Cholesky() does not work
            // Symmetric LU shall be used, which has half of operations of
            // full LU, but it is not available
            var res = mtx.LU().Solve(vec);

            _weights = res.ToArray();
        }

        public float[] weights
        {
            get { return _weights; }
        }
    }
}
