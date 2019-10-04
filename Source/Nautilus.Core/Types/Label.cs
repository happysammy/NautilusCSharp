//--------------------------------------------------------------------------------------------------
// <copyright file="Label.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Types
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a validated label.
    /// </summary>
    [Immutable]
    public sealed class Label : Identifier<Label>
    {
        private const string NONE = "NONE";

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="value">The label value.</param>
        public Label(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }

        /// <summary>
        /// Returns a value indicating whether the label is equal to 'NONE'.
        /// </summary>
        /// <returns>True if the label value is 'NONE', else False.</returns>
        public bool IsNone()
        {
            return this.Value == NONE;
        }

        /// <summary>
        /// Returns a value indicating whether the label is not equal to 'NONE'.
        /// </summary>
        /// <returns>True if the label value is not 'NONE', else False.</returns>
        public bool NotNone()
        {
            return this.Value != NONE;
        }
    }
}
