// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SharkByte Software Inc.">
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
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using NServiceBus;
using NServiceBus.MongoDB;

class Program
{
    static void Main()
    {
        Configure.Serialization.Json();

        var bus = Configure.With()
            .DefaultBuilder()
            .MongoPersistence()
            .CreateBus();

        bus.Start();

        bus.SendLocal(new MyMessage
        {
            SomeId = Guid.NewGuid()
        });

        Console.ReadLine();
    }
}