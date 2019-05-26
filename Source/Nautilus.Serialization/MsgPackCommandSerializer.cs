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
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a command serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackCommandSerializer : ICommandSerializer
    {
        /// <inheritdoc />
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
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.OrderId, cmd.OrderId.ToString());
                    package.Add(Key.Reason, cmd.Reason);
                    break;
                case ModifyOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.OrderId, cmd.OrderId.ToString());
                    package.Add(Key.ModifiedPrice, cmd.ModifiedPrice.ToString());
                    break;
                case CollateralInquiry cmd:
                    break;
                case SubmitOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Order, OrderSerializer.Serialize(cmd.Order));
                    package.Add(Key.InitEventGuid, cmd.InitEventGuid.ToString());
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Entry, OrderSerializer.SerializeEntry(cmd.AtomicOrder));
                    package.Add(Key.StopLoss, OrderSerializer.SerializeStopLoss(cmd.AtomicOrder));
                    package.Add(Key.TakeProfit, OrderSerializer.SerializeTakeProfit(cmd.AtomicOrder));
                    package.Add(Key.InitEventGuidEntry, cmd.InitEventGuidEntry.ToString());
                    package.Add(Key.InitEventGuidStopLoss, cmd.InitEventGuidStopLoss.ToString());
                    package.Add(Key.InitEventGuidTakeProfit, cmd.InitEventGuidTakeProfit.ToString());
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }

            return MsgPackSerializer.Serialize(package);
        }

        /// <inheritdoc />
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
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        unpacked[Key.Reason].ToString(),
                        commandId,
                        commandTimestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.Price(unpacked[Key.ModifiedPrice].ToString()),
                        commandId,
                        commandTimestamp);
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.PositionId(unpacked[Key.PositionId]),
                        OrderSerializer.Deserialize(unpacked[Key.Order].AsBinary(), ObjectExtractor.Guid(unpacked[Key.InitEventGuid])),
                        ObjectExtractor.Guid(unpacked[Key.InitEventGuid]),
                        commandId,
                        commandTimestamp);
                case nameof(SubmitAtomicOrder):
                    return new SubmitAtomicOrder(
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.PositionId(unpacked[Key.PositionId]),
                        new AtomicOrder(
                            OrderSerializer.DeserializeEntry(unpacked),
                            OrderSerializer.DeserializeStopLoss(unpacked),
                            OrderSerializer.DeserializeTakeProfit(unpacked)),
                        commandId,
                        commandTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }
    }
}
