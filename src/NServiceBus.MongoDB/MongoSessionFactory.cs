// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSessionFactory.cs" company="SharkByte Software Inc.">
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
//   The mongo session factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    using System;
    using global::MongoDB.Driver;
    using NServiceBus.Persistence;

    /// <summary>
    /// The mongo session factory.
    /// </summary>
    public class MongoSessionFactory
    {
        ////[ThreadStatic]
        ////static MongoClient session;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "TBD")]
        private static Func<IMessageContext, string> getDatabaseName = context => string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSessionFactory"/> class.
        /// </summary>
        /// <param name="serverAccessor">
        /// The server accessor.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "serverAccessor", Justification = "TBD")]
        public MongoSessionFactory(ServerAccessor serverAccessor)
        {
            ////Store = storeAccessor.Store;
        }

        /// <summary>
        /// Gets or sets the bus.
        /// </summary>
        public IBus Bus { get; set; }

        /// <summary>
        /// The release session.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TBD")]
        public void ReleaseSession()
        {
        }

        /// <summary>
        /// The save changes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "TBD")]
        public void SaveChanges()
        {
        }
    }
}
