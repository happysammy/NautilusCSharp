//--------------------------------------------------------------------------------------------------
// <copyright file="InitializeRiskModel.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.SystemCommands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Core;
    using Nautilus.DomainModel.Interfaces;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="InitializeRiskModel"/> class.
    /// </summary>
    [Immutable]
    public sealed class InitializeRiskModel : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeRiskModel"/> class.
        /// </summary>
        /// <param name="account">The message account.</param>
        /// <param name="riskModel">The message risk model.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public InitializeRiskModel(
            IBrokerageAccount account,
            IRiskModel riskModel,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(account, nameof(account));
            Validate.NotNull(riskModel, nameof(riskModel));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Account = account;
            this.RiskModel = riskModel;
        }

        /// <summary>
        /// Gets the messages account.
        /// </summary>
        public IBrokerageAccount Account { get; }

        /// <summary>
        /// Gets the messages risk model.
        /// </summary>
        public IRiskModel RiskModel { get; }
    }
}
