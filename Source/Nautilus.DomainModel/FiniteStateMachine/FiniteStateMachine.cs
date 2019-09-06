// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachine.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.FiniteStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;

    /// <summary>
    /// Represents a simple generic finite state machine comprising of a state transition look-up
    /// table to determine trigger processing validity.
    /// </summary>
    [PerformanceOptimized]
    internal class FiniteStateMachine
    {
        private readonly ImmutableDictionary<StateTransition, State> stateTransitionTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachine"/> class.
        /// </summary>
        /// <param name="stateTransitionTable">The state transition table.</param>
        /// <param name="startingState">The starting state.</param>
        /// <exception cref="ArgumentException">Throws if the state transition table is empty.</exception>
        internal FiniteStateMachine(
            Dictionary<StateTransition, State> stateTransitionTable,
            State startingState)
        {
            Condition.NotEmpty(stateTransitionTable, nameof(stateTransitionTable));

            this.stateTransitionTable = stateTransitionTable.ToImmutableDictionary();
            this.State = startingState;
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        internal State State { get; private set; }

        /// <summary>
        /// Gets the current state as an enumeration of type T.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration to return.</typeparam>
        /// <returns>The state as an enumeration.</returns>
        internal T StateAsEnum<T>()
            where T : struct, Enum
        {
            return this.State.ToString().ToEnum<T>();
        }

        /// <summary>
        /// Processes the finite state machine with the given <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">Throws if the trigger is null.</exception>
        internal void Process(Trigger trigger)
        {
            var transition = new StateTransition(this.State, trigger);

            if (!this.IsValidStateTransition(transition))
            {
                throw new InvalidOperationException($"Invalid state transition ({transition.Description()}).");
            }

            this.ChangeStateTo(this.StateTransitionResult(transition));
        }

        private bool IsValidStateTransition(StateTransition transition) => this.stateTransitionTable.ContainsKey(transition);

        private void ChangeStateTo(State state)
        {
            Debug.NotDefault(state, nameof(state));

            this.State = state;
        }

        private State StateTransitionResult(StateTransition transition)
        {
            return this.stateTransitionTable[transition];
        }
    }
}
