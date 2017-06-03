using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ServiceFabric.PubSubActors.Helpers
{
    public interface IPublisherActorHelper
    {
        #region Public Methods

        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        /// <param name="actor">The actor.</param>
        /// <param name="message">The message.</param>
        /// <param name="brokerServiceName">The name of the SF Service of type <see cref="BrokerService" />.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns>Task.</returns>
        Task PublishMessageAsync<TMessage>(ActorBase actor, TMessage message, Uri brokerServiceName = null, string correlationId = null);

        #endregion Public Methods
    }
}