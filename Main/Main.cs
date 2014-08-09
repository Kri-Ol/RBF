using System;
using System.IO;
using System.Globalization;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

using Ceres.Utilities;

namespace Ceres.RBF
{
    class Program
    {
        static Tuple<Point3f[], float[]> ReadFile(string filename)
        {
            string line;
            StreamReader file = new StreamReader(filename);

            line = file.ReadLine();
            int n = Convert.ToInt32(line);

            Point3f[] points = new Point3f[n];
            for (int k = 0; k != n; ++k)
            {
                line = file.ReadLine();
                string[] lines = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                float x = Convert.ToSingle(lines[0]);
                float y = Convert.ToSingle(lines[1]);
                float z = Convert.ToSingle(lines[2]);
                points[k] = new Point3f(x, y, z);
            }

            float[] weights = new float[n + 4];
            for (int k = 0; k != n + 4; ++k)
            {
                line = file.ReadLine();

                weights[k] = Convert.ToSingle(line);
            }

            file.Close();

            return new Tuple<Point3f[], float[]>(points, weights);
        }

        // check which solver is good for the purpose
        public static void TestNumerics()
        {
            // simple test
            Matrix<float> A = DenseMatrix.OfArray(new float[,] {{1f,1f,1f,1f}, {1f,2f,3f,4f}, {4f,3f,2f,1f}});
            Vector<float>[] nullspace = A.Kernel();

            // verify: the following should be approximately (0,0,0)
            var q = (A * (2 * nullspace[0] - 3 * nullspace[1]));

            // now test solvers
            var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            formatProvider.TextInfo.ListSeparator = " ";

            var matrixA = DenseMatrix.OfArray(new[,] { { 5.00f, 2.00f, -4.00f }, { 3.00f, -7.00f, 6.00f }, { 4.00f, 1.00f, 5.00f } });
            Console.WriteLine(@"Matrix 'A' with coefficients");
            Console.WriteLine(matrixA.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            // Create vector "b" with the constant terms.
            var vectorB = new DenseVector(new[] { -7.0f, 38.0f, 43.0f });
            Console.WriteLine(@"Vector 'b' with the constant terms");
            Console.WriteLine(vectorB.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            // 1. Solve linear equations using LU decomposition
            var resultX = matrixA.LU().Solve(vectorB);
            Console.WriteLine(@"1. Solution using LU decomposition");
            Console.WriteLine(resultX.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            var newMatrixA = matrixA.TransposeAndMultiply(matrixA);
            Console.WriteLine(@"Symmetric positive definite matrix");
            Console.WriteLine(newMatrixA.ToString("#0.00\t", formatProvider));
            Console.WriteLine();

            // 6. Solve linear equations using Cholesky decomposition
            resultX = newMatrixA.Cholesky().Solve(vectorB);
            Console.WriteLine(@"6. Solution using Cholesky decomposition");
            Console.WriteLine(resultX.ToString("#0.00\t", formatProvider));
            Console.WriteLine();
        }

        static void TestSphere(string fname)
        {
            // this is simple sphere test, 6 points at boundaries and one in the center
            Tuple<Point3f[], float[]> tu = ReadFile(fname);

            Point3f[] points  = tu.Item1;
            float[]   weights = tu.Item2;

            Evaluator.InOut[] inout = new Evaluator.InOut[points.Length];

            // for this file, only point in center is inside
            for (int k = 0; k != inout.Length; ++k)
            {
                inout[k] = Evaluator.InOut.BND;
                if (points[k].X == 0.0f && points[k].Y == 0.0f && points[k].Z == 0.0f)
                {
                    inout[k] = Evaluator.InOut.IN;
                }
            }

            EvaluatorRBF eval = new EvaluatorRBF(points, inout, null /*weights*/);
            eval.Evaluate(points[0]);

            float[] new_weights = eval.weights;

            Console.WriteLine("Stored weights vs Computed weights");
            for (int k = 0; k != new_weights.Length; ++k)
            {
                Console.WriteLine(String.Format("Stored weight: {0}   Computed weight: {1}", weights[k], new_weights[k]));
            }
            Console.WriteLine("");

            // self-test
            // each original point shall be classified properly
            Console.WriteLine("Original points classification");
            for (int k = 0; k != points.Length; ++k)             
            {
                float d = eval.Evaluate(points[k]);
                Console.WriteLine(String.Format("Original class: {0}  Computed class: {1}", (float)inout[k], d));
            }
            Console.WriteLine("");

            for (int iz = -6; iz != 7; ++iz)
            {
                float z = (float)iz * 1.0f;
                for (int r = -6; r != 7; ++r)
                {
                    float y = (float)r * 1.0f;
                    for (int c = -6; c != 7; ++c)
                    {
                        float x = (float)c * 1.0f;

                        float d = eval.Evaluate(new Point3f(x, y, z));

                        Console.Write(d);
                        Console.Write("  ");
                    }
                    Console.WriteLine("");
                }
            }
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            TestNumerics();
            TestSphere("../../../SPHERE");
        }
    }
}
