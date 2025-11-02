using MultiThreading.Task3.MatrixMultiplier.Matrices;
using System.Threading.Tasks;

namespace MultiThreading.Task3.MatrixMultiplier.Multipliers
{
    public class MatricesMultiplierParallel : IMatricesMultiplier
    {
        public IMatrix Multiply(IMatrix m1, IMatrix m2)
        {
            var resultMatrix = new Matrix(m1.RowCount, m2.ColCount);

            var m1RowCount = m1.RowCount;
            var m2ColCount = m2.ColCount;
            var m1ColCount = m1.ColCount;

            Parallel.For(0, m2ColCount, (i) => {
                for (byte j = 0; j < m2ColCount; j++)
                {
                    long sum = 0;

                    for (byte k = 0; k < m1ColCount; k++)
                    {
                        sum += m1.GetElement(i, k) * m2.GetElement(k, j);
                    }

                    resultMatrix.SetElement(i, j, sum);
                }
            });

            return resultMatrix;
        }
    }
}
