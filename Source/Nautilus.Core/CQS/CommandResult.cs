//--------------------------------------------------------------------------------------------------
// <copyright file="CommandResult.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.CQS
{
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS.Base;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents the result of a command operation. May contain a result message.
    /// </summary>
    [Immutable]
    public sealed class CommandResult : Result
    {
        private static readonly CommandResult OkResult = new CommandResult(false, "No result message");

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class.
        /// </summary>
        /// <param name="isFailure">The result is failure boolean flag.</param>
        /// <param name="message">The result message string.</param>
        private CommandResult(bool isFailure, string message)
            : base(isFailure, message)
        {
            Debug.NotNull(message, nameof(message));
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
            Debug.NotNull(message, nameof(message));

            return new CommandResult(false, message);
        }

        /// <summary>
        /// Returns a failure <see cref="CommandResult"/> with the given message.
        /// </summary>
        /// <param name="errorMessage">The result error message (cannot be null or white space).</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public static CommandResult Fail(string errorMessage)
        {
            Debug.NotNull(errorMessage, nameof(errorMessage));

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
            Debug.NotNullOrEmpty(results, nameof(results));

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
            Debug.NotNullOrEmpty(results, nameof(results));

            var failedResults = results
                .Where(x => x.IsFailure)
                .ToArray();

            return failedResults.Any()
                ? Fail(CombineErrorMessages(failedResults))
                : Ok();
        }

        private static string CombineErrorMessages(CommandResult[] failedResults)
        {
            Debug.NotNullOrEmpty(failedResults, nameof(failedResults));

            return string.Join("; ", failedResults.Select(x => x.Message));
        }
    }
}
