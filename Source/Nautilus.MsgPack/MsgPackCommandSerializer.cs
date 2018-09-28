// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.MsgPack
{
    using System;
    using global::MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Commands.Base;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Provides a command binary serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackCommandSerializer : ICommandSerializer
    {
        private const string OrderCommand = "order_command";
        private const string SubmitOrder = "submit_order";
        private const string CancelOrder = "cancel_order";
        private const string ModifyOrder = "modify_order";
        private const string CollateralInquiry = "collateral_inquiry";

        private readonly IOrderSerializer orderSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackCommandSerializer"/> class.
        /// </summary>
        public MsgPackCommandSerializer()
        {
            this.orderSerializer = new MsgPackOrderSerializer();
        }

        /// <summary>
        /// Serialize the given command to Message Pack specification bytes.
        /// </summary>
        /// <param name="command">The command to serialize.</param>
        /// <returns>The serialized command.</returns>
        public byte[] Serialize(Command command)
        {
            Debug.NotNull(@command, nameof(@command));

            switch (command)
            {
                case OrderCommand orderCommand:
                    return this.SerializeOrderCommand(orderCommand);

                case CollateralInquiry collateralInquiry:
                    var package = new MessagePackObjectDictionary
                    {
                        { new MessagePackObject(Key.CommandType), CollateralInquiry },
                        { new MessagePackObject(Key.CommandId), collateralInquiry.Id.ToString() },
                        { new MessagePackObject(Key.CommandTimestamp), collateralInquiry.Timestamp.ToIsoString() },
                    };
                    return MsgPackSerializer.Serialize(package.Freeze());

                default: throw new InvalidOperationException(
                    "Cannot serialize the command (unrecognized command).");
            }
        }

        /// <summary>
        /// Deserialize the given Message Pack specification bytes to a command.
        /// </summary>
        /// <param name="commandBytes">The command bytes to deserialize.</param>
        /// <returns>The deserialized command.</returns>
        public Command Deserialize(byte[] commandBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(commandBytes);

            var commandId = new Guid(unpacked[Key.CommandId].ToString());
            var commandTimestamp = unpacked[Key.CommandTimestamp].ToString().ToZonedDateTimeFromIso();

            switch (unpacked[Key.CommandType].ToString())
            {
                case OrderCommand:
                    return this.DeserializeOrderCommand(commandId, commandTimestamp, unpacked);

                case CollateralInquiry:
                    return new CollateralInquiry(commandId, commandTimestamp);

                default: throw new InvalidOperationException(
                    "Cannot deserialize the command (unrecognized command).");
            }
        }

        private byte[] SerializeOrderCommand(OrderCommand orderCommand)
        {
            var package = new MessagePackObjectDictionary
            {
                { new MessagePackObject(Key.CommandType), OrderCommand },
                { new MessagePackObject(Key.Order), Hex.ToHexString(this.orderSerializer.Serialize(orderCommand.Order)) },
                { new MessagePackObject(Key.CommandId), orderCommand.Id.ToString() },
                { new MessagePackObject(Key.CommandTimestamp), orderCommand.Timestamp.ToIsoString() },
            };

            switch (orderCommand)
            {
                case SubmitOrder command:
                    package.Add(new MessagePackObject(Key.OrderCommand), SubmitOrder);
                    break;

                case CancelOrder command:
                    package.Add(new MessagePackObject(Key.OrderCommand), CancelOrder);
                    package.Add(new MessagePackObject(Key.CancelReason), command.Reason);
                    break;

                case ModifyOrder command:
                    package.Add(new MessagePackObject(Key.OrderCommand), ModifyOrder);
                    package.Add(new MessagePackObject(Key.ModifiedPrice), command.ModifiedPrice.ToString());
                    break;

                default: throw new InvalidOperationException(
                    "Cannot serialize the order command (unrecognized order command).");
            }

            return MsgPackSerializer.Serialize(package.Freeze());
        }

        private OrderCommand DeserializeOrderCommand(
            Guid commandId,
            ZonedDateTime commandTimestamp,
            MessagePackObjectDictionary unpacked)
        {
            var order = this.orderSerializer.Deserialize(
                Hex.FromHexString(unpacked[Key.Order].ToString()));

            switch (unpacked[Key.OrderCommand].ToString())
            {
                case SubmitOrder:
                    return new SubmitOrder(
                        order,
                        commandId,
                        commandTimestamp);

                case CancelOrder:
                    return new CancelOrder(
                        order,
                        unpacked[Key.CancelReason].ToString(),
                        commandId,
                        commandTimestamp);

                case ModifyOrder:
                    return new ModifyOrder(
                        order,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.ModifiedPrice].ToString()).Value,
                        commandId,
                        commandTimestamp);

                default: throw new InvalidOperationException(
                    "Cannot deserialize the order command (unrecognized order command).");
            }
        }
    }
}
