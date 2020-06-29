//--------------------------------------------------------------------------------------------------
// <copyright file="FileManager.cs" company="Nautech Systems Pty Ltd">
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
    using System.IO;

    /// <summary>
    /// Provides a builder for required system directories and files.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Move all files from the given file path to the given destination directory.
        /// Note any directories and files at the destination will be overwritten.
        /// </summary>
        /// <param name="filePath">The path to the file to move.</param>
        /// <param name="destinationPath">The path to move the files to.</param>
        public static void Copy(string filePath, string destinationPath)
        {
            Directory.CreateDirectory(destinationPath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No file found at {filePath}.");
            }

            var file = Path.GetFileName(filePath);
            var destFile = Path.Combine(destinationPath, file);

            File.Copy(filePath, destFile, true);
        }

        /// <summary>
        /// Move all files from the given file path to the given destination directory.
        /// Note any directories and files at the destination will be overwritten.
        /// </summary>
        /// <param name="filesPath">The path to the files to move.</param>
        /// <param name="destinationPath">The path to move the files to.</param>
        public static void CopyAll(string filesPath, string destinationPath)
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

                var destFile = Path.Combine(destinationPath, fileName);
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
