// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyMessage.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the MyMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NServiceBus;

public class MyMessage:IMessage
{
    public Guid SomeId { get; set; }
}