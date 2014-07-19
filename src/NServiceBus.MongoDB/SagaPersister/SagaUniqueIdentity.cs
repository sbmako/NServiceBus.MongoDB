// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaUniqueIdentity.cs" company="Carlos Sandoval">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Carlos Sandoval
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
//   The saga unique identity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.SagaPersister
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The saga unique identity.
    /// </summary>
    public class SagaUniqueIdentity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the saga id.
        /// </summary>
        public Guid SagaId { get; set; }

        /// <summary>
        /// Gets or sets the unique value.
        /// </summary>
        public object UniqueValue { get; set; }

        /// <summary>
        /// The format id.
        /// </summary>
        /// <param name="sagaType">
        /// The saga type.
        /// </param>
        /// <param name="uniqueProperty">
        /// The unique property.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatId(Type sagaType, KeyValuePair<string, object> uniqueProperty)
        {
            if (uniqueProperty.Value == null)
            {
                throw new ArgumentNullException("uniqueProperty", string.Format("Property {0} is marked with the [Unique] attribute on {1} but contains a null value. Please make sure that all unique properties are set on your SagaData and/or that you have marked the correct properties as unique.", uniqueProperty.Key, sagaType.Name));
            }

            var value = Utils.DeterministicGuid.Create(uniqueProperty.Value.ToString());

            var id = string.Format("{0}/{1}/{2}", sagaType.FullName.Replace('+', '-'), uniqueProperty.Key, value);

            if (id.Length > 1024)
            {
                // generate a guid from the hash:
                var key = Utils.DeterministicGuid.Create(sagaType.FullName, uniqueProperty.Key);

                id = string.Format("MoreThan1024/{0}/{1}", key, value);
            }

            return id;
        }
    }
}
