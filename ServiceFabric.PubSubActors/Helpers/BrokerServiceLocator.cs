using System;
using System.Fabric;
using System.Fabric.Query;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace ServiceFabric.PubSubActors.Helpers
{
    public class BrokerServiceLocator : IBrokerServiceLocator
    {
        #region Private Fields

        private static ServicePartitionList _cachedPartitions;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a new default instance.
        /// </summary>
        public BrokerServiceLocator()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc />
        public async Task RegisterAsync(Uri brokerServiceName)
        {
            CodePackageActivationContext activationContext = FabricRuntime.GetActivationContext();
            using (FabricClient fabricClient = new FabricClient())
            {
                await fabricClient.PropertyManager.PutPropertyAsync(new Uri(activationContext.ApplicationName), nameof(BrokerService), brokerServiceName.ToString());
            }
        }

        /// <inheritdoc />
        public async Task<Uri> LocateAsync()
        {
            Uri serviceUri = null;

            try
            {
                CodePackageActivationContext activationContext = FabricRuntime.GetActivationContext();
                using (FabricClient fabricClient = new FabricClient())
                {
                    NamedProperty property = await fabricClient.PropertyManager.GetPropertyAsync(new Uri(activationContext.ApplicationName), nameof(BrokerService));
                    if (property != null)
                    {
                        serviceUri = new Uri(property.GetValue<string>());
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return serviceUri;
        }

        /// <inheritdoc />
        public async Task<ServicePartitionKey> GetPartitionForMessageAsync(object message, Uri brokerServiceName)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (brokerServiceName == null) throw new ArgumentNullException(nameof(brokerServiceName));

            string messageTypeName = (message.GetType().FullName);

            if (_cachedPartitions == null)
            {
                using (FabricClient fabricClient = new FabricClient())
                {
                    _cachedPartitions = await fabricClient.QueryManager.GetPartitionListAsync(brokerServiceName);
                }
            }

            int index = Math.Abs(messageTypeName.GetHashCode() % _cachedPartitions.Count);
            var partition = _cachedPartitions[index];
            if (partition.PartitionInformation.Kind != ServicePartitionKind.Int64Range)
            {
                throw new InvalidOperationException("Sorry, only Int64 Range Partitions are supported.");
            }

            var info = (Int64RangePartitionInformation)partition.PartitionInformation;
            var resolvedPartition = new ServicePartitionKey(info.LowKey);

            return resolvedPartition;
        }

        /// <inheritdoc />
        public Task<IBrokerService> GetBrokerServiceForMessageAsync(object message, Uri brokerServiceName)
        {
            return GetBrokerServiceForMessageAsync(message.GetType().FullName, brokerServiceName);
        }

        /// <inheritdoc />
        public async Task<IBrokerService> GetBrokerServiceForMessageAsync(string messageTypeName, Uri brokerServiceName)
        {
            var resolvedPartition = await GetPartitionForMessageAsync(messageTypeName, brokerServiceName);
            var brokerService = ServiceProxy.Create<IBrokerService>(brokerServiceName, resolvedPartition, listenerName: BrokerServiceBase.ListenerName);
            return brokerService;
        }

        #endregion Public Methods
    }
}