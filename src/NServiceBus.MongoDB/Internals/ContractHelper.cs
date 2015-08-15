// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContractHelper.cs" company="SharkByte Software">
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
//   Defines the ContractHelpers type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Internals
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The contract helpers.
    /// </summary>
    public static class ContractHelper
    {
        /// <summary>
        /// Helper class to assist with invariant warnings
        /// </summary>
        /// <param name="obj">
        ///  The object to assume the invariants on
        /// </param>
        /// <typeparam name="T">
        /// The type to assume invariants
        /// </typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "obj",
            Justification = "Ok here.")]
        [Pure]
        public static void AssumeInvariant<T>(T obj)
        {
        }

        /// <summary>
        /// Helper class to assist with contracts
        /// </summary>
        /// <param name="obj">
        /// The object to assume is not null
        /// </param>
        /// <typeparam name="T">
        /// The type of assume is not null
        /// </typeparam>
        /// <returns>
        /// The same object
        /// </returns>
        public static T AssumedNotNull<T>(this T obj) where T : class
        {
            Contract.Ensures(Contract.Result<T>() != null);

            Contract.Assume(obj != null);
            return obj;
        }

        /// <summary>
        /// The is null or whitespace checked.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AssumedNotNullOrWhiteSpace(this string value)
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            Contract.Assume(!string.IsNullOrWhiteSpace(value));
            return value;
        }
    }
}
