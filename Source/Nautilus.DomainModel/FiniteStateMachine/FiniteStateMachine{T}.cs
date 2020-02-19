// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachine{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.FiniteStateMachine
{
    using System;
    using System.Collections.Immutable;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides a simple generic finite state machine of state T, comprising of a state transition look-up
    /// table to determine trigger processing validity.
    /// </summary>
    /// <typeparam name="T">The state type.</typeparam>
    internal sealed class FiniteStateMachine<T>
        where T : struct
    {
        private readonly ImmutableDictionary<StateTransition<T>, T> stateTransitionTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachine{T}"/> class.
        /// </summary>
        /// <param name="stateTransitionTable">The state transition table.</param>
        /// <param name="startingState">The starting state.</param>
        /// <exception cref="ArgumentException">Throws if the state transition table is empty.</exception>
        internal FiniteStateMachine(ImmutableDictionary<StateTransition<T>, T> stateTransitionTable, T startingState)
        {
            Condition.NotEmpty(stateTransitionTable, nameof(stateTransitionTable));

            this.stateTransitionTable = stateTransitionTable;
            this.State = startingState;
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        internal T State { get; private set; }

        /// <summary>
        /// Processes the finite state machine with the given <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">Throws if the trigger is null.</exception>
        internal void Process(Trigger trigger)
        {
            var transition = new StateTransition<T>(this.State, trigger);
            if (this.stateTransitionTable.TryGetValue(transition, out var state))
            {
                this.State = state;
            }
            else
            {
                throw new InvalidOperationException($"Invalid state transition ({transition.Description()}).");
            }
        }
    }
}
