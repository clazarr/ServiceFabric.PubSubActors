using System;
using System.Fabric;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabric.PubSubActors.Interfaces;

namespace ServiceFabric.PubSubActors.Helpers
{
    public class SubscriberServiceHelper : ISubscriberServiceHelper
    {
        #region Private Fields

        private readonly IBrokerServiceLocator _brokerServiceLocator;

        #endregion Private Fields

        #region Public Constructors

        public SubscriberServiceHelper()
        {
            _brokerServiceLocator = new BrokerServiceLocator();
        }

        public SubscriberServiceHelper(IBrokerServiceLocator brokerServiceLocator)
        {
            _brokerServiceLocator = brokerServiceLocator;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Registers this stateless service as a subscriber for messages of type <paramref name="messageType" /> with the
        /// <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message subscriber.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">service or messageType</exception>
        /// <exception cref="System.InvalidOperationException">No brokerServiceName was provided or discovered in the current application.</exception>
        public async Task RegisterMessageTypeAsync(StatelessService service, Type messageType, Uri brokerServiceName = null, string correlationId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));
            if (brokerServiceName == null)
            {
                brokerServiceName = await PublisherServiceHelper.DiscoverBrokerServiceNameAsync(this._brokerServiceLocator);
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException(
                        "No brokerServiceName was provided or discovered in the current application.");
                }
            }
            var brokerService =
                await _brokerServiceLocator.GetBrokerServiceForMessageAsync(messageType.Name, brokerServiceName);
            var serviceReference = CreateServiceReference(service.Context, GetServicePartition(service).PartitionInfo);
            if (correlationId == null)
            {
                await brokerService.RegisterServiceSubscriberAsync(serviceReference, messageType.FullName);
            }
            else
            {
                await brokerService.RegisterCorrelatedServiceSubscriberAsync(serviceReference, messageType.FullName, correlationId);
            }
        }

        /// <summary>
        /// Unregisters this stateless service as a subscriber for messages of type <paramref name="messageType" /> with the
        /// <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="flushQueue">if set to <c>true</c> [flush queue].</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message subscriber.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">service or messageType</exception>
        /// <exception cref="System.InvalidOperationException">No brokerServiceName was provided or discovered in the current application.</exception>
        public async Task UnregisterMessageTypeAsync(StatelessService service, Type messageType, bool flushQueue, Uri brokerServiceName = null, string correlationId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));
            if (brokerServiceName == null)
            {
                brokerServiceName = await PublisherServiceHelper.DiscoverBrokerServiceNameAsync(this._brokerServiceLocator);
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException(
                        "No brokerServiceName was provided or discovered in the current application.");
                }
            }
            var brokerService =
                await _brokerServiceLocator.GetBrokerServiceForMessageAsync(messageType.Name, brokerServiceName);
            var serviceReference = CreateServiceReference(service.Context, GetServicePartition(service).PartitionInfo);
            if (correlationId == null)
            {
                await brokerService.UnregisterServiceSubscriberAsync(serviceReference, messageType.FullName, flushQueue);
            }
            else
            {
                await brokerService.UnregisterCorrelatedServiceSubscriberAsync(serviceReference, messageType.FullName, correlationId, flushQueue);
            }
        }

        /// <summary>
        /// Registers this stateful service as a subscriber for messages of type <paramref name="messageType" /> with the <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message subscriber.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">service or messageType</exception>
        /// <exception cref="System.InvalidOperationException">No brokerServiceName was provided or discovered in the current application.</exception>
        public async Task RegisterMessageTypeAsync(StatefulService service, Type messageType, Uri brokerServiceName = null, string correlationId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));
            if (brokerServiceName == null)
            {
                brokerServiceName = await _brokerServiceLocator.LocateAsync();
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException(
                        "No brokerServiceName was provided or discovered in the current application.");
                }
            }
            var brokerService =
                await _brokerServiceLocator.GetBrokerServiceForMessageAsync(messageType.Name, brokerServiceName);
            var serviceReference = CreateServiceReference(service.Context, GetServicePartition(service).PartitionInfo);
            if (correlationId == null)
            {
                await brokerService.RegisterServiceSubscriberAsync(serviceReference, messageType.FullName);
            }
            else
            {
                await brokerService.RegisterCorrelatedServiceSubscriberAsync(serviceReference, messageType.FullName, correlationId);
            }
        }

        /// <summary>
        /// Unregisters this stateful service as a subscriber for messages of type <paramref name="messageType" /> with the
        /// <see cref="BrokerService" />.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="flushQueue">if set to <c>true</c> [flush queue].</param>
        /// <param name="brokerServiceName">Name of the broker service.</param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message subscriber.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">service or messageType</exception>
        /// <exception cref="System.InvalidOperationException">No brokerServiceName was provided or discovered in the current application.</exception>
        public async Task UnregisterMessageTypeAsync(StatefulService service, Type messageType, bool flushQueue, Uri brokerServiceName = null, string correlationId = null)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (messageType == null) throw new ArgumentNullException(nameof(messageType));
            if (brokerServiceName == null)
            {
                brokerServiceName = await _brokerServiceLocator.LocateAsync();
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException(
                        "No brokerServiceName was provided or discovered in the current application.");
                }
            }
            var brokerService =
                await _brokerServiceLocator.GetBrokerServiceForMessageAsync(messageType.Name, brokerServiceName);
            var serviceReference = CreateServiceReference(service.Context, GetServicePartition(service).PartitionInfo);
            if (correlationId == null)
            {
                await brokerService.UnregisterServiceSubscriberAsync(serviceReference, messageType.FullName, flushQueue);
            }
            else
            {
                await brokerService.UnregisterCorrelatedServiceSubscriberAsync(serviceReference, messageType.FullName, correlationId, flushQueue);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the Partition info for the provided StatelessService instance.
        /// </summary>
        /// <param name="serviceBase"></param>
        /// <returns></returns>
        private static IStatelessServicePartition GetServicePartition(StatelessService serviceBase)
        {
            if (serviceBase == null) throw new ArgumentNullException(nameof(serviceBase));
            return (IStatelessServicePartition)serviceBase
                .GetType()
                .GetProperty("Partition", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(serviceBase);
        }

        /// <summary>
        /// Creates a <see cref="ServiceReference" /> for the provided service context and partition info.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static ServiceReference CreateServiceReference(ServiceContext context, ServicePartitionInformation info)
        {
            var serviceReference = new ServiceReference
            {
                ApplicationName = context.CodePackageActivationContext.ApplicationName,
                PartitionKind = info.Kind,
                ServiceUri = context.ServiceName,
                PartitionGuid = context.PartitionId,
            };

            var longInfo = info as Int64RangePartitionInformation;

            if (longInfo != null)
            {
                serviceReference.PartitionID = longInfo.LowKey;
            }
            else
            {
                var stringInfo = info as NamedPartitionInformation;
                if (stringInfo != null)
                {
                    serviceReference.PartitionName = stringInfo.Name;
                }
            }
            return serviceReference;
        }

        /// <summary>
        /// Gets the Partition info for the provided StatefulServiceBase instance.
        /// </summary>
        /// <param name="serviceBase"></param>
        /// <returns></returns>
        private IStatefulServicePartition GetServicePartition(StatefulServiceBase serviceBase)
        {
            if (serviceBase == null) throw new ArgumentNullException(nameof(serviceBase));
            return (IStatefulServicePartition)serviceBase
                .GetType()
                .GetProperty("Partition", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(serviceBase);
        }

        #endregion Private Methods
    }
}