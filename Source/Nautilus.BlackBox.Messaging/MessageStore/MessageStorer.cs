//--------------------------------------------------------------
// <copyright file="MessageStorer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Messaging.MessageStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Messaging.Base;

    /// <summary>
    /// The sealed <see cref="MessageStorer"/> class. Sends received messages to the
    /// <see cref="MessageWarehouse"/>.
    /// </summary>
    public sealed class MessageStorer : ReceiveActor
    {
        private readonly MessageWarehouse warehouse;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStorer"/> class.
        /// </summary>
        /// <param name="warehouse">The warehouse.</param>
        /// <exception cref="ValidationException">Throws if the warehouse is null.</exception>
        public MessageStorer(MessageWarehouse warehouse)
        {
            Validate.NotNull(warehouse, nameof(warehouse));

            this.warehouse = warehouse;

            this.SetupMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="Message"/> handling methods.
        /// </summary>
        private void SetupMessageHandling()
        {
            this.Receive<Envelope<CommandMessage>>(envelope => this.warehouse.Store(envelope));
            this.Receive<Envelope<EventMessage>>(envelope => this.warehouse.Store(envelope));
            this.Receive<Envelope<DocumentMessage>>(envelope => this.warehouse.Store(envelope));
        }

        /// <summary>
        /// Runs when the inherited <see cref="ReceiveActor"/> stops.
        /// </summary>
        protected override void PostStop()
        {
            var commandReport = GetDiagnosticReport(this.warehouse.CommandEnvelopes);
            var eventReport = GetDiagnosticReport(this.warehouse.EventEnvelopes);
            var serviceReport = GetDiagnosticReport(this.warehouse.DocumentEnvelopes);

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("        Messaging Service - Diagnostics Report " + DateTime.UtcNow);
            Console.WriteLine("-----------------------------------------------------------------------------");

            PrintReportToConsole(commandReport);
            PrintReportToConsole(eventReport);
            PrintReportToConsole(serviceReport);
        }

        private static MessagingDiagnosticsReport GetDiagnosticReport<T>(IReadOnlyCollection<Envelope<T>> envelopeList) where T : Message
        {
            Debug.NotNull(envelopeList, nameof(envelopeList));

            var messageType = typeof(T).Name;

            var totalCount = envelopeList.Count;

            var isUnopenedCount = envelopeList.Count(e => e.OpenedTime.HasNoValue);

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

        private static int GetDeliveryTime<T>(Envelope<T> envelope) where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            return envelope.OpenedTime.HasValue
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