// -------------------------------------------------------------------------------------------------
// <copyright file="BsonInstrumentSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BsonInstrumentSerializerTests : TestBase
    {
        public BsonInstrumentSerializerTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void CanSerializeAndDeserializeInstrument()
        {
            // Arrange
            var serializer = new InstrumentDataSerializer();
            var instrument = StubInstrumentProvider.AUDUSD();

            // Act
            var packed = serializer.Serialize(instrument);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(instrument, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }
    }
}
