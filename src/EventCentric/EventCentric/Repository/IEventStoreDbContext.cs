﻿using EventCentric.Repository.Mapping;
using System;
using System.Data.Entity;

namespace EventCentric.Repository
{
    public interface IEventStoreDbContext : IDisposable
    {
        IDbSet<StreamEntity> Streams { get; }

        IDbSet<EventEntity> Events { get; }

        IDbSet<SubscriptionEntity> Subscriptions { get; }

        IDbSet<InboxEntity> Inbox { get; }

        int SaveChanges();
    }
}