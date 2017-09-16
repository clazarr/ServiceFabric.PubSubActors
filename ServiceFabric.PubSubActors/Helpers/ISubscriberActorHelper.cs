using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ServiceFabric.PubSubActors.Helpers
{
    public interface ISubscriberActorHelper
    {
        #region Public Methods

        /// <summary>
        /// Registers this Actor as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message subscriber.</param>
        /// <returns>Task.</returns>
        Task RegisterMessageTypeAsync(ActorBase actor, Type messageType, Uri brokerServiceName = null, string correlationId = null);

        /// <summary>
        /// Unregisters this Actor as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="flushQueue">if set to <c>true</c> [flush queue].</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message subscriber.</param>
        /// <returns>Task.</returns>
        Task UnregisterMessageTypeAsync(ActorBase actor, Type messageType, bool flushQueue, Uri brokerServiceName = null, string correlationId = null);

        #endregion Public Methods
    }
}