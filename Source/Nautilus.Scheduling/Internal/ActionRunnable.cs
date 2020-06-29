// -------------------------------------------------------------------------------------------------
// <copyright file="ActionRunnable.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Scheduling.Internal
{
    using System;

    /// <summary>
    /// <see cref="IRunnable"/> which executes an <see cref="Action"/>.
    /// </summary>
    internal sealed class ActionRunnable : IRunnable
    {
        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRunnable"/> class.
        /// </summary>
        /// <param name="action">The runnable delegate.</param>
        internal ActionRunnable(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        public void Run()
        {
            this.action();
        }
    }
}
