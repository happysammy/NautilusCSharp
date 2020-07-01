// -------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;

namespace Nautilus.DomainModel.FiniteStateMachine
{
    /// <summary>
    /// Represents a possible state within the <see cref="FiniteStateMachine"/>.
    /// </summary>
    [Immutable]
    internal struct State : IEquatable<State>
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> structure.
        /// </summary>
        /// <param name="state">The state.</param>
        internal State(string state)
        {
            Debug.NotEmptyOrWhiteSpace(state, nameof(state));

            this.value = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> structure.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentNullException">Throws if the argument is null.</exception>
        internal State(Enum state)
        {
            this.value = state.ToString();
        }

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
        public override bool Equals(object? obj) => obj is State state && this.Equals(state);

        /// <summary>
        /// Returns a value indicating whether this <see cref="State"/> is equal to the
        /// specified <see cref="State"/>.
        /// </summary>
        /// <param name="other">The other state.</param>
        /// <returns>A boolean.</returns>
        public bool Equals(State other) => this.value.Equals(other.value);

        /// <summary>
        /// Returns the hash code of this <see cref="State"/>.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode() => this.value.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="State"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => this.value;
    }
}
