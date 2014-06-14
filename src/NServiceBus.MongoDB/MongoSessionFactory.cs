// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSessionFactory.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
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

        ////public IDocumentStore Store { get; private set; }

        ////public IDocumentSession Session
        ////{
        ////    get { return session ?? (session = OpenSession()); }
        ////}

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoSessionFactory"/> class.
        /// </summary>
        /// <param name="serverAccessor">
        /// The server accessor.
        /// </param>
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
        public void ReleaseSession()
        {
            ////if (session == null)
            ////{
            ////    return;
            ////}

            ////session.Dispose();
            ////session = null;
        }

        ////IDocumentSession OpenSession()
        ////{
        ////    IMessageContext context = null;

        ////    if (Bus != null)
        ////        context = Bus.CurrentMessageContext;

        ////    var databaseName = GetDatabaseName(context);

        ////    IDocumentSession documentSession;

        ////    if (string.IsNullOrEmpty(databaseName))
        ////        documentSession = Store.OpenSession();
        ////    else
        ////        documentSession = Store.OpenSession(databaseName);

        ////    documentSession.Advanced.AllowNonAuthoritativeInformation = false;
        ////    documentSession.Advanced.UseOptimisticConcurrency = true;

        ////    return documentSession;
        ////}

        public void SaveChanges()
        {
            ////if (session == null)
            ////    return;
            ////try
            ////{
            ////    session.SaveChanges();
            ////}
            ////catch (global::Raven.Abstractions.Exceptions.ConcurrencyException ex)
            ////{                
            ////    throw new ConcurrencyException("A saga with the same Unique property already existed in the storage. See the inner exception for further details", ex);
            ////}
        }

        public static Func<IMessageContext, string> GetDatabaseName = context => String.Empty;
    }
}
