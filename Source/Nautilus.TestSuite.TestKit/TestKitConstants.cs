// -------------------------------------------------------------------------------------------------
// <copyright file="TestKitConstants.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit
{
    using System;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using ServiceStack;

    /// <summary>
    /// Provides constant values to be used in the test suite.
    /// </summary>
    public static class TestKitConstants
    {
        private static readonly Guid TestGuid = Guid.NewGuid();

        /// <summary>
        /// Gets the full path for the test data (/Source/TestData)
        /// </summary>
        public static string TestDataDirectory => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "TestData\\";

        /// <summary>
        /// Gets the test <see cref="Guid"/> for this run of tests.
        /// </summary>
        public static Guid GetTestGuid => TestGuid;

        /// <summary>
        /// Registers the ServiceStack license contained within the config.json.
        /// </summary>
        public static void RegisterServiceStackLicense()
        {
            var nautilusDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "NautilusDB\\";
            var config = JObject.Parse(File.ReadAllText(nautilusDirectory + "config.json"));

            Licensing.RegisterLicense((string)config["serviceStack"]["licenseKey"]);
        }
    }
}
