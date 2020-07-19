// -------------------------------------------------------------------------------------------------
// <copyright file="StateTransition{T}.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core;
using Nautilus.Core.Annotations;

namespace Nautilus.DomainModel.FiniteStateMachine
{
    /// <summary>
    /// Represents the concept of a starting <see cref="State"/>, which is then affected by an event
    /// <see cref="Trigger"/> resulting in a valid resultant <see cref="State"/>.
    /// </summary>
    /// <typeparam name="T">The state type.</typeparam>
    [Immutable]
    internal struct StateTransition<T> : IEquatable<object>, IEquatable<StateTransition<T>>
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateTransition{T}"/> struct.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="trigger">The trigger.</param>
        internal StateTransition(T currentState, Trigger trigger)
        {
            this.CurrentState = currentState;
            this.Trigger = trigger;
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public T CurrentState { get; }

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        public Trigger Trigger { get; }

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A boolean.</returns>
        public static bool operator ==(StateTransition<T> left, StateTransition<T> right) => left.Equals(right);

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A boolean.</returns>
        public static bool operator !=(StateTransition<T> left, StateTransition<T> right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj is StateTransition<T> transition && this.Equals(transition);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified <see cref="StateTransition{T}"/>.
        /// </summary>
        /// <param name="other">The other state transition.</param>
        /// <returns>A boolean.</returns>
        public bool Equals(StateTransition<T> other) => this.CurrentState.Equals(other.CurrentState) && this.Trigger.Equals(other.Trigger);

        /// <summary>
        /// Returns the hash code of this <see cref="StateTransition{T}"/>.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode() => Hash.GetCode(this.CurrentState, this.Trigger);

        /// <summary>
        /// Returns a string representation of the <see cref="StateTransition{T}"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => $"{nameof(StateTransition<T>)}({this.Description()})";

        /// <summary>
        /// Returns the description of the state transition including current state and trigger.
        /// </summary>
        /// <returns>The description string.</returns>
        public string Description() => $"{this.CurrentState} -> {this.Trigger}";
    }
}
