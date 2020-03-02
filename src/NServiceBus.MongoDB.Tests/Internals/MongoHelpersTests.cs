﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoHelpersTests.cs" company="SharkByte Software">
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

namespace NServiceBus.MongoDB.Tests.Internals
{
    using FluentAssertions;

    using NServiceBus.MongoDB.Internals;
    using NServiceBus.Settings;

    using Xunit;

    public class MongoHelpersTests
    {
        [Fact]
        public void GetConnectionUsingSettingsFromConnectionString()
        {
            var settings = new SettingsHolder();
            settings.Set(MongoPersistenceConstants.ConnectionStringKey, "mongodb://root:mongoR0cks@localhost:27017");
            var readonlySettings = (ReadOnlySettings)settings;

            var result = MongoHelpers.GetConnectionString(readonlySettings);
            result.Should().Be("mongodb://root:mongoR0cks@localhost:27017");
        }
    }
}
