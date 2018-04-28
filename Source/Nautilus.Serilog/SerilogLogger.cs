//--------------------------------------------------------------
// <copyright file="SerilogLogger.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Serilog
{
    using System;
    using System.Reflection;
    using global::Serilog;
    using NautechSystems.CSharp.CQS;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The <see cref="Serilog"/> adapter.
    /// </summary>
    public class SerilogLogger : ILoggingAdapter
    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
//        /// </summary>
//        /// <param name="logDatabaseName">
//        /// The log Database Name.
//        /// </param>
//        public SerilogLogger(string logDatabaseName)
//        {
//            SerilogLogFactory.Create(logDatabaseName);
//
//            this.Information($"Serilog (version {Assembly.LoadFrom("Serilog.dll").GetName().Version})");
//        }

        public string AssemblyVersion =>
            $"Serilog (version {Assembly.LoadFrom("Serilog.dll").GetName().Version})";

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Verbose(Enum service, string message)
        {
            Log.Verbose($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Debug(Enum service, string message)
        {
            Log.Debug($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Information(Enum service, string message)
        {
            Log.Information($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        public void Warning(Enum service, string message)
        {
            Log.Warning($"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public void Error(Enum service, string message, Exception ex)
        {
            Log.Error(ex, $"[{ToOutput(service)}] {message}");
        }

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="message">
        /// The log text.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public void Fatal(Enum service, string message, Exception ex)
        {
            Log.Fatal(ex, $"[{ToOutput(service)}] {message}");
        }

        public void LogResult(ResultBase result)
        {
            throw new NotImplementedException();
        }

        private static string ToOutput(Enum service)
        {
            const int LogStringLength = 10;

            if (service.ToString().Length >= LogStringLength)
            {
                return service.ToString();
            }

            var lengthDifference = LogStringLength - service.ToString().Length;

            var underscoreAppend = string.Empty;
            var builder = new System.Text.StringBuilder();
            builder.Append(underscoreAppend);

            for (int i = 0; i < lengthDifference; i++)
            {
                builder.Append("_");
            }

            underscoreAppend = builder.ToString();

            return service + underscoreAppend;
        }
    }
}
