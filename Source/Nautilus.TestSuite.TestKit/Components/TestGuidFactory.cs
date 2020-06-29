//--------------------------------------------------------------------------------------------------
// <copyright file="TestGuidFactory.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.TestKit.Components
{
    using System;
    using Nautilus.Common.Interfaces;

    /// <inheritdoc/>
    public sealed class TestGuidFactory : IGuidFactory
    {
        private readonly Guid guid = Guid.Parse("3532d5de-f67f-4a8d-9c42-e1002fa6733b");

        /// <inheritdoc/>
        public Guid Generate()
        {
            return this.guid;
        }
    }
}
