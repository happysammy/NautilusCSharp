// -------------------------------------------------------------------------------------------------
// <copyright file="Trigger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.FiniteStateMachine
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a possible trigger within the <see cref="FiniteStateMachine"/>.
    /// </summary>
    [Immutable]
    internal struct Trigger : IEquatable<Trigger>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> structure.
        /// </summary>
        /// <param name="trigger">The state.</param>
        /// <exception cref="ArgumentNullException">Throws if the argument is null.</exception>
        internal Trigger(string trigger)
        {
            Debug.NotEmptyOrWhiteSpace(trigger, nameof(trigger));

            this.Value = trigger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> structure.
        /// </summary>
        /// <param name="trigger">The state.</param>
        /// <exception cref="ArgumentNullException">Throws if the argument is null.</exception>
        internal Trigger(Enum trigger)
        {
            Debug.NotNull(trigger, nameof(trigger));

            this.Value = trigger.ToString();
        }

        /// <summary>
        /// Gets the <see cref="string"/> value of this trigger.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A boolean.</returns>
        public static bool operator ==(Trigger left, Trigger right) => left.Equals(right);

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A boolean.</returns>
        public static bool operator !=(Trigger left, Trigger right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A boolean.</returns>
        public override bool Equals(object obj) => obj is Trigger other && this.Value == other.Value;

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified <see cref="Trigger"/>.
        /// </summary>
        /// <param name="other">The other state.</param>
        /// <returns>A boolean.</returns>
        public bool Equals(Trigger other) => this.Value == other.Value;

        /// <summary>
        /// Returns the hash code of this <see cref="Trigger"/>.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="Trigger"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => this.Value;
    }
}
