// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="SharkByte Software Inc.">
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
//   Defines the Subscription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SubscriptionStorage
{
    using System.Collections.Generic;
    using NServiceBus.MongoDB.Utils;
    using NServiceBus.Unicast.Subscriptions;

    /// <summary>
    /// The subscription.
    /// </summary>
    public class Subscription : IHaveDocumentVersion
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the document version.
        /// </summary>
        public int DocumentVersion { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        public List<Address> Clients { get; set; }

        /// <summary>
        /// The format id.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatId(MessageType messageType)
        {
            var id = DeterministicGuid.Create(messageType.TypeName, "/", messageType.Version.Major);
            return string.Format("Subscriptions/{0}", id);
        }
    }
}
