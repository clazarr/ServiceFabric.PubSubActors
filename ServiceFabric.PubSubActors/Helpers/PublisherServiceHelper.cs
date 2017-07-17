using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabric.PubSubActors.Interfaces;

namespace ServiceFabric.PubSubActors.Helpers
{
    public class PublisherServiceHelper : IPublisherServiceHelper
    {
        #region Private Fields

        private readonly IBrokerServiceLocator _brokerServiceLocator;

        #endregion Private Fields

        #region Public Constructors

        public PublisherServiceHelper()
        {
            _brokerServiceLocator = new BrokerServiceLocator();
        }

        public PublisherServiceHelper(IBrokerServiceLocator brokerServiceLocator)
        {
            _brokerServiceLocator = brokerServiceLocator;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Attempts to discover the <see cref="BrokerService" /> running in this Application.
        /// </summary>
        /// <param name="serviceLocator">The optional service locator to use.</param>
        /// <returns>Task&lt;Uri&gt;.</returns>
        public static Task<Uri> DiscoverBrokerServiceNameAsync(IBrokerServiceLocator serviceLocator = null)
        {
            IBrokerServiceLocator locator = serviceLocator ?? new BrokerServiceLocator();
            return locator.LocateAsync();
        }

        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="message"></param>
        /// <param name="brokerServiceName">The name of a SF Service of type <see cref="BrokerService" />.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns></returns>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        public async virtual Task PublishMessageAsync<TMessage>(StatelessService service, TMessage message, Uri brokerServiceName = null, string correlationId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (brokerServiceName == null)
            {
                brokerServiceName = await DiscoverBrokerServiceNameAsync(this._brokerServiceLocator);
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException(
                        "No brokerServiceName was provided or discovered in the current application.");
                }
            }

            var brokerService = await _brokerServiceLocator.GetBrokerServiceForMessageAsync(message, brokerServiceName);
            var wrapper = MessageWrapper.CreateMessageWrapper(message, correlationId);
            await brokerService.PublishMessageAsync(wrapper);
        }

        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="message"></param>
        /// <param name="brokerServiceName">The name of a SF Service of type <see cref="BrokerService" />.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns></returns>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        public async virtual Task PublishMessageAsync<TMessage>(StatefulServiceBase service, TMessage message, Uri brokerServiceName = null, string correlationId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (brokerServiceName == null)
            {
                brokerServiceName = await DiscoverBrokerServiceNameAsync(this._brokerServiceLocator);
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException(
                        "No brokerServiceName was provided or discovered in the current application.");
                }
            }

            var brokerService = await _brokerServiceLocator.GetBrokerServiceForMessageAsync(message, brokerServiceName);
            var wrapper = MessageWrapper.CreateMessageWrapper(message, correlationId);
            await brokerService.PublishMessageAsync(wrapper);
        }

        #endregion Public Methods
    }
}