﻿using EventCentric.Authorization;
using EventCentric.Publishing;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Occ.ServiceHost.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AuthorizeEventSourcing]
    public class EventSourceController : ApiController
    {
        private readonly IPollableEventSource source;

        public EventSourceController(IPollableEventSource source)
        {
            this.source = source;
        }

        [HttpGet]
        [Route("eventsource/events/{eventBufferVersion}/{consumerName}")]
        public IHttpActionResult Events(int eventBufferVersion, string consumerName)
        {
            return this.Ok(this.source.PollEvents(eventBufferVersion, consumerName));
        }
    }
}
