﻿//--------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachine.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.FiniteStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// Represents a simple generic finite state machine comprising of a state transition look-up
    /// table to determine trigger processing validity.
    /// </summary>
    public class FiniteStateMachine
    {
        private readonly IImmutableDictionary<StateTransition, State> stateTransitionTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachine"/> class.
        /// </summary>
        /// <param name="stateTransitionTable">The state transition table.</param>
        /// <param name="startingState">The starting state.</param>
        /// <exception cref="ArgumentNullException">Throws if either argument is null.</exception>
        /// <exception cref="ArgumentException">Throws if the state transition table is empty.</exception>
        public FiniteStateMachine(
            IReadOnlyDictionary<StateTransition, State> stateTransitionTable,
            State startingState)
        {
            Validate.ReadOnlyCollectionNotNullOrEmpty(stateTransitionTable, nameof(stateTransitionTable));
            Validate.NotNull(startingState, nameof(startingState));

            this.stateTransitionTable = stateTransitionTable.ToImmutableDictionary();
            this.CurrentState = startingState;
        }

        /// <summary>
        /// Gets the current <see cref="State"/>.
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Processes the finite state machine with the given <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <returns>A command result.</returns>
        /// <exception cref="ArgumentNullException">Throws if the trigger is null.</exception>
        public CommandResult Process(Trigger trigger)
        {
            Validate.NotNull(trigger, nameof(trigger));

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
