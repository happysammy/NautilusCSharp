// -------------------------------------------------------------------------------------------------
// <copyright file="MessageStorer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.MessageStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Sends received messages to the <see cref="InMemoryMessageStore"/>.
    /// </summary>
    public sealed class MessageStorer : ReceiveActor
    {
        private readonly InMemoryMessageStore store;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStorer"/> class.
        /// </summary>
        /// <param name="store">The warehouse.</param>
        /// <exception cref="ValidationException">Throws if the warehouse is null.</exception>
        public MessageStorer(InMemoryMessageStore store)
        {
            Validate.NotNull(store, nameof(store));

            this.store = store;

            this.Receive<Envelope<CommandMessage>>(envelope => this.store.Store(envelope));
            this.Receive<Envelope<EventMessage>>(envelope => this.store.Store(envelope));
            this.Receive<Envelope<DocumentMessage>>(envelope => this.store.Store(envelope));
        }

        /// <summary>
        /// Runs when the inherited <see cref="ReceiveActor"/> stops.
        /// </summary>
        protected override void PostStop()
        {
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
            Debug.NotNull(envelopeList, nameof(envelopeList));

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
            Debug.NotNull(envelope, nameof(envelope));

            return envelope.IsOpened
                ? (envelope.OpenedTime.Value - envelope.Timestamp).Value.SubsecondTicks
                : 0;
        }

        private static void PrintReportToConsole(MessagingDiagnosticsReport report)
        {
            Debug.NotNull(report, nameof(report));

            Console.WriteLine();
            Console.WriteLine($"[{report.MessageType} Messages]");
            Console.WriteLine($"FastestDeliveryTime = {Math.Round(report.FastestDeliveryTime / 10000, 2)}ms");
            Console.WriteLine($"AverageDeliveryTime = {Math.Round(report.AverageDeliveryTime / 10000, 2)}ms");
            Console.WriteLine($"SlowestDeliveryTime = {Math.Round(report.SlowestDeliveryTime / 10000, 2)}ms");
            Console.WriteLine($"TotalCount = {report.TotalCount}");
            Console.WriteLine($"IsUnopenedCount = {report.IsUnopenedCount}");
        }
    }
}
