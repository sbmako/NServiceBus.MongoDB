// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoGridFSDataBusTests.cs" company="Carlos Sandoval">
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
// --------------------------------------------------------------------------------------------------------------------

namespace NServiceBus.MongoDB.Tests.DataBus
{
    using System;
    using System.IO;
    using System.Text;

    using FluentAssertions;

    using NServiceBus.MongoDB.DataBus;
    using NServiceBus.MongoDB.Internals;
    using NServiceBus.MongoDB.Tests.TestingUtilities;

    using Xunit.Extensions;

    public class MongoGridFsDataBusTests
    {
        [Theory, UnitTest]
        [AutoConfigureData]
        public void PutTest(MongoGridFSDataBus sut, byte[] data)
        {
            var input = new MemoryStream(data);
            var key = sut.Put(input, TimeSpan.FromDays(1));

            var result = sut.Get(key);
            result.Length.Should().Be(input.Length);
        }
    }
}
