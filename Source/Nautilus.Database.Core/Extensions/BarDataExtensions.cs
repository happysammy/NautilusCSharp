//--------------------------------------------------------------
// <copyright file="BarDataExtensions.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System;
using System.Text;
using NautechSystems.CSharp;
using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Validation;

namespace Nautilus.Database.Core.Extensions
{
    /// <summary>
    /// Provides useful extension methods for converting bar data between types.
    /// </summary>
    [Immutable]
    public static class BarDataExtensions
    {
        /// <summary>
        /// Returns a valid <see cref="Bar"/> from this <see cref="string"/>.
        /// </summary>
        /// <param name="barString">The bar string.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        public static Bar ToBarData(this string barString)
        {
            Debug.NotNull(barString, nameof(barString));

            var values = barString.Split(',');

            return new Bar(
                values[0].ToZonedDateTimeFromIso(),
                SafeConvert.ToDecimal(values[1]),
                SafeConvert.ToDecimal(values[2]),
                SafeConvert.ToDecimal(values[3]),
                SafeConvert.ToDecimal(values[4]),
                Convert.ToInt64(SafeConvert.ToDecimal(values[5])));
        }

        /// <summary>
        /// Returns a valid <see cref="Bar"/> from this <see cref="byte"/> array.
        /// </summary>
        /// <param name="barBytes">The bar bytes array.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public static Bar ToBarData(this byte[] barBytes)
        {
            Debug.CollectionNotNullOrEmpty(barBytes, nameof(barBytes));

            var values = Encoding.Default
                .GetString(barBytes)
                .Split(',');

            return new Bar(
                values[0].ToZonedDateTimeFromIso(),
                SafeConvert.ToDecimal(values[1]),
                SafeConvert.ToDecimal(values[2]),
                SafeConvert.ToDecimal(values[3]),
                SafeConvert.ToDecimal(values[4]),
                Convert.ToInt64(SafeConvert.ToDecimal(values[5])));
        }
    }
}