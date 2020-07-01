//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarData.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.TestSuite.TestKit.Stubs
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public static class StubBarData
    {
        public static Bar Create(int offsetMinutes = 0)
        {
            return new Bar(
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Volume.Create(1000000),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(offsetMinutes));
        }

        public static Bar Create(Duration offset)
        {
            return new Bar(
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Volume.Create(1000000),
                StubZonedDateTime.UnixEpoch() + offset);
        }
    }
}
