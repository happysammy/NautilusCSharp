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
            var package = new MessagePackObjectDictionary
            {
                { Key.Type, nameof(Command) },
                { Key.CommandId, command.Id.ToString() },
                { Key.CommandTimestamp, command.Timestamp.ToIsoString() },
            };

            switch (command)
            {
                case CancelOrder cmd:
                    package.Add(Key.Command, nameof(CancelOrder));
                    package.Add(Key.Order, this.orderSerializer.Serialize(cmd.Order));
                    package.Add(Key.CancelReason, cmd.Reason);
                    break;
                case ModifyOrder cmd:
                    package.Add(Key.Command, nameof(ModifyOrder));
                    package.Add(Key.Order, this.orderSerializer.Serialize(cmd.Order));
                    package.Add(Key.ModifiedPrice, cmd.ModifiedPrice.ToString());
                    break;
                case CollateralInquiry cmd:
                    package.Add(Key.Command, nameof(CollateralInquiry));
                    break;
                case SubmitOrder cmd:
                    package.Add(Key.Command, nameof(SubmitOrder));
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Order, this.orderSerializer.Serialize(cmd.Order));
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(Key.Command, nameof(SubmitAtomicOrder));
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Entry, this.orderSerializer.Serialize(cmd.AtomicOrder.Entry));
                    package.Add(Key.StopLoss, this.orderSerializer.Serialize(cmd.AtomicOrder.StopLoss));
                    package.Add(Key.TakeProfit, this.SerializeTakeProfit(cmd.AtomicOrder.TakeProfit));
                    package.Add(Key.HasTakeProfit, cmd.HasTakeProfit);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }

            return SerializeToMsgPack(package);
        }

        /// <summary>
        /// Deserialize the given Message Pack specification bytes to a command.
        /// </summary>
        /// <param name="commandBytes">The command bytes to deserialize.</param>
        /// <returns>The deserialized command.</returns>
        public Command Deserialize(byte[] commandBytes)
        {
            var unpacked = DeserializeFromMsgPack<MessagePackObjectDictionary>(commandBytes);

            var commandId = new Guid(unpacked[Key.CommandId].ToString());
            var commandTimestamp = unpacked[Key.CommandTimestamp].ToString().ToZonedDateTimeFromIso();
            var command = unpacked[Key.Command].ToString();

            switch (command)
            {
                case nameof(CollateralInquiry):
                    return new CollateralInquiry(commandId, commandTimestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        this.orderSerializer.Deserialize(unpacked[Key.Order].AsBinary()),
                        unpacked[Key.CancelReason].ToString(),
                        commandId,
                        commandTimestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        this.orderSerializer.Deserialize(unpacked[Key.Order].AsBinary()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.ModifiedPrice].ToString()),
                        commandId,
                        commandTimestamp);
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        this.orderSerializer.Deserialize(unpacked[Key.Order].AsBinary()),
                        new TraderId(unpacked[Key.TraderId].ToString()),
                        new StrategyId(unpacked[Key.StrategyId].ToString()),
                        new PositionId(unpacked[Key.PositionId].ToString()),
                        commandId,
                        commandTimestamp);
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
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }

        private byte[] SerializeTakeProfit(Order? takeProfit)
        {
            return takeProfit != null
                ? this.orderSerializer.Serialize(takeProfit)
                : new[] { NIL };
        }

        private Order? DeserializeTakeProfit(byte[] takeProfit, bool hasTakeProfit)
        {
            return hasTakeProfit
                ? this.orderSerializer.Deserialize(takeProfit)
                : null;
        }
    }
}
