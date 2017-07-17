using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using ServiceFabric.PubSubActors.Interfaces;

namespace ServiceFabric.PubSubActors.Helpers
{
    /// <summary>
    /// Common operations of <see cref="ServiceFabric.PubSubActors.PublisherActors" />
    /// </summary>
	public class PublisherActorHelper : IPublisherActorHelper
    {
        #region Private Fields

        private readonly IBrokerServiceLocator _brokerServiceLocator;

        #endregion Private Fields

        #region Public Constructors

        public PublisherActorHelper()
        {
            _brokerServiceLocator = new BrokerServiceLocator();
        }

        public PublisherActorHelper(IBrokerServiceLocator brokerServiceLocator)
        {
            _brokerServiceLocator = brokerServiceLocator;
        }

        #endregion Public Constructors

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
        /// <exception cref="System.ArgumentNullException">actor or message</exception>
        /// <exception cref="System.InvalidOperationException">No brokerServiceName was provided or discovered in the current application.</exception>
        public async virtual Task PublishMessageAsync<TMessage>(ActorBase actor, TMessage message, Uri brokerServiceName = null, string correlationId = null)
        {
            if (actor == null) throw new ArgumentNullException(nameof(actor));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (brokerServiceName == null)
            {
                brokerServiceName = await PublisherServiceHelper.DiscoverBrokerServiceNameAsync(this._brokerServiceLocator);
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException("No brokerServiceName was provided or discovered in the current application.");
                }
            }

            var wrapper = MessageWrapper.CreateMessageWrapper(message, correlationId);
            var brokerService = await _brokerServiceLocator.GetBrokerServiceForMessageAsync(message, brokerServiceName);
            await brokerService.PublishMessageAsync(wrapper);
        }

        #endregion Public Methods
    }
}