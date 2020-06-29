//--------------------------------------------------------------------------------------------------
// <copyright file="StubAtomicOrderProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Stubs
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public static class StubAtomicOrderProvider
    {
        public static AtomicOrder Create(bool hasTakeProfit = true)
        {
            if (hasTakeProfit)
            {
                return new AtomicOrder(
                    new StubOrderBuilder().WithOrderId("O-123456-1").BuildStopMarketOrder(),
                    new StubOrderBuilder().WithOrderId("O-123456-2").BuildStopMarketOrder(),
                    new StubOrderBuilder().WithOrderId("O-123456-3").BuildLimitOrder());
            }

            return new AtomicOrder(
                new StubOrderBuilder().WithOrderId("O-123456-1").BuildStopMarketOrder(),
                new StubOrderBuilder().WithOrderId("O-123456-2").BuildStopMarketOrder());
        }
    }
}
