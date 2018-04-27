// -------------------------------------------------------------------------------------------------
// <copyright file="FractalDimension.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;

    /// <summary>
    /// The fractal dimension.
    /// </summary>
    public class FractalDimension
    {
        /// <summary>
        /// The ymin.
        /// </summary>
        private double ymin;

        /// <summary>
        /// The ymax.
        /// </summary>
        private double ymax;

        /// <summary>
        /// The calculate.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The double array.
        /// </returns>
        public double[] Calculate(double[] data)
        {
            if (data== null || data.Length <= 0)
            {
                return null;
            }

            var result = new double[data.Length];
            ymin = data[0];
            ymax = data[0];

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < ymin)
                {
                    ymin = data[i];
                }

                if (data[i] > ymax)
                {
                    ymax = data[i];
                }

                var length = LengthCalculation(data, i);

                if (i == 0)
                {
                    result[i] = length;
                }
                else
                {
                    result[i] = 1 + Math.Log(length) / Math.Log(2 * i);
                }
            }

            return result;
        }

        /// <summary>
        /// The length calculation.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="pos">
        /// The pos.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private double LengthCalculation(double[] data, int pos)
        {
            double y;
            var yant = 0.0;
            var result = double.NaN;

            if (pos == 0)
            {
                return result;
            }

            result = 0.0;

            for (int i = 0; i < pos; i++)
            {
                y = (data[i] - ymin) / (ymax - ymin);


                if (i > 0)
                {
                    result = result + Math.Sqrt(Math.Pow(y - yant, 2) + Math.Pow(1 / pos, 2));
                }

                yant = y;
            }

            return result;
        }
    }
}