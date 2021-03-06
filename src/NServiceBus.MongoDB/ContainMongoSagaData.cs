﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainMongoSagaData.cs" company="SharkByte Software">
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
// <summary>
//   Base Helper class for MongoDB saga data
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB
{
    /// <summary>
    /// Base Helper class for MongoDB saga data
    /// </summary>
    public abstract class ContainMongoSagaData : ContainSagaData, IHaveDocumentVersion
    {
        /// <summary>
        /// Gets or sets the document version.
        /// </summary>
        public int DocumentVersion { get; set; } = -1;

        /// <summary>
        /// Gets or sets the document ETag value
        /// </summary>
        public int ETag { get; set; }
    }
}
