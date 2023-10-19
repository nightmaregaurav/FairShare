namespace Core.Extensions
{
    public static class MatrixExtensions
    {
        public static double[][] Transpose(this double[][] matrix)
        {
            var rows = matrix.Length;
            var cols = matrix[0].Length;

            var result = new double[cols][];

            for (var i = 0; i < cols; i++)
            {
                result[i] = new double[rows];
                for (var j = 0; j < rows; j++)
                {
                    result[i][j] = matrix[j][i];
                }
            }
            return result;
        }

        public static double[][] Copy(this double[][] matrix)
        {
            var rows = matrix.Length;
            if (rows == 0) return Array.Empty<double[]>();

            var cols = matrix[0].Length;
            var copy = new double[rows][];

            for (var i = 0; i < rows; i++)
            {
                if (matrix[i] == null || matrix[i].Length != cols) throw new ArgumentException("Matrix should have consistent row lengths.");
                copy[i] = new double[cols];
                Array.Copy(matrix[i], copy[i], cols);
            }
            return copy;
        }

    }
}
