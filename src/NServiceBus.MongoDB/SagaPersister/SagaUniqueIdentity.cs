// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaUniqueIdentity.cs" company="SharkByte Software Inc.">
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
//   Defines the SagaUniqueIdentity type.
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

            // raven has a size limit of 255 bytes == 127 unicode chars
            if (id.Length > 127)
            {
                // generate a guid from the hash:
                var key = Utils.DeterministicGuid.Create(sagaType.FullName, uniqueProperty.Key);

                id = string.Format("MoreThan127/{0}/{1}", key, value);
            }

            return id;
        }
    }
}
