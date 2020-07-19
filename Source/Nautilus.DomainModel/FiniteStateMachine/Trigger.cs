// -------------------------------------------------------------------------------------------------
// <copyright file="Trigger.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Message;

namespace Nautilus.DomainModel.FiniteStateMachine
{
    /// <summary>
    /// Represents a possible trigger within the <see cref="FiniteStateMachine"/>.
    /// </summary>
    [Immutable]
    internal struct Trigger : IEquatable<Trigger>
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> structure.
        /// </summary>
        /// <param name="value">The trigger value.</param>
        private Trigger(string value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));

            this.value = value;
        }

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
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj is Trigger trigger && this.Equals(trigger);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified <see cref="Trigger"/>.
        /// </summary>
        /// <param name="other">The other state.</param>
        /// <returns>A boolean.</returns>
        public bool Equals(Trigger other) => this.value == other.value;

        /// <summary>
        /// Returns the hash code of this <see cref="Trigger"/>.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode() => this.value.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="Trigger"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => this.value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> structure.
        /// </summary>
        /// <param name="type">The trigger event type.</param>
        /// <returns>The trigger.</returns>
        internal static Trigger Event(Type type)
        {
            Debug.True(type.IsSubclassOf(typeof(Event)), "type.IsSubclassOf(typeof(Event))");

            return new Trigger(type.Name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> structure.
        /// </summary>
        /// <param name="event">The trigger event.</param>
        /// <returns>The trigger.</returns>
        internal static Trigger Event(Event @event)
        {
            return new Trigger(@event.Type.Name);
        }
    }
}
