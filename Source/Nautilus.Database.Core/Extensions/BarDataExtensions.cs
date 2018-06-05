// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataExtensions.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Extensions
{
    using System;
    using System.Text;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.ValueObjects;

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
        public static BarData ToBarData(this string barString)
        {
            Debug.NotNull(barString, nameof(barString));

            var values = barString.Split(',');

            return new BarData(
                SafeConvert.ToDecimal(values[0]),
                SafeConvert.ToDecimal(values[1]),
                SafeConvert.ToDecimal(values[2]),
                SafeConvert.ToDecimal(values[3]),
                Convert.ToInt64(SafeConvert.ToDecimal(values[4])),
                values[5].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a valid <see cref="Bar"/> from this <see cref="byte"/> array.
        /// </summary>
        /// <param name="barBytes">The bar bytes array.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public static BarData ToBarData(this byte[] barBytes)
        {
            Debug.CollectionNotNullOrEmpty(barBytes, nameof(barBytes));

            var values = Encoding.UTF8
                .GetString(barBytes)
                .Split(',');

            return new BarData(
                SafeConvert.ToDecimal(values[0]),
                SafeConvert.ToDecimal(values[1]),
                SafeConvert.ToDecimal(values[2]),
                SafeConvert.ToDecimal(values[3]),
                Convert.ToInt64(SafeConvert.ToDecimal(values[4])),
                values[5].ToZonedDateTimeFromIso());
        }
    }
}
