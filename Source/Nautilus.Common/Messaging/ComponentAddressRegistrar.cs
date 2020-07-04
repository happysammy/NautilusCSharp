// -------------------------------------------------------------------------------------------------
// <copyright file="ComponentAddressRegistrar.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using Nautilus.Common.Componentry;
using Nautilus.Messaging;
using Nautilus.Messaging.Interfaces;

namespace Nautilus.Common.Messaging
{
    /// <summary>
    /// Provides a means of creating a dictionary of component addresses and endpoints.
    /// </summary>
    public class ComponentAddressRegistrar
    {
        private readonly Dictionary<Address, IEndpoint> addressBook;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAddressRegistrar"/> class.
        /// </summary>
        public ComponentAddressRegistrar()
        {
            this.addressBook = new Dictionary<Address, IEndpoint>();
        }

        /// <summary>
        /// Register the given messaging component.
        /// </summary>
        /// <param name="component">The component to register.</param>
        public void Register(MessagingComponent component)
        {
            this.addressBook.Add(new Address(component.GetType().Name), component.Endpoint);
        }

        /// <summary>
        /// Return the book of registered component addresses and endpoints.
        /// </summary>
        /// <returns>The address book.</returns>
        public Dictionary<Address, IEndpoint> GetAddressBook() => this.addressBook;
    }
}
