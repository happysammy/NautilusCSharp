//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigReader.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Collections.Generic;
using System.IO;

namespace Nautilus.Common.Configuration
{
    /// <summary>
    /// Provides a configuration file reader.
    /// </summary>
    public static class ConfigReader
    {
        /// <summary>
        /// Parse the configuration file with the given name.
        /// </summary>
        /// <param name="fileName">The configuration filename.</param>
        /// <returns>A read-only dictionary of key value pairs.</returns>
        public static IReadOnlyDictionary<string, string> LoadConfig(string fileName)
        {
            var dictionary = new Dictionary<string, string>();

            if (!File.Exists(fileName))
            {
                throw new InvalidOperationException($"Cannot load configuration (the file {fileName} was not found).");
            }

            foreach (var setting in File.ReadAllLines(fileName))
            {
                var index = setting.IndexOf("=", StringComparison.Ordinal);
                if (index < 0)
                {
                    continue;
                }

                var key = setting.Substring(0, index);
                var value = setting.Substring(index + 1);
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value);
                }
            }

            return new Dictionary<string, string>(dictionary);
        }
    }
}
