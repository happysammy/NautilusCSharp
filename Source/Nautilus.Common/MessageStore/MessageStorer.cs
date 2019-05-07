// -------------------------------------------------------------------------------------------------
// <copyright file="MessageStorer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.MessageStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;

    /// <summary>
    /// Sends received messages to the <see cref="InMemoryMessageStore"/>.
    /// </summary>
    public sealed class MessageStorer : ComponentBase
    {
        private readonly IMessageStore store;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStorer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="store">The message store.</param>
        public MessageStorer(IComponentryContainer container, IMessageStore store)
        : base(
            NautilusService.Messaging,
            new Label(nameof(MessageStorer)),
            container)
        {
            this.store = store;
        }

        /// <summary>
        /// Runs when the component stops.
        /// </summary>
        public new void Stop()
        {
            // Allow the system to shutdown first.
            Task.Delay(1000).Wait();

            var commandReport = GetDiagnosticReport(this.store.CommandEnvelopes);
            var eventReport = GetDiagnosticReport(this.store.EventEnvelopes);
            var serviceReport = GetDiagnosticReport(this.store.DocumentEnvelopes);

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("        Messaging Service - Diagnostics Report " + DateTime.UtcNow);
            Console.WriteLine("-----------------------------------------------------------------------------");

            PrintReportToConsole(commandReport);
            PrintReportToConsole(eventReport);
            PrintReportToConsole(serviceReport);
        }

        private static MessagingDiagnosticsReport GetDiagnosticReport<T>(IReadOnlyCollection<Envelope<T>> envelopeList)
            where T : Message
        {
            var messageType = typeof(T).Name;

            var totalCount = envelopeList.Count;

            var isUnopenedCount = envelopeList.Count(e => !e.IsOpened);

            var fastestDeliveryTime = int.MaxValue;
            var slowestDeliveryTime = int.MinValue;

            var deliveryTimesList = new List<int>();

            foreach (var envelope in envelopeList)
            {
                var deliveryTime = GetDeliveryTime(envelope);

                if (deliveryTime < fastestDeliveryTime)
                {
                    fastestDeliveryTime = deliveryTime;
                }

                if (deliveryTime > slowestDeliveryTime)
                {
                    slowestDeliveryTime = deliveryTime;
                }

                deliveryTimesList.Add(GetDeliveryTime(envelope));
            }

            var averageDeliveryTime = deliveryTimesList.Count > 0
                ? (int)Math.Round(deliveryTimesList.Average())
                : 0;

            if (fastestDeliveryTime == int.MaxValue)
            {
                fastestDeliveryTime = 0;
            }

            if (slowestDeliveryTime == int.MinValue)
            {
                slowestDeliveryTime = 0;
            }

            var diagnosticsReport = new MessagingDiagnosticsReport(
                messageType,
                totalCount,
                isUnopenedCount,
                averageDeliveryTime,
                fastestDeliveryTime,
                slowestDeliveryTime);

            return diagnosticsReport;
        }

        private static int GetDeliveryTime<T>(Envelope<T> envelope)
            where T : Message
        {
            return envelope.IsOpened
                ? (envelope.OpenedTime.Value - envelope.Timestamp).SubsecondTicks
                : 0;
        }

        private static void PrintReportToConsole(MessagingDiagnosticsReport report)
        {
            Console.WriteLine();
            Console.WriteLine($"[{report.MessageType} Messages]");
            Console.WriteLine($"FastestDeliveryTime = {Math.Round(report.FastestDeliveryTime / 10000, 2)}ms");
            Console.WriteLine($"AverageDeliveryTime = {Math.Round(report.AverageDeliveryTime / 10000, 2)}ms");
            Console.WriteLine($"SlowestDeliveryTime = {Math.Round(report.SlowestDeliveryTime / 10000, 2)}ms");
            Console.WriteLine($"TotalCount = {report.TotalCount}");
            Console.WriteLine($"IsUnopenedCount = {report.IsUnopenedCount}");
        }

        /// <summary>
        /// Store the given command envelope.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        private void OnMessage(Envelope<Command> envelope)
        {
            this.store.Store(envelope);
        }

        /// <summary>
        /// Store the given event envelope.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        private void OnMessage(Envelope<Event> envelope)
        {
            this.store.Store(envelope);
        }

        /// <summary>
        /// Store the given document envelope.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        private void OnMessage(Envelope<Document> envelope)
        {
            this.store.Store(envelope);
        }
    }
}
