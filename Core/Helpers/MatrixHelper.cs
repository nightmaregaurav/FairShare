namespace Core.Helpers
{
    public static class MatrixHelper
    {
        public static void Print(this IEnumerable<double[]> matrix)
        {
            foreach (var row in matrix)
            {
                foreach (var element in row) System.Console.Write(element.ToString("0.00").PadLeft(8) + "\t");
                System.Console.WriteLine();
            }
        }

        public static void Print(this IEnumerable<double> matrix)
        {
            foreach (var element in matrix)
            {
                System.Console.Write(element.ToString("0.00").PadLeft(8) + "\t");
            }
            System.Console.WriteLine();
        }

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

        public static double[][] Add(this double[][] matrix1, double[][] matrix2)
        {
            if (matrix1.Length != matrix2.Length || matrix1[0].Length != matrix2[0].Length)
            {
                throw new ArgumentException("Matrices must have the same dimensions for addition.");
            }

            var rows = matrix1.Length;
            var columns = matrix1[0].Length;
            var result = new double[rows][];

            for (var i = 0; i < rows; i++)
            {
                result[i] = new double[columns];
                for (var j = 0; j < columns; j++)
                {
                    result[i][j] = matrix1[i][j] + matrix2[i][j];
                }
            }

            return result;
        }

        public static double[][] Subtract(this double[][] matrix1, double[][] matrix2)
        {
            if (matrix1.Length != matrix2.Length || matrix1[0].Length != matrix2[0].Length)
            {
                throw new ArgumentException("Matrices must have the same dimensions for subtraction.");
            }

            var rows = matrix1.Length;
            var columns = matrix1[0].Length;
            var result = new double[rows][];

            for (var i = 0; i < rows; i++)
            {
                result[i] = new double[columns];
                for (var j = 0; j < columns; j++)
                {
                    result[i][j] = matrix1[i][j] - matrix2[i][j];
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

        public static double[][] Max(this double[][] matrix1, double[][] matrix2)
        {
            if (matrix1.Length != matrix2.Length || matrix1[0].Length != matrix2[0].Length)
            {
                throw new ArgumentException("Matrices must have the same dimensions for finding the maximum values.");
            }

            var rows = matrix1.Length;
            var columns = matrix1[0].Length;
            var result = new double[rows][];

            for (var i = 0; i < rows; i++)
            {
                result[i] = new double[columns];
                for (var j = 0; j < columns; j++)
                {
                    result[i][j] = Math.Max(matrix1[i][j], matrix2[i][j]);
                }
            }

            return result;
        }

        public static double[][] Min(this double[][] matrix1, double[][] matrix2)
        {
            if (matrix1.Length != matrix2.Length || matrix1[0].Length != matrix2[0].Length)
            {
                throw new ArgumentException("Matrices must have the same dimensions for finding the minimum values.");
            }

            var rows = matrix1.Length;
            var columns = matrix1[0].Length;
            var result = new double[rows][];

            for (var i = 0; i < rows; i++)
            {
                result[i] = new double[columns];
                for (var j = 0; j < columns; j++)
                {
                    result[i][j] = Math.Min(matrix1[i][j], matrix2[i][j]);
                }
            }

            return result;
        }

        public static double[][] CreateFlatMatrix(this double[][] reference, double value)
        {
            return CreateFlatMatrix(reference.Length, reference[0].Length, value);
        }

        public static double[][] CreateFlatMatrix(int rows, int columns, double value)
        {
            if (rows <= 0 || columns <= 0)
            {
                throw new ArgumentException("Matrix dimensions must be positive.");
            }

            var result = new double[rows][];
            for (var i = 0; i < rows; i++)
            {
                result[i] = new double[columns];
                for (var j = 0; j < columns; j++)
                {
                    result[i][j] = value;
                }
            }
            return result;
        }
    }
}
