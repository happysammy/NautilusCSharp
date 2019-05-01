//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusExecutorService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor.Service
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a REST API for the <see cref="NautilusExecutor"/> system.
    /// </summary>
    [Immutable]
    public sealed class NautilusExecutorService
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusExecutorService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        public NautilusExecutorService(IComponentryContainer setupContainer)
        {
            Precondition.NotNull(setupContainer, nameof(setupContainer));

            this.clock = setupContainer.Clock;
            this.guidFactory = setupContainer.GuidFactory;
            this.logger = setupContainer.LoggerFactory.Create(
                NautilusService.Execution,
                LabelFactory.Create(nameof(NautilusExecutorService)));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        public void Test()
        {
            var x1 = this.clock.TimeNow();
            var x2 = this.guidFactory.NewGuid();
            this.logger.Debug($"Test logger {x1} {x2}");
        }
    }
}
