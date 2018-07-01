// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseNameExtensions.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2018 SharkByte Software
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
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Extensions
{
    using System.Diagnostics.Contracts;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.Settings;

    internal static class DatabaseNameExtensions
    {
        /// <summary>
        /// The endpoint name as database name.
        /// </summary>
        /// <param name="endpointName">
        /// The endpoint name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AsValidDatabaseName(this string endpointName)
        {
            Contract.Requires(endpointName != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return endpointName.Replace('.', '_').AssumedNotNull();
        }

        /// <summary>
        /// The database name from endpoint name.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DatabaseNameFromEndpointName(this SettingsHolder settings)
        {
            Contract.Requires(settings != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return settings.EndpointName().AssumedNotNull().AsValidDatabaseName();
        }
    }
}
