// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoGridFSDataBusTests.cs" company="SharkByte Software">
//   The MIT License (MIT)
//   
//   Copyright (c) 2016 SharkByte Software
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

namespace NServiceBus.MongoDB.Tests.DataBus
{
    using System;
    using System.IO;

    using CategoryTraits.Xunit2;

    using FluentAssertions;

    using global::MongoDB.Bson;
    using global::MongoDB.Driver.GridFS;

    using NServiceBus.MongoDB.DataBus;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.Tests.TestingUtilities;

    using Xunit;

    public class MongoGridFsDataBusTests
    {
        [Theory, IntegrationTest]
        [AutoDatabase]
        public void PutTest(MongoDatabaseFactory factory, byte[] data)
        {
            var sut = new MongoGridFSDataBus(factory);

            var input = new MemoryStream(data);
            var key = sut.Put(input, TimeSpan.FromDays(1)).Result;

            var gridFsBucket = new GridFSBucket(factory.GetDatabase());

            var result = gridFsBucket.DownloadAsBytes(ObjectId.Parse(key));

            result.Length.Should().Be(data.Length);
            result.Should().BeEquivalentTo(data);
        }

        [Theory, IntegrationTest]
        [AutoDatabase]
        public void GetTest(MongoDatabaseFactory factory, byte[] data)
        {

            var gridFsBucket = new GridFSBucket(factory.GetDatabase());
            var key = gridFsBucket.UploadFromBytes(Guid.NewGuid().ToString(), data);

            var sut = new MongoGridFSDataBus(factory);
            var resultStream = sut.Get(key.ToString()).Result;

            var result = new byte[data.Length];
            resultStream.Read(result, 0, data.Length);

            resultStream.Length.Should().Be(data.Length);
            result.Should().BeEquivalentTo(data);
        }
    }
}
