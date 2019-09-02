//--------------------------------------------------------------------------------------------------
// <copyright file="IdentifierUniqueness.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Annotations
{
    using System;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// This decorative attribute indicates that there is a uniqueness requirement for the annotated
    /// identifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IdentifierUniqueness : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierUniqueness"/> class.
        /// </summary>
        /// <param name="uniqueness">The identifier uniqueness level.</param>
        public IdentifierUniqueness(Uniqueness uniqueness)
        {
            this.UniquenessLevel = uniqueness;
        }

        /// <summary>
        /// Gets the annotations uniqueness level.
        /// </summary>
        public Uniqueness UniquenessLevel { get; }
    }
}
