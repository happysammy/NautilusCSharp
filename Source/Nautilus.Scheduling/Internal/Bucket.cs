// -------------------------------------------------------------------------------------------------
// <copyright file="Bucket.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Nautilus.Scheduling.Internal
{
    /// <summary>
    /// Represents a bucket.
    /// </summary>
    internal sealed class Bucket
    {
        private readonly ILogger logger;

        private SchedulerRegistration? head;
        private SchedulerRegistration? tail;
        private SchedulerRegistration? rescheduleHead;
        private SchedulerRegistration? rescheduleTail;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        internal Bucket(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Adds a <see cref="SchedulerRegistration"/> to this bucket.
        /// </summary>
        /// <param name="registration">The scheduler registration.</param>
        internal void AddRegistration(SchedulerRegistration registration)
        {
            registration.Bucket = this;
            if (this.head == null)
            {
                // First time the bucket has been used.
                this.head = this.tail = registration;
            }
            else
            {
                if (this.tail != null)
                {
                    this.tail.Next = registration;
                    registration.Prev = this.tail;
                }

                this.tail = registration;
            }
        }

        /// <summary>
        /// Empty this bucket.
        /// </summary>
        /// <param name="registrations">A set of registrations to populate.</param>
        internal void ClearRegistrations(HashSet<SchedulerRegistration> registrations)
        {
            while (true)
            {
                var reg = this.Poll();
                if (reg == null)
                {
                    return;
                }

                if (reg.Cancelled)
                {
                    continue;
                }

                registrations.Add(reg);
            }
        }

        /// <summary>
        /// Execute all <see cref="SchedulerRegistration"/>s that are due by or after <paramref name="deadline"/>.
        /// </summary>
        /// <param name="deadline">The execution time.</param>
        internal void Execute(long deadline)
        {
            var current = this.head;

            // Process all registrations.
            while (current != null)
            {
                var remove = false;

                // Check for cancellation first.
                if (current.Cancelled)
                {
                    remove = true;
                }
                else if (current.RemainingRounds <= 0)
                {
                    if (current.Deadline <= deadline)
                    {
                        try
                        {
                            // Execute the scheduled work.
                            current.Action.Run();
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogError($"Error while executing scheduled task {current}", ex);
                            var nextErrored = current.Next;
                            this.Remove(current);
                            current = nextErrored;
                            continue; // Don't reschedule any failed actions.
                        }

                        remove = true;
                    }
                    else
                    {
                        // Registration was placed into the wrong bucket. This should never happen.
                        throw new InvalidOperationException(
                            $"SchedulerRegistration.Deadline [{current.Deadline}] > Timer.Deadline [{deadline}]");
                    }
                }
                else
                {
                    current.RemainingRounds--;
                }

                var next = current.Next;
                if (remove)
                {
                    this.Remove(current);
                }

                if (current.Repeat && remove)
                {
                    this.Reschedule(current);
                }

                current = next;
            }
        }

        /// <summary>
        /// Reset the reschedule list for this bucket.
        /// </summary>
        /// <param name="registrations">A set of registrations to populate.</param>
        internal void ClearReschedule(HashSet<SchedulerRegistration> registrations)
        {
            while (true)
            {
                var reg = this.PollReschedule();
                if (reg == null)
                {
                    return;
                }

                if (reg.Cancelled)
                {
                    continue;
                }

                registrations.Add(reg);
            }
        }

        private void Remove(SchedulerRegistration reg)
        {
            var next = reg.Next;

            // Remove work that's already been completed or cancelled.
            // Work that is scheduled to repeat will be handled separately.
            if (reg.Prev != null)
            {
                reg.Prev.Next = next;
            }

            if (reg.Next != null)
            {
                reg.Next.Prev = reg.Prev;
            }

            if (reg == this.head)
            {
                // need to adjust the ends
                if (reg == this.tail)
                {
                    this.tail = null;
                    this.head = null;
                }
                else
                {
                    this.head = next;
                }
            }
            else if (reg == this.tail)
            {
                this.tail = reg.Prev;
            }

            // Detach the node from Linked list so it can be GCed.
            reg.Reset();
        }

        /// <summary>
        /// Slot a repeating task into the "reschedule" linked list.
        /// </summary>
        /// <param name="reg">The registration scheduled for repeating.</param>
        private void Reschedule(SchedulerRegistration reg)
        {
            if (this.rescheduleHead == null)
            {
                this.rescheduleHead = this.rescheduleTail = reg;
            }
            else
            {
                if (this.rescheduleTail != null)
                {
                    this.rescheduleTail.Next = reg;
                    reg.Prev = this.rescheduleTail;
                    this.rescheduleTail = reg;
                }
            }
        }

        private SchedulerRegistration? Poll()
        {
            var thisHead = this.head;
            if (thisHead == null)
            {
                return null;
            }

            var next = thisHead.Next;
            if (next == null)
            {
                this.tail = this.head = null;
            }
            else
            {
                this.head = next;
                next.Prev = null;
            }

            thisHead.Reset();
            return thisHead;
        }

        private SchedulerRegistration? PollReschedule()
        {
            var thisHead = this.rescheduleHead;
            if (thisHead == null)
            {
                return null;
            }

            var next = thisHead.Next;
            if (next == null)
            {
                this.rescheduleTail = this.rescheduleHead = null;
            }
            else
            {
                this.rescheduleHead = next;
                next.Prev = null;
            }

            thisHead.Reset();
            return thisHead;
        }
    }
}
