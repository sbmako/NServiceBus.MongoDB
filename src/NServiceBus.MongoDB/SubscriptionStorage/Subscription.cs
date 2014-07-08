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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using NServiceBus.MongoDB.Utils;
    using NServiceBus.Unicast.Subscriptions;

    /// <summary>
    /// The subscription.
    /// </summary>
    public sealed class Subscription : IHaveDocumentVersion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        public Subscription()
        {
            this.Id = string.Empty;
            this.DocumentVersion = 0;
            this.MessageType = new MessageType(typeof(object));
            this.Clients = new List<Address>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="clients">
        /// The clients.
        /// </param>
        public Subscription(MessageType messageType, IEnumerable<Address> clients)
        {
            Contract.Requires<ArgumentNullException>(messageType != null);
            Contract.Requires<ArgumentNullException>(clients != null);

            this.Id = FormatId(messageType);
            this.MessageType = messageType;
            this.Clients = clients.ToList();
        }

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

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.Id != null);
            Contract.Invariant(this.MessageType != null);
            Contract.Invariant(this.Clients != null);
        }
    }
}
