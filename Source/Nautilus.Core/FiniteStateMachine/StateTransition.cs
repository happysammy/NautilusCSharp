// -------------------------------------------------------------------------------------------------
// <copyright file="StateTransition.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   https://github.com/nautechsystems/NautechSystems.FiniteStateMachine
//   the use of this source code is governed by the Apache 2.0 license
//   as found in the LICENSE.txt file.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Core.FiniteStateMachine
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// Represents the concept of a starting <see cref="State"/>, which is then affected by an event
    /// <see cref="Trigger"/> resulting in a valid resultant <see cref="State"/>.
    /// </summary>
    [Immutable]
    public struct StateTransition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateTransition"/> struct.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">Throws if either argument is null.</exception>
        public StateTransition(State currentState, Trigger trigger)
        {
            Validate.NotNull(currentState, nameof(currentState));
            Validate.NotNull(trigger, nameof(trigger));

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
        /// Returns a value indicating whether this instance is equal to the specified <see cref="StateTransition"/>.
        /// </summary>
        /// <param name="other">The other state transition.</param>
        /// <returns>A boolean.</returns>
        public bool Equals(StateTransition other) =>
               this.CurrentState.Equals(other.CurrentState)
            && this.Trigger.Equals(other.Trigger);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A boolean.</returns>
        public override bool Equals(object obj) =>
            obj is StateTransition other
            && this.CurrentState == other.CurrentState
            && this.Trigger == other.Trigger;

        /// <summary>
        /// Returns the hash code of this <see cref="StateTransition"/>.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode() => this.CurrentState.GetHashCode() + this.Trigger.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="StateTransition"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => $"{nameof(StateTransition)}: {this.CurrentState} -> {this.Trigger}";
    }
}
