// -------------------------------------------------------------------------------------------------
// <copyright file="StateTransition.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.FiniteStateMachine
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents the concept of a starting <see cref="State"/>, which is then affected by an event
    /// <see cref="Trigger"/> resulting in a valid resultant <see cref="State"/>.
    /// </summary>
    [Immutable]
    internal struct StateTransition : IEquatable<object>, IEquatable<StateTransition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateTransition"/> struct.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="trigger">The trigger.</param>
        internal StateTransition(State currentState, Trigger trigger)
        {
            this.CurrentState = currentState;
            this.Trigger = trigger;
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public State CurrentState { get; }

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
        public static bool operator ==(StateTransition left, StateTransition right) => left.Equals(right);

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A boolean.</returns>
        public static bool operator !=(StateTransition left, StateTransition right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A boolean.</returns>
        public override bool Equals(object other) => other is StateTransition transition && this.Equals(transition);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to the specified <see cref="StateTransition"/>.
        /// </summary>
        /// <param name="other">The other state transition.</param>
        /// <returns>A boolean.</returns>
        public bool Equals(StateTransition other) => this.CurrentState.Equals(other.CurrentState) && this.Trigger.Equals(other.Trigger);

        /// <summary>
        /// Returns the hash code of this <see cref="StateTransition"/>.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode() => Hash.GetCode(this.CurrentState, this.Trigger);

        /// <summary>
        /// Returns a string representation of the <see cref="StateTransition"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => $"{nameof(StateTransition)}({this.Description()})";

        /// <summary>
        /// Returns the description of the state transition including current state and trigger.
        /// </summary>
        /// <returns>The description string.</returns>
        public string Description() => $"{this.CurrentState} -> {this.Trigger}";
    }
}
