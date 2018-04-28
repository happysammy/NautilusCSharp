//--------------------------------------------------------------
// <copyright file="ArrayExtensions.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators.Extensions
{
    using System;
    using System.Linq;
    using NautechSystems.CSharp.Annotations;

    /// <summary>
    /// The immutable static <see cref="ArrayExtensions"/> class.
    /// </summary>
    [Immutable]
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns the variance of the array of values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double Variance(this double[] values)
        {
            return values.Variance(values.Average(), 0, values.Length);
        }

        /// <summary>
        /// The variance.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="mean">The mean.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double Variance(this double[] values, double mean)
        {
            return values.Variance(mean, 0, values.Length);
        }

        /// <summary>
        /// Returns the variance of the array of values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="mean">The mean.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double Variance(this double[] values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / n;
        }

        /// <summary>
        /// Returns the standard deviation of the array of values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double StandardDeviation(this double[] values)
        {
            return values.Length == 0
                ? 0
                : values.StandardDeviation(0, values.Length);
        }

        /// <summary>
        /// Returns the standard deviation of the array of values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double StandardDeviation(this double[] values, int start, int end)
        {
            double mean = values.Average();
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }
    }
}