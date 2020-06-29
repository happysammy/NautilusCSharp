// -------------------------------------------------------------------------------------------------
// <copyright file="RedisConstants.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Redis
{
    /// <summary>
    /// Provides constants for the <see cref="Redis"/> database infrastructure.
    /// </summary>
    public static class RedisConstants
    {
        /// <summary>
        /// Gets the <see cref="Redis"/> localhost constant.
        /// </summary>
        public static string Localhost => "localhost";

        /// <summary>
        /// Gets the <see cref="Redis"/> default port constant.
        /// </summary>
        public static int DefaultPort => 6379;
    }
}
