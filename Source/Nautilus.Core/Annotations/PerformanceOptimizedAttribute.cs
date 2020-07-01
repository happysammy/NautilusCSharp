//--------------------------------------------------------------------------------------------------
// <copyright file="PerformanceOptimizedAttribute.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Core.Annotations
{
    /// <summary>
    /// This decorative attribute indicates that the annotated method has been performance
    /// optimized, therefore there is a tendency towards low level implementations over
    /// typical OO abstractions as follows;
    /// - primitive types
    /// - concrete types (avoiding interface dispatch overhead)
    /// - arrays
    /// - for loops
    ///
    /// To meet its design specifications of performance as a priority, avoid refactoring of this
    /// class, struct or method towards increasing OO abstractions for code readability and
    /// understanding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PerformanceOptimizedAttribute : Attribute
    {
    }
}
