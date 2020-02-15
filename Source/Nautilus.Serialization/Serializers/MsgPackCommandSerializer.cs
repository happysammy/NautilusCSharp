// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MessagePack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Commands;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Serialization.Internal;

#pragma warning disable CS8604
    /// <summary>
    /// Provides a <see cref="Command"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackCommandSerializer : IMessageSerializer<Command>
    {
        private readonly IdentifierCache identifierCache;
        private readonly OrderSerializer orderSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackCommandSerializer"/> class.
        /// </summary>
        public MsgPackCommandSerializer()
        {
            this.identifierCache = new IdentifierCache();
            this.orderSerializer = new OrderSerializer();
        }

        /// <inheritdoc />
        public byte[] Serialize(Command command)
        {
            var package = new Dictionary<string, object>
            {
                { nameof(Command.Type), command.Type.Name },
                { nameof(Command.Id), command.Id.ToString() },
                { nameof(Command.Timestamp), command.Timestamp.ToIsoString() },
            };

            switch (command)
            {
                case AccountInquiry cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    break;
                case SubmitOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.StrategyId), cmd.StrategyId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.PositionId), cmd.PositionId.Value);
                    package.Add(nameof(cmd.Order), this.orderSerializer.Serialize(cmd.Order));
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.StrategyId), cmd.StrategyId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.PositionId), cmd.PositionId.Value);
                    package.Add(nameof(cmd.AtomicOrder.Entry), this.orderSerializer.Serialize(cmd.AtomicOrder.Entry));
                    package.Add(nameof(cmd.AtomicOrder.StopLoss), this.orderSerializer.Serialize(cmd.AtomicOrder.StopLoss));
                    package.Add(nameof(cmd.AtomicOrder.TakeProfit), this.orderSerializer.SerializeNullable(cmd.AtomicOrder.TakeProfit));
                    break;
                case ModifyOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.OrderId), cmd.OrderId.Value);
                    package.Add(nameof(cmd.ModifiedQuantity), cmd.ModifiedQuantity.ToString());
                    package.Add(nameof(cmd.ModifiedPrice), cmd.ModifiedPrice.ToString());
                    break;
                case CancelOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.OrderId), cmd.OrderId.Value);
                    package.Add(nameof(cmd.CancelReason), cmd.CancelReason);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Command Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, object>>(dataBytes);

            var command = unpacked[nameof(Command.Type)].ToString();
            var id = ObjectExtractor.AsGuid(unpacked[nameof(Command.Id)]);
            var timestamp = unpacked[nameof(Command.Timestamp)].ToString().ToZonedDateTimeFromIso();

            switch (command)
            {
                case nameof(AccountInquiry):
                    return new AccountInquiry(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        id,
                        timestamp);
                case nameof(SubmitOrder):
                    byte[] orderBytes;
                    try
                    {
                        orderBytes = (byte[])unpacked[nameof(SubmitOrder.Order)];
                    }
                    catch (InvalidCastException)
                    {
                        var orderMapTempFix = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(dataBytes);
                        orderBytes = orderMapTempFix[nameof(SubmitOrder.Order)];
                    }

                    return new SubmitOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        this.identifierCache.StrategyId(unpacked),
                        ObjectExtractor.AsPositionId(unpacked),
                        this.orderSerializer.Deserialize(orderBytes),
                        id,
                        timestamp);
                case nameof(SubmitAtomicOrder):
                    byte[] entryOrderBytes;
                    byte[] stopLossOrderBytes;
                    byte[] profitTargetBytes;
                    try
                    {
                        entryOrderBytes = (byte[])unpacked[nameof(AtomicOrder.Entry)];
                        stopLossOrderBytes = (byte[])unpacked[nameof(AtomicOrder.StopLoss)];
                        profitTargetBytes = (byte[])unpacked[nameof(AtomicOrder.TakeProfit)];
                    }
                    catch (InvalidCastException)
                    {
                        var orderMapTempFix = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(dataBytes);
                        entryOrderBytes = orderMapTempFix[nameof(AtomicOrder.Entry)];
                        stopLossOrderBytes = orderMapTempFix[nameof(AtomicOrder.StopLoss)];
                        profitTargetBytes = orderMapTempFix[nameof(AtomicOrder.TakeProfit)];
                    }

                    return new SubmitAtomicOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        this.identifierCache.StrategyId(unpacked),
                        ObjectExtractor.AsPositionId(unpacked),
                        new AtomicOrder(
                            this.orderSerializer.Deserialize(entryOrderBytes),
                            this.orderSerializer.Deserialize(stopLossOrderBytes),
                            this.orderSerializer.DeserializeNullable(profitTargetBytes)),
                        id,
                        timestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        ObjectExtractor.AsOrderId(unpacked),
                        ObjectExtractor.AsQuantity(unpacked[nameof(ModifyOrder.ModifiedQuantity)]),
                        ObjectExtractor.AsPrice(unpacked[nameof(ModifyOrder.ModifiedPrice)]),
                        id,
                        timestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        ObjectExtractor.AsOrderId(unpacked),
                        unpacked[nameof(CancelOrder.CancelReason)].ToString(),
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }
    }
}
