﻿using Clientes.Events;
using EventCentric.EventSourcing;
using EventCentric.Messaging;
using EventCentric.Processing;
using System;

namespace Clientes.ReadModel
{
    public class ClientesDenormalizerHandler : EventProcessor<ClientesDenormalizer>,
        IEventHandler<CuentaCreadaANuevoCliente>
    {
        public ClientesDenormalizerHandler(IBus bus, IEventStore<ClientesDenormalizer> store)
            : base(bus, store)
        { }

        public void Handle(CuentaCreadaANuevoCliente incomingEvent)
        {
            base.CreateNewStreamIfNotExists(Guid.Empty, incomingEvent);
        }
    }
}