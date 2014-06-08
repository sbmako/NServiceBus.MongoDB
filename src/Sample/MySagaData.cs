// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySagaData.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   Defines the MySagaData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NServiceBus.Saga;

public class MySagaData : IContainSagaData
{
    public Guid Id { get; set; }
    public string Originator { get; set; }
    public string OriginalMessageId { get; set; }
    public Guid SomeId { get; set; }
}