using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ServiceFabric.PubSubActors.Helpers
{
    public interface ISubscriberServiceHelper
    {
        #region Public Methods

        /// <summary>
        /// Registers this stateless service as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns>Task.</returns>
        Task RegisterMessageTypeAsync(StatelessService service, Type messageType, Uri brokerServiceName = null, string correlationId = null);

        /// <summary>
        /// Unregisters this stateless service as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="flushQueue">if set to <c>true</c> [flush queue].</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <returns>Task.</returns>
        Task UnregisterMessageTypeAsync(StatelessService service, Type messageType, bool flushQueue, Uri brokerServiceName = null);

        /// <summary>
        /// Registers this stateful service as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns>Task.</returns>
        Task RegisterMessageTypeAsync(StatefulService service, Type messageType, Uri brokerServiceName = null, string correlationId = null);

        /// <summary>
        /// Unregisters this stateful service as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="flushQueue">if set to <c>true</c> [flush queue].</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <returns>Task.</returns>
        Task UnregisterMessageTypeAsync(StatefulService service, Type messageType, bool flushQueue, Uri brokerServiceName = null);

        #endregion Public Methods
    }
}