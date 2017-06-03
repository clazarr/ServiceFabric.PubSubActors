using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ServiceFabric.PubSubActors.Helpers
{
    public interface IPublisherServiceHelper
    {
        #region Public Methods

        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="message"></param>
        /// <param name="brokerServiceName">The name of a SF Service of type <see cref="BrokerService" />.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns></returns>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        Task PublishMessageAsync<TMessage>(StatelessService service, TMessage message, Uri brokerServiceName = null, string correlationId = null);

        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="message">The message.</param>
        /// <param name="brokerServiceName">The name of a SF Service of type <see cref="BrokerService" />.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns>Task.</returns>
        Task PublishMessageAsync<TMessage>(StatefulServiceBase service, TMessage message, Uri brokerServiceName = null, string correlationId = null);

        #endregion Public Methods
    }
}