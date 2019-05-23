// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Identifiers;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Execution.Messages.Commands.Base;
    using NodaTime;

    /// <summary>
    /// Provides a command binary serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackCommandSerializer : MsgPackSerializer, ICommandSerializer
    {
        private const byte NIL = 0xc0;

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
            switch (command)
            {
                case OrderCommand orderCommand:
                    return this.SerializeOrderCommand(orderCommand);
                case CollateralInquiry collateralInquiry:
                    return this.SerializeToMsgPack(new MessagePackObjectDictionary
                    {
                        { Key.CommandType, nameof(CollateralInquiry) },
                        { Key.CommandId, collateralInquiry.Id.ToString() },
                        { Key.CommandTimestamp, collateralInquiry.Timestamp.ToIsoString() },
                    });
                case SubmitAtomicOrder submitOrder:
                    return this.SerializeToMsgPack(new MessagePackObjectDictionary
                    {
                        { Key.CommandType, nameof(SubmitAtomicOrder) },
                        { Key.Entry, this.orderSerializer.Serialize(submitOrder.AtomicOrder.Entry) },
                        { Key.StopLoss, this.orderSerializer.Serialize(submitOrder.AtomicOrder.StopLoss) },
                        { Key.TakeProfit, this.SerializeTakeProfit(submitOrder.AtomicOrder.StopLoss) },
                        { Key.HasTakeProfit, submitOrder.AtomicOrder.HasTakeProfit },
                        { Key.TraderId, submitOrder.TraderId.Value },
                        { Key.StrategyId, submitOrder.StrategyId.Value },
                        { Key.PositionId, submitOrder.PositionId.Value },
                        { Key.CommandId, submitOrder.Id.ToString() },
                        { Key.CommandTimestamp, submitOrder.Timestamp.ToIsoString() },
                    });
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }

        /// <summary>
        /// Deserialize the given Message Pack specification bytes to a command.
        /// </summary>
        /// <param name="commandBytes">The command bytes to deserialize.</param>
        /// <returns>The deserialized command.</returns>
        public Command Deserialize(byte[] commandBytes)
        {
            var unpacked = this.DeserializeFromMsgPack<MessagePackObjectDictionary>(commandBytes);

            var commandId = new Guid(unpacked[Key.CommandId].ToString());
            var commandTimestamp = unpacked[Key.CommandTimestamp].ToString().ToZonedDateTimeFromIso();
            var commandType = unpacked[Key.CommandType].ToString();

            switch (commandType)
            {
                case nameof(OrderCommand):
                    return this.DeserializeOrderCommand(commandId, commandTimestamp, unpacked);
                case nameof(CollateralInquiry):
                    return new CollateralInquiry(commandId, commandTimestamp);
                case nameof(SubmitAtomicOrder):
                    return new SubmitAtomicOrder(
                        new AtomicOrder(
                            this.orderSerializer.Deserialize(unpacked[Key.Entry].AsBinary()),
                            this.orderSerializer.Deserialize(unpacked[Key.StopLoss].AsBinary()),
                            this.DeserializeTakeProfit(
                                unpacked[Key.TakeProfit].AsBinary(),
                                unpacked[Key.HasTakeProfit].AsBoolean())),
                        new TraderId(unpacked[Key.TraderId].ToString()),
                        new StrategyId(unpacked[Key.StrategyId].ToString()),
                        new PositionId(unpacked[Key.PositionId].ToString()),
                        commandId,
                        commandTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(commandType, nameof(commandType));
            }
        }

        private byte[] SerializeTakeProfit(OptionRef<Order> takeProfit)
        {
            return takeProfit.HasValue
                ? this.orderSerializer.Serialize(takeProfit.Value)
                : new[] { NIL };
        }

        private OptionRef<Order> DeserializeTakeProfit(byte[] takeProfit, bool hasTakeProfit)
        {
            return hasTakeProfit
                ? OptionRef<Order>.Some(this.orderSerializer.Deserialize(takeProfit))
                : OptionRef<Order>.None();
        }

        private byte[] SerializeOrderCommand(OrderCommand orderCommand)
        {
            var package = new MessagePackObjectDictionary
            {
                { Key.CommandType, nameof(OrderCommand) },
                { Key.Order, this.orderSerializer.Serialize(orderCommand.Order) },
                { Key.CommandId, orderCommand.Id.ToString() },
                { Key.CommandTimestamp, orderCommand.Timestamp.ToIsoString() },
            };

            switch (orderCommand)
            {
                case SubmitOrder command:
                    package.Add(Key.OrderCommand, nameof(SubmitOrder));
                    package.Add(Key.TraderId, command.TraderId.ToString());
                    package.Add(Key.StrategyId, command.StrategyId.ToString());
                    package.Add(Key.PositionId, command.PositionId.ToString());
                    break;
                case CancelOrder command:
                    package.Add(Key.OrderCommand, nameof(CancelOrder));
                    package.Add(Key.CancelReason, command.Reason);
                    break;
                case ModifyOrder command:
                    package.Add(Key.OrderCommand, nameof(ModifyOrder));
                    package.Add(Key.ModifiedPrice, command.ModifiedPrice.ToString());
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderCommand, nameof(orderCommand));
            }

            return this.SerializeToMsgPack(package);
        }

        private OrderCommand DeserializeOrderCommand(
            Guid commandId,
            ZonedDateTime commandTimestamp,
            MessagePackObjectDictionary unpacked)
        {
            var order = this.orderSerializer.Deserialize(unpacked[Key.Order].AsBinary());
            var orderCommand = unpacked[Key.OrderCommand].ToString();

            switch (orderCommand)
            {
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        order,
                        new TraderId(unpacked[Key.TraderId].ToString()),
                        new StrategyId(unpacked[Key.StrategyId].ToString()),
                        new PositionId(unpacked[Key.PositionId].ToString()),
                        commandId,
                        commandTimestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        order,
                        unpacked[Key.CancelReason].ToString(),
                        commandId,
                        commandTimestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        order,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.ModifiedPrice].ToString()).Value,
                        commandId,
                        commandTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderCommand, nameof(orderCommand));
            }
        }
    }
}
