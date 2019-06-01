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
    /// Provides a <see cref="Command"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackCommandSerializer : ICommandSerializer
    {
        /// <inheritdoc />
        public byte[] Serialize(Command command)
        {
            var package = new MessagePackObjectDictionary
            {
                { Key.Type, nameof(Command) },
                { Key.Command, command.Type.Name },
                { Key.Id, command.Id.ToString() },
                { Key.Timestamp, command.Timestamp.ToIsoString() },
            };

            switch (command)
            {
                case CollateralInquiry cmd:
                    break;
                case SubmitOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Order, OrderSerializer.Serialize(cmd.Order));
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.PositionId, cmd.PositionId.ToString());
                    package.Add(Key.Entry, OrderSerializer.Serialize(cmd.AtomicOrder.Entry));
                    package.Add(Key.StopLoss, OrderSerializer.Serialize(cmd.AtomicOrder.StopLoss));
                    package.Add(Key.TakeProfit, OrderSerializer.SerializeNullable(cmd.AtomicOrder.TakeProfit));
                    break;
                case ModifyOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.OrderId, cmd.OrderId.ToString());
                    package.Add(Key.ModifiedPrice, cmd.ModifiedPrice.ToString());
                    break;
                case CancelOrder cmd:
                    package.Add(Key.TraderId, cmd.TraderId.ToString());
                    package.Add(Key.StrategyId, cmd.StrategyId.ToString());
                    package.Add(Key.OrderId, cmd.OrderId.ToString());
                    package.Add(Key.CancelReason, cmd.CancelReason);
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

            var identifier = new Guid(unpacked[Key.Id].ToString());
            var timestamp = unpacked[Key.Timestamp].ToString().ToZonedDateTimeFromIso();
            var command = unpacked[Key.Command].ToString();

            switch (command)
            {
                case nameof(CollateralInquiry):
                    return new CollateralInquiry(identifier, timestamp);
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.PositionId(unpacked[Key.PositionId]),
                        OrderSerializer.Deserialize(unpacked[Key.Order].AsBinary()),
                        identifier,
                        timestamp);
                case nameof(SubmitAtomicOrder):
                    return new SubmitAtomicOrder(
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.PositionId(unpacked[Key.PositionId]),
                        new AtomicOrder(
                            OrderSerializer.Deserialize(unpacked[Key.Entry].AsBinary()),
                            OrderSerializer.Deserialize(unpacked[Key.StopLoss].AsBinary()),
                            OrderSerializer.DeserializeNullable(unpacked[Key.TakeProfit].AsBinary())),
                        identifier,
                        timestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.Price(unpacked[Key.ModifiedPrice].ToString()),
                        identifier,
                        timestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        ObjectExtractor.TraderId(unpacked[Key.TraderId]),
                        ObjectExtractor.StrategyId(unpacked[Key.StrategyId]),
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        unpacked[Key.CancelReason].ToString(),
                        identifier,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }
    }
}
