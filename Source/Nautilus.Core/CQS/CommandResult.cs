//--------------------------------------------------------------------------------------------------
// <copyright file="CommandResult.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

using System.Linq;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.CQS.Base;

namespace Nautilus.Core.CQS
{
    /// <summary>
    /// Represents the result of a command operation. May contain a result message.
    /// </summary>
    [Immutable]
    public sealed class CommandResult : Result
    {
        private static readonly CommandResult OkResult = new CommandResult(false, nameof(Ok));

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class.
        /// </summary>
        /// <param name="isFailure">The result is failure boolean flag.</param>
        /// <param name="message">The result message string.</param>
        private CommandResult(bool isFailure, string message)
            : base(isFailure, message)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));
        }

        /// <summary>
        /// Returns a success <see cref="CommandResult"/> with a 'None' message.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public static CommandResult Ok()
        {
            return OkResult;
        }

        /// <summary>
        /// Returns a success <see cref="CommandResult"/> with the given message.
        /// </summary>
        /// <param name="message">The result success message.</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public static CommandResult Ok(string message)
        {
            return new CommandResult(false, message);
        }

        /// <summary>
        /// Returns a failure <see cref="CommandResult"/> with the given message.
        /// </summary>
        /// <param name="errorMessage">The result error message (cannot be null or white space).</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public static CommandResult Fail(string errorMessage)
        {
            return new CommandResult(true, errorMessage);
        }

        /// <summary>
        /// Returns first failure in the list of <paramref name="results"/> (if there is no failure
        /// then returns success).
        /// </summary>
        /// <param name="results">The command results array.</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public static CommandResult FirstFailureOrSuccess(params CommandResult[] results)
        {
            Debug.NotEmpty(results, nameof(results));

            // ReSharper disable once ConstantNullCoalescingCondition (can be null)
            return results.FirstOrDefault(c => c.IsFailure) ?? Ok();
        }

        /// <summary>
        /// Returns a result from all failures in the <paramref name="results"/> list (if
        /// there is no failure then returns success).
        /// </summary>
        /// <param name="results">The command results array.</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public static CommandResult Combine(params CommandResult[] results)
        {
            var failedResults = results
                .Where(x => x.IsFailure)
                .ToArray();

            return failedResults.Any()
                ? Fail(CombineErrorMessages(failedResults))
                : Ok();
        }

        private static string CombineErrorMessages(CommandResult[] failedResults)
        {
            Debug.NotEmpty(failedResults, nameof(failedResults));

            return string.Join("; ", failedResults.Select(x => x.Message));
        }
    }
}
