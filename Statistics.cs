using System;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI
{
    internal static class Statistics
    {
        #region Variables
        private static int AmountOfData;
        #endregion



        static Statistics()
        {
            AmountOfData = 0;
        }



        #region Root
        internal static float NthRoot(float x, float y)
        {
            return Convert.ToSingle(Math.Pow(x, 1.0f / y));
        }

        internal static async Task<float> NthRootAsync(float x, float y)
        {
            return await Task.Run<float>(function: () => NthRoot(x, y));
        }

        internal static double NthRoot(double x, double y)
        {
            return Math.Pow(x, 1.0 / y);
        }

        internal static async Task<double> NthRootAsync(double x, double y)
        {
            return await Task.Run<double>(function: () => NthRoot(x, y));
        }
        #endregion



        #region StandardDeviation
        internal static float StandardDeviation(float[] Values)
        {
            return Convert.ToSingle(value: Math.Sqrt(d: Variance(Values: Values)));
        }

        internal static async Task<float> StandardDeviationAsync(float[] Values)
        {
            return await Task.Run<float>(function: () => StandardDeviation(Values: Values));
        }

        internal static double StandardDeviation(double[] Values)
        {
            return Math.Sqrt(d: Variance(Values: Values));
        }

        internal static async Task<double> StandardDeviationAsync(double[] Values)
        {
            return await Task.Run<double>(function: () => StandardDeviation(Values: Values));
        }
        #endregion



        #region Variance
        internal static float Variance(float[] Values)
        {
            var average = Values.Average();
            var sumOfSquares = Values.Select(selector: value => Math.Pow(x: value - average, y: 2)).Sum();
            var variance = sumOfSquares / Values.Length;

            return Convert.ToSingle(value: variance);
        }

        internal static async Task<float> VarianceAsync(float[] Values)
        {
            return await Task.Run<float>(function: () => Variance(Values: Values));
        }

        internal static double Variance(double[] Values)
        {
            var average = Values.Average();
            var sumOfSquares = Values.Select(selector: value => Math.Pow(x: value - average, y: 2)).Sum();
            var variance = sumOfSquares / Values.Length;

            return variance;
        }

        internal static async Task<double> VarianceAsync(double[] Values)
        {
            return await Task.Run<double>(function: () => Variance(Values: Values));
        }
        #endregion



        #region Mean
        internal static float GeometricMean(float[] Values)
        {
            AmountOfData = Values.Length;
            float Multiplication = 1;

            if (Values.Length == 0
                ||
                Values.Length.Equals(0))
            {
                throw new ArgumentException("El array no puede estar vacío");
            }

            foreach (float Item in Values)
            {
                if (Item <= 0)
                {
                    throw new ArgumentException("Los números deben ser mayores que cero para calcular la media geométrica");
                }

                Multiplication *= Item;

                if (float.IsInfinity(Multiplication))
                {
                    throw new OverflowException("El resultado es demasiado grande para ser representado.");
                }
            }

            return NthRoot(Multiplication, AmountOfData);
        }

        internal static async Task<float> GeometricMeanAsync(float[] Values)
        {
            return await Task.Run<float>(function: () => GeometricMean(Values: Values));
        }

        internal static double GeometricMean(double[] Values)
        {
            AmountOfData = Values.Length;
            double Multiplication = 1;

            if (Values.Length == 0
                ||
                Values.Length.Equals(0))
            {
                throw new ArgumentException("El array no puede estar vacío");
            }

            foreach (double Item in Values)
            {
                if (Item <= 0)
                {
                    throw new ArgumentException("Los números deben ser mayores que cero para calcular la media geométrica");
                }

                Multiplication *= Item;

                if (double.IsInfinity(Multiplication))
                {
                    throw new OverflowException("El resultado es demasiado grande para ser representado.");
                }
            }

            return NthRoot(Multiplication, AmountOfData);
        }

        internal static async Task<double> GeometricMeanAsync(double[] Values)
        {
            return await Task.Run<double>(function: () => GeometricMean(Values));
        }
        #endregion
    }
}