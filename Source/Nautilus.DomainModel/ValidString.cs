//--------------------------------------------------------------
// <copyright file="ValidString.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// The immutable abstract <see cref="ValidString"/> class. Encapsulates a validated string.
    /// </summary>
    [Immutable]
    public abstract class ValidString : ValueObject<ValidString>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ValidationException">Throws if the value is null or white space, or if
        /// the string values length is greater than 100 characters.</exception>
        protected ValidString(string value)
        {
            Validate.NotNull(value, nameof(value));
            Validate.True(value.Length <= 100, nameof(value));

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
