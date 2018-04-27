// -------------------------------------------------------------------------------------------------
// <copyright file="EntityId.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// The immutable sealed <see cref="EntityId"/> class. Represents a unique validated entity identifier.
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