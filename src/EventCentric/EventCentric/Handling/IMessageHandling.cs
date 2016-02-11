﻿using EventCentric.EventSourcing;
using System;

namespace EventCentric.Handling
{
    /// <summary>
    /// Represents a message handling function. 
    /// </summary>
    public interface IMessageHandling
    {
        Guid StreamId { get; }
        bool ShouldBeIgnored { get; }
        Func<IEventSourced> Handle { get; }
    }
}