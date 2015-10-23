﻿using EventCentric.EventSourcing;
using EventCentric.Messaging;
using EventCentric.Repository;
using EventCentric.Serialization;
using EventCentric.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventCentric.Polling
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly Func<bool, EventStoreDbContext> contextFactory;
        private readonly ITextSerializer serializer;
        private readonly ITimeProvider time;

        public SubscriptionRepository(Func<bool, EventStoreDbContext> contextFactory, ITextSerializer serializer, ITimeProvider time)
        {
            Ensure.NotNull(contextFactory, "contextFactory");
            Ensure.NotNull(serializer, "serializer");
            Ensure.NotNull(time, "time");

            this.contextFactory = contextFactory;
            this.serializer = serializer;
            this.time = time;
        }

        public SubscriptionBuffer[] GetSubscriptions()
        {
            var subscriptions = new List<SubscriptionBuffer>();

            using (var context = this.contextFactory(true))
            {
                var subscriptionsQuery = context.Subscriptions.Where(s => !s.IsPoisoned && !s.WasCanceled);
                if (subscriptionsQuery.Any())
                    foreach (var s in subscriptionsQuery)
                        // We substract one version in order to set the current version bellow the last one, in case that first event
                        // was not yet processed.
                        subscriptions.Add(new SubscriptionBuffer(s.StreamType.Trim(), s.Url.Trim(), s.Token, s.ProcessorBufferVersion - 1, s.IsPoisoned));
            }

            return subscriptions.ToArray();
        }

        public void FlagSubscriptionAsPoisoned(IEvent poisonedEvent, PoisonMessageException exception)
        {
            using (var context = this.contextFactory.Invoke(false))
            {
                var subscription = context.Subscriptions.Where(s => s.StreamType == poisonedEvent.StreamType).Single();
                subscription.IsPoisoned = true;
                subscription.UpdateTime = this.time.Now;
                subscription.PoisonEventCollectionVersion = poisonedEvent.EventCollectionVersion;
                try
                {
                    subscription.ExceptionMessage = this.serializer.Serialize(exception);
                }
                catch (Exception)
                {
                    subscription.ExceptionMessage = string.Format("Exception type: {0}. Exception message: {1}. Inner exception: {2}", exception.GetType().Name, exception.Message, exception.InnerException.Message != null ? exception.InnerException.Message : "null");
                }
                try
                {
                    subscription.DeadLetterPayload = this.serializer.Serialize(poisonedEvent);
                }
                catch (Exception)
                {
                    subscription.DeadLetterPayload = string.Format("EventType: {0}", poisonedEvent.GetType().Name);
                }

                context.SaveChanges();
            }
        }
    }
}
