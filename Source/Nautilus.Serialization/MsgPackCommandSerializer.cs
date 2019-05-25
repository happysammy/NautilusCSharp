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
    using Nautilus.DomainModel.Entities;
    using Nautilus.Execution.Messages.Commands;

    /// <summary>
    /// Provides a command binary serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackCommandSerializer : ICommandSerializer
    {
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
                { Key.Command, command.Type.Name },
                { Key.CommandId, command.Id.ToString() },
                { Key.CommandTimestamp, command.Timestamp.ToIsoString() },
            };

            switch (command)
            {
                case CancelOrder cmd:
                    package.Add(Key.Order, MsgPackOrderSerializer.Serialize(cmd.Order));
                    package.Add(Key.CancelReason, cmd.Reason);
                    break;
                case ModifyOrder cmd:
                    package.Add(Key.Order, MsgPackOrderSerializer.Serialize(cmd.Order));
                    package.Add(Key.ModifiedPrice, cmd.ModifiedPrice.ToString());
                    break;
                case CollateralInquiry cmd:
                    break;
                case SubmitOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Order, MsgPackOrderSerializer.Serialize(cmd.Order));
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Entry, MsgPackOrderSerializer.Serialize(cmd.AtomicOrder.Entry));
                    package.Add(Key.StopLoss, MsgPackOrderSerializer.Serialize(cmd.AtomicOrder.StopLoss));
                    package.Add(Key.TakeProfit, MsgPackOrderSerializer.SerializeTakeProfit(cmd.AtomicOrder.TakeProfit));
                    package.Add(Key.HasTakeProfit, cmd.HasTakeProfit);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }

            return MsgPackSerializer.Serialize(package);
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
            var command = unpacked[Key.Command].ToString();

            switch (command)
            {
                case nameof(CollateralInquiry):
                    return new CollateralInquiry(commandId, commandTimestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        MsgPackOrderSerializer.Deserialize(unpacked[Key.Order].AsBinary()),
                        unpacked[Key.CancelReason].ToString(),
                        commandId,
                        commandTimestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        MsgPackOrderSerializer.Deserialize(unpacked[Key.Order].AsBinary()),
                        MsgPackObjectConverter.ToPrice(unpacked[Key.ModifiedPrice].ToString()),
                        commandId,
                        commandTimestamp);
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        MsgPackOrderSerializer.Deserialize(unpacked[Key.Order].AsBinary()),
                        MsgPackObjectConverter.ToTraderId(unpacked[Key.TraderId]),
                        MsgPackObjectConverter.ToStrategyId(unpacked[Key.StrategyId]),
                        MsgPackObjectConverter.ToPositionId(unpacked[Key.PositionId]),
                        commandId,
                        commandTimestamp);
                case nameof(SubmitAtomicOrder):
                    return new SubmitAtomicOrder(
                        new AtomicOrder(
                            MsgPackOrderSerializer.Deserialize(unpacked[Key.Entry].AsBinary()),
                            MsgPackOrderSerializer.Deserialize(unpacked[Key.StopLoss].AsBinary()),
                            MsgPackOrderSerializer.DeserializeTakeProfit(
                                unpacked[Key.TakeProfit].AsBinary(),
                                unpacked[Key.HasTakeProfit].AsBoolean())),
                        MsgPackObjectConverter.ToTraderId(unpacked[Key.TraderId]),
                        MsgPackObjectConverter.ToStrategyId(unpacked[Key.StrategyId]),
                        MsgPackObjectConverter.ToPositionId(unpacked[Key.PositionId]),
                        commandId,
                        commandTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }
    }
}
