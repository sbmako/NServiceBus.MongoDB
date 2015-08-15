// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoTimeoutStorage.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 SharkByte Software
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
//   The mongo DB timeout storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.TimeoutPersister
{
    using NServiceBus.Features;
    using NServiceBus.MongoDB.Internals;

    /// <summary>
    /// The mongo DB timeout storage.
    /// </summary>
    public class MongoTimeoutStorage : Feature
    {
        internal MongoTimeoutStorage()
        {
            this.DependsOn<TimeoutManager>();
            this.DependsOn<MongoDocumentStore>();
        }

        /// <summary>
        /// Called when the features is activated
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Setup(FeatureConfigurationContext context)
        {
            TimeoutClassMaps.ConfigureClassMaps();

            context.Container.ConfigureComponent<MongoTimeoutPersister>(DependencyLifecycle.SingleInstance)
                .ConfigureProperty(x => x.EndpointName, context.Settings.EndpointName());
        }
    }
}
