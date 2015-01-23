// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureMongoSubscriptionPersistence.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Carlos Sandoval
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SubscriptionPersister
{
    using global::MongoDB.Bson.Serialization;
    using NServiceBus.Unicast.Subscriptions;

    /// <summary>
    /// The configure MongoDB subscription storage.
    /// </summary>
    public static class ConfigureMongoSubscriptionPersistence
    {
        internal static void ConfigureClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(MessageType)))
            {
                ConfigureMessageTypeClassMap();
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Address)))
            {
                ConfigureAddressClassMap();
            }
        }

        private static void ConfigureAddressClassMap()
        {
            BsonClassMap.RegisterClassMap<Address>(
                cm =>
                    {
                        cm.AutoMap();
                        cm.MapCreator(a => new Address(a.Queue, a.Machine));
                    });
        }

        private static void ConfigureMessageTypeClassMap()
        {
            BsonClassMap.RegisterClassMap<MessageType>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(m => new MessageType(m.TypeName, m.Version));
            });            
        }
    }
}
