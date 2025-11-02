using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiThreading.Task3.MatrixMultiplier.Matrices;
using MultiThreading.Task3.MatrixMultiplier.Multipliers;

namespace MultiThreading.Task3.MatrixMultiplier.Tests
{
    [TestClass]
    public class MultiplierTest
    {
        [TestMethod]
        public void MultiplyMatrix3On3Test()
        {
            TestMatrix3On3(new MatricesMultiplier());
            TestMatrix3On3(new MatricesMultiplierParallel());
        }

        [TestMethod]
        public void ParallelEfficiencyTest()
        {
            int[] sizes = { 1, 5, 10, 20, 30, 50 };

            double thresholdRatio = 0.9;

            foreach (var size in sizes)
            {
                var mA = InitializeMatrix(size, size);
                var mB = InitializeMatrix(size, size);

                IMatrix seqResult;
                IMatrix parResult;

                var seqDuration = MeasureMicroseconds(
                    () => MultiplyMatrices(mA, mB, new MatricesMultiplier(), out seqResult)
                );
                
                var parDuration = MeasureMicroseconds(
                    () => MultiplyMatrices(mA, mB, new MatricesMultiplierParallel(), out parResult)
                );

                Console.WriteLine($"Size: {size}, Seq: {seqDuration} microseconds, Par: {parDuration} microseconds");

                if (parDuration < seqDuration * thresholdRatio)
                {
                    Console.WriteLine($"Parallel version became faster at size {size}");
                    return;
                }
            }

            Assert.Fail("Parallel version was not faster for any tested size");
        }

        #region private methods

        private static double MeasureMicroseconds(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.ElapsedTicks * 1_000_000.0 / Stopwatch.Frequency;
        }

        void MultiplyMatrices(IMatrix a, IMatrix b, IMatricesMultiplier multiplier, out IMatrix result)
        {
            result = multiplier.Multiply(a, b);
        }

        void TestMatrix3On3(IMatricesMultiplier matrixMultiplier)
        {
            if (matrixMultiplier == null)
            {
                throw new ArgumentNullException(nameof(matrixMultiplier));
            }

            var m1 = new Matrix(3, 3);
            m1.SetElement(0, 0, 34);
            m1.SetElement(0, 1, 2);
            m1.SetElement(0, 2, 6);

            m1.SetElement(1, 0, 5);
            m1.SetElement(1, 1, 4);
            m1.SetElement(1, 2, 54);

            m1.SetElement(2, 0, 2);
            m1.SetElement(2, 1, 9);
            m1.SetElement(2, 2, 8);

            var m2 = new Matrix(3, 3);
            m2.SetElement(0, 0, 12);
            m2.SetElement(0, 1, 52);
            m2.SetElement(0, 2, 85);

            m2.SetElement(1, 0, 5);
            m2.SetElement(1, 1, 5);
            m2.SetElement(1, 2, 54);

            m2.SetElement(2, 0, 5);
            m2.SetElement(2, 1, 8);
            m2.SetElement(2, 2, 9);

            var multiplied = matrixMultiplier.Multiply(m1, m2);
            Assert.AreEqual(448, multiplied.GetElement(0, 0));
            Assert.AreEqual(1826, multiplied.GetElement(0, 1));
            Assert.AreEqual(3052, multiplied.GetElement(0, 2));

            Assert.AreEqual(350, multiplied.GetElement(1, 0));
            Assert.AreEqual(712, multiplied.GetElement(1, 1));
            Assert.AreEqual(1127, multiplied.GetElement(1, 2));

            Assert.AreEqual(109, multiplied.GetElement(2, 0));
            Assert.AreEqual(213, multiplied.GetElement(2, 1));
            Assert.AreEqual(728, multiplied.GetElement(2, 2));
        }


        Matrix InitializeMatrix(int rows, int cols)
        {
            var matrix = new Matrix(rows, cols);

            Random r = new Random();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix.SetElement(i, j, r.Next(100));
                }
            }
            return matrix;
        }
        #endregion
    }
}
