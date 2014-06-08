// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigErrorQueue.cs" company="SharkByte Software Inc.">
//   Copyright (c) 2014 SharkByte Software Inc. All rights reserved.
// </copyright>
// <summary>
//   The config error queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

/// <summary>
/// The config error queue.
/// </summary>
public class ConfigErrorQueue : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
{
    /// <summary>
    /// The get configuration.
    /// </summary>
    /// <returns>
    /// The <see cref="MessageForwardingInCaseOfFaultConfig"/>.
    /// </returns>
    public MessageForwardingInCaseOfFaultConfig GetConfiguration()
    {
        return new MessageForwardingInCaseOfFaultConfig
               {
                   ErrorQueue = "error"
               };
    }
}