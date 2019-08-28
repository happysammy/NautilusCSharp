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
        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="value">The label value.</param>
        public Label(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
