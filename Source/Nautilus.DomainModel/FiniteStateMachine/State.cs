// -------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.FiniteStateMachine
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core;

    /// <summary>
    /// Represents a possible state within the <see cref="FiniteStateMachine"/>.
    /// </summary>
    [Immutable]
    internal struct State : IEquatable<State>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> structure.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentNullException">Throws if the argument is null.</exception>
        internal State(Enum state)
        {
            Debug.NotNull(state, nameof(state));

            this.Value = state;
        }

        /// <summary>
        /// Gets the <see cref="Enum"/> value of the state.
        /// </summary>
        public Enum Value { get; }

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A boolean.</returns>
        public static bool operator ==(State left, State right) => left.Equals(right);

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A boolean.</returns>
        public static bool operator !=(State left, State right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A boolean.</returns>
        public override bool Equals(object obj) => obj is State other && this.Value.Equals(other.Value);

        /// <summary>
        /// Returns a value indicating whether this <see cref="State"/> is equal to the
        /// specified <see cref="State"/>.
        /// </summary>
        /// <param name="other">The other state.</param>
        /// <returns>A boolean.</returns>
        public bool Equals(State other) => this.Value.Equals(other.Value);

        /// <summary>
        /// Returns the hash code of this <see cref="State"/>.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="State"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => this.Value.ToString();
    }
}
