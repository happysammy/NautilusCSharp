//--------------------------------------------------------------------------------------------------
// <copyright file="MessageTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using Nautilus.Fxcm.MessageFactories;
using Nautilus.TestSuite.TestKit.Stubs;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.AdapterTests.Fxcm
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageTests
    {
        [Fact]
        internal void MarketDataRequestMessage()
        {
            // Arrange
            // Act
            var message = MarketDataRequestFactory.Create("AUD/USD", 1, StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("8=FIX.4.49=6735=V262=MD_0263=1264=1265=0267=2269=0269=1146=155=AUD/USD10=238", message.ToString());
        }
    }
}
