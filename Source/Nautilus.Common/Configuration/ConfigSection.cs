//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigSection.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Common.Configuration
{
    /// <summary>
    /// Provides JSON configuration section strings.
    /// </summary>
    public static class ConfigSection
    {
        /// <summary>
        /// Gets the logging configuration section string.
        /// </summary>
        public static string Logging { get; } = nameof(Logging);

        /// <summary>
        /// Gets the messaging configuration section string.
        /// </summary>
        public static string Messaging { get; } = nameof(Messaging);

        /// <summary>
        /// Gets the logging configuration section string.
        /// </summary>
        public static string Network { get; } = nameof(Network);

        /// <summary>
        /// Gets the FIX configuration section string.
        /// </summary>
        // ReSharper disable once InconsistentNaming (correct name)
        public static string FIX44 { get; } = nameof(FIX44);

        /// <summary>
        /// Gets the database configuration section string.
        /// </summary>
        public static string Data { get; } = nameof(Data);
    }
}
