//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderSingleFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    /// <summary>
    /// The new order single.
    /// </summary>
    public static class NewOrderSingleFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <returns>
        /// The <see cref="NewOrderSingleFactory"/>.
        /// </returns>
        public static QuickFix.FIX44.NewOrderSingle Create()
        {
            var message = new QuickFix.FIX44.NewOrderSingle();

            return message;
        }
    }
}
