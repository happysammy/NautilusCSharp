//--------------------------------------------------------------------------------------------------
// <copyright file="EntityId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// A <see cref="ValueObject{T}"/> which represents a unique validated entity identifier.
    /// </summary>
    [Immutable]
    public sealed class EntityId : ValidString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId"/> class.
        /// </summary>
        /// <param name="value">The entity id value.</param>
        /// <exception cref="ValidationException">Throws if the value is null or white space, or if
        /// the string values length is greater than 100 characters.</exception>
        public EntityId(string value) : base(value)
        {
            // Validated by base class.
        }
    }
}