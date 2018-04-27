// -------------------------------------------------------------------------------------------------
// <copyright file="BusConnectedComponentBase.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The abstract <see cref="BusConnectedComponentBase"/> class. The base class for all 
    /// <see cref="BlackBox"/> components which are connected to the messaging service.
    /// </summary>
    public abstract class BusConnectedComponentBase : ComponentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusConnectedComponentBase"/> class.
        /// </summary>
        /// <param name="service">The service context.</param>
        /// <param name="component">The component label.</param>
        /// <param name="setupContainer">The black box setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        protected BusConnectedComponentBase(
            BlackBoxService service,
            Label component,
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter)
            : base(service, component, setupContainer)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.MessagingAdapter = messagingAdapter;
        }

        /// <summary>
        /// Gets the components messaging adapter.
        /// </summary>
        protected IMessagingAdapter MessagingAdapter { get; }
    }
}