//--------------------------------------------------------------------------------------------------
// <copyright file="ValidString.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;

    /// <summary>
    /// A <see cref="ValueObject{T}"/> which encapsulates a validated string.
    /// </summary>
    [Immutable]
    public abstract class ValidString : ValueObject<ValidString>, IEquatable<ValidString>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected ValidString(string value)
        {
            Debug.NotNull(value, nameof(value));
            Debug.True(value.Length <= 100, nameof(value));

            this.Value = value.RemoveAllWhitespace();
        }

        /// <summary>
        /// Gets the value of the string.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="ValidString"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;

        /// <summary>
        /// Gets the attributes to include in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[] { this.Value };
        }
    }
}
