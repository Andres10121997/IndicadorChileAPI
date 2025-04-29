using System;
using System.Collections.Generic;
using System.Numerics;

namespace IndicadorChileAPI
{
    public static partial class Enumerable
    {
        public static double Variance(this IEnumerable<long> source) => Variance<long, long, double>(source);

        public static float Variance(this IEnumerable<float> source) => (float)Variance<float, double, double>(source);

        public static double Variance(this IEnumerable<double> source) => Variance<double, double, double>(source);

        public static decimal Variance(this IEnumerable<decimal> source) => Variance<decimal, decimal, decimal>(source);

        private static TResult Variance<TSource, TAccumulator, TResult>(this IEnumerable<TSource> source)
            where TSource : struct, INumber<TSource>
            where TAccumulator : struct, INumber<TAccumulator>
            where TResult : struct, INumber<TResult>
        {
            #region Variables
            long count;
            TSource average;
            TAccumulator sum;
            TAccumulator sumOfSquares;
            TAccumulator variance;
            #endregion

            if (source is null)
            {
                throw new ArgumentNullException(paramName: nameof(source));
            }

            count = 0;
            sum = TAccumulator.Zero;

            // First pass: calculate sum and quantity
            foreach (var item in source)
            {
                sum += TAccumulator.CreateChecked(value: item);

                count++;
            }

            if (count == 0)
            {
                throw new InvalidOperationException(message: "Sequence contains no elements.");
            }

            average = TSource.CreateChecked(value: sum) / TSource.CreateChecked((TAccumulator)TAccumulator.CreateChecked(value: count));

            // Second pass: calculate sum of squares
            sumOfSquares = TAccumulator.Zero;

            foreach (TSource item in source)
            {
                TAccumulator diff = TAccumulator.CreateChecked(value: item - average);
                sumOfSquares += diff * diff;
            }

            variance = sumOfSquares / TAccumulator.CreateChecked(value: count);

            return TResult.CreateChecked(value: variance);
        }
    }
}
