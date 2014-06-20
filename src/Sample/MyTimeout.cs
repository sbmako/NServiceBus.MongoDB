// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyTimeout.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 Carlos Sandoval. All rights reserved.
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Affero General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Affero General Public License for more details.
//   
//   You should have received a copy of the GNU Affero General Public License
//   along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
//   The my timeout.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sample
{
    using NServiceBus;

    /// <summary>
    /// The my timeout.
    /// </summary>
    public class MyTimeout : IMessage
    {
        /// <summary>
        /// Gets or sets the how long.
        /// </summary>
        public int HowLong { get; set; }
    }
}
