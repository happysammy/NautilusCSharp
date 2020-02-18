//--------------------------------------------------------------------------------------------------
// <copyright file="FileSystemBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Configuration
{
    using System.IO;

    /// <summary>
    /// Provides a builder for required system directories and files.
    /// </summary>
    public static class FileSystemBuilder
    {
        /// <summary>
        /// Move all files from the given file path to the given destination directory.
        /// Note any directories and files at the destination will be overwritten.
        /// </summary>
        /// <param name="filesPath">The path to the files to move.</param>
        /// <param name="destinationPath">The path to move the files to.</param>
        public static void MoveAll(string filesPath, string destinationPath)
        {
            Directory.CreateDirectory(destinationPath);

            if (!Directory.Exists(filesPath))
            {
                throw new DirectoryNotFoundException($"No directory found at {filesPath}.");
            }

            foreach (var file in Directory.GetFiles(filesPath))
            {
                var fileName = Path.GetFileName(file);
                if (fileName is null)
                {
                    continue;
                }

                var destFile = System.IO.Path.Combine(destinationPath, fileName);
                File.Copy(file, destFile, true);
            }
        }

        /// <summary>
        /// Return the path to the first file found a the given directory with the given extension.
        /// </summary>
        /// <param name="directoryPath">The path to file directory.</param>
        /// <param name="extension">The extension for the file.</param>
        /// <returns>The path to the file.</returns>
        public static string GetFirstFilename(string directoryPath, string extension)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"No directory found at {directoryPath}.");
            }

            foreach (var file in Directory.GetFiles(directoryPath))
            {
                var fileName = Path.GetFileName(file);
                if (fileName is null)
                {
                    continue;
                }

                if (fileName.EndsWith(extension))
                {
                    return Path.Combine(directoryPath, fileName);
                }
            }

            throw new FileNotFoundException($"No .{extension} type files found at {directoryPath}.");
        }
    }
}
