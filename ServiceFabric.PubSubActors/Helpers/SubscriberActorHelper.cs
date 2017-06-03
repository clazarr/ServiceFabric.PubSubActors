using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace ServiceFabric.PubSubActors.Helpers
{
    /// <summary>
    /// Common operations for Actors to become Subscribers
    /// </summary>
    public class SubscriberActorHelper : ISubscriberActorHelper
    {
        #region Private Fields

        private readonly IBrokerServiceLocator _brokerServiceLocator;

        #endregion Private Fields

        #region Public Constructors

        public SubscriberActorHelper()
        {
            _brokerServiceLocator = new BrokerServiceLocator();
        }

        public SubscriberActorHelper(IBrokerServiceLocator brokerServiceLocator)
        {
            _brokerServiceLocator = brokerServiceLocator;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Registers this Actor as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">messageType or actor</exception>
        /// <exception cref="System.InvalidOperationException">No brokerServiceName was provided or discovered in the current application.</exception>
        public async Task RegisterMessageTypeAsync(ActorBase actor, Type messageType, Uri brokerServiceName = null, string correlationId = null)
        {
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));
            if (actor == null) throw new ArgumentNullException(nameof(actor));
            if (brokerServiceName == null)
            {
                brokerServiceName = await PublisherServiceHelper.DiscoverBrokerServiceNameAsync(this._brokerServiceLocator);
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException("No brokerServiceName was provided or discovered in the current application.");
                }
            }
            var brokerService = await _brokerServiceLocator.GetBrokerServiceForMessageAsync(messageType.Name, brokerServiceName);
            if (correlationId == null)
            {
                await brokerService.RegisterSubscriberAsync(ActorReference.Get(actor), messageType.FullName);
            }
            else
            {
                await brokerService.RegisterCorrelatedSubscriberAsync(ActorReference.Get(actor), messageType.FullName, correlationId);
            }
        }

        /// <summary>
        /// Unregisters this Actor as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="flushQueue">if set to <c>true</c> [flush queue].</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">messageType or actor</exception>
        /// <exception cref="System.InvalidOperationException">No brokerServiceName was provided or discovered in the current application.</exception>
        public async Task UnregisterMessageTypeAsync(ActorBase actor, Type messageType, bool flushQueue, Uri brokerServiceName = null)
        {
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            if (brokerServiceName == null)
            {
                brokerServiceName = await PublisherServiceHelper.DiscoverBrokerServiceNameAsync(this._brokerServiceLocator);
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException("No brokerServiceName was provided or discovered in the current application.");
                }
            }
            var brokerService = await _brokerServiceLocator.GetBrokerServiceForMessageAsync(messageType.Name, brokerServiceName);
            await brokerService.UnregisterSubscriberAsync(ActorReference.Get(actor), messageType.FullName, flushQueue);
        }

        #endregion Public Methods
    }
}