// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachine{T}.cs" company="Nautech Systems Pty Ltd">
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
using System.Collections.Immutable;
using Nautilus.Core.Correctness;

namespace Nautilus.DomainModel.FiniteStateMachine
{
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
