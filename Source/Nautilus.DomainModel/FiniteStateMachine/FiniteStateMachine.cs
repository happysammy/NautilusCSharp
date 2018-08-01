// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachine.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.FiniteStateMachine
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Collections;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a simple generic finite state machine comprising of a state transition look-up
    /// table to determine trigger processing validity.
    /// </summary>
    internal class FiniteStateMachine
    {
        private readonly ReadOnlyDictionary<StateTransition, State> stateTransitionTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachine"/> class.
        /// </summary>
        /// <param name="stateTransitionTable">The state transition table.</param>
        /// <param name="startingState">The starting state.</param>
        /// <exception cref="ArgumentNullException">Throws if either argument is null.</exception>
        /// <exception cref="ArgumentException">Throws if the state transition table is empty.</exception>
        internal FiniteStateMachine(
            Dictionary<StateTransition, State> stateTransitionTable,
            State startingState)
        {
            Validate.CollectionNotNullOrEmpty(stateTransitionTable, nameof(stateTransitionTable));
            Validate.NotNull(startingState, nameof(startingState));

            this.stateTransitionTable = new ReadOnlyDictionary<StateTransition, State>(stateTransitionTable);
            this.CurrentState = startingState;
        }

        /// <summary>
        /// Gets the current <see cref="State"/>.
        /// </summary>
        internal State CurrentState { get; private set; }

        /// <summary>
        /// Processes the finite state machine with the given <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <returns>A command result.</returns>
        /// <exception cref="ArgumentNullException">Throws if the trigger is null.</exception>
        internal CommandResult Process(Trigger trigger)
        {
            Debug.NotNull(trigger, nameof(trigger));

            var transition = new StateTransition(this.CurrentState, trigger);

            return this.IsValidStateTransition(transition)
                       ? CommandResult.Ok().OnSuccess(() => this.ChangeStateTo(this.StateTransitionResult(transition)))
                       : CommandResult.Fail($"Invalid state transition: {this.CurrentState} -> {trigger}");
        }

        private bool IsValidStateTransition(StateTransition transition) => this.stateTransitionTable.ContainsKey(transition);

        private void ChangeStateTo(State state)
        {
            Debug.NotDefault(state, nameof(state));

            this.CurrentState = state;
        }

        private State StateTransitionResult(StateTransition transition)
        {
            return this.stateTransitionTable[transition];
        }
    }
}
