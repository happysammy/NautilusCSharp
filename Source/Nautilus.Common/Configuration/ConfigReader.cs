//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigReader.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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
