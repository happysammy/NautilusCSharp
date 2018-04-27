// -------------------------------------------------------------------------------------------------
// <copyright file="TestActor.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Akka.Actor;

    /// <summary>
    /// The test actor.
    /// </summary>
    public class TestActor : UntypedActor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestActor"/> class.
        /// </summary>
        public TestActor()
        {
            // Do not delete.
        }

        /// <summary>
        /// The on receive.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected override void OnReceive(object message)
        {
        }
    }
}