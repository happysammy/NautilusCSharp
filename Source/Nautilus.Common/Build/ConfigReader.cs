//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigReader.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Build
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Nautilus.Core.Collections;

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
        public static ReadOnlyDictionary<string, string> LoadConfig(string fileName)
        {
            var dic = new Dictionary<string, string>();

            if (!File.Exists(fileName))
            {
                throw new InvalidOperationException($"Cannot load configuration (the file {fileName} was not found).");
            }

            var settings = File.ReadAllLines(fileName);
            for (var i = 0; i < settings.Length; i++)
            {
                var setting = settings[i];
                var index = setting.IndexOf("=", StringComparison.Ordinal);
                if (index >= 0)
                {
                    var skey = setting.Substring(0, index);
                    var svalue = setting.Substring(index + 1);
                    if (!dic.ContainsKey(skey))
                    {
                        dic.Add(skey, svalue);
                    }
                }
            }

            return new ReadOnlyDictionary<string, string>(dic);
        }
    }
}
