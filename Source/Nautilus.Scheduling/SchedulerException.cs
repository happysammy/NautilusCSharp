// -------------------------------------------------------------------------------------------------
// <copyright file="SchedulerException.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Scheduling
{
    /// <summary>
    /// An exception that is thrown by the <see cref="IScheduler">Schedule*</see> methods
    /// when scheduling is not possible, e.g. after shutting down the <see cref="IScheduler"/>.
    /// </summary>
    public sealed class SchedulerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SchedulerException(string message)
            : base(message)
        {
        }
    }
}
