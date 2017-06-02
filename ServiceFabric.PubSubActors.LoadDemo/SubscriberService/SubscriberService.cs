using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.DataContracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabric.PubSubActors.Helpers;
using ServiceFabric.PubSubActors.Interfaces;
using ServiceFabric.PubSubActors.SubscriberServices;

namespace SubscriberService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class SubscriberService : StatelessService, ISubscriberService
    {
        #region Private Fields

        private static readonly string Messagesettings = "MessageSettings";
        private readonly object _lockMe = new object();
        private int _messageTypeCount;

        private int _messagesExpectedCount;
        private int _messagesReceivedCount;
        private Dictionary<string, HashSet<Guid>> _messagesReceived;
        private IBrokerServiceLocator _brokerServiceLocator;

        private Stopwatch _stopwatch;

        #endregion Private Fields

        #region Public Constructors

        public SubscriberService(StatelessServiceContext context)
            : base(context)
        {
            _brokerServiceLocator = new BrokerServiceLocator();
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task ReceiveMessageAsync(MessageWrapper message)
        {
            if (message == null)
            {
                ServiceEventSource.Current.ServiceMessage(this, "*** Subscriber unexpectedly received a NULL message!");
            }

            DataContract dc = message.UnwrapMessage<DataContract>();
            var set = _messagesReceived[message.MessageType];
            int localMessagesReceived = 0;
            lock (_lockMe)
            {
                if (_stopwatch == null)
                {
                    _stopwatch = Stopwatch.StartNew();
                }
                if (!set.Add(dc.Id))
                {
                    ServiceEventSource.Current.ServiceMessage(this, $"Received duplicate Message ID {dc.Id}.");
                }
                ServiceEventSource.Current.ServiceMessage(this,
                    $"Instance {Context.InstanceId} Received Message Type {message.MessageType}. Total count:{set.Count}.");
                localMessagesReceived = _messagesReceivedCount++;

                if (_messagesReceivedCount == _messagesExpectedCount)
                {
                    _stopwatch.Stop();

                    ServiceEventSource.Current.ServiceMessage(this,
                        $"In {_stopwatch.ElapsedMilliseconds}ms - Received all {_messagesExpectedCount} expected messages.");
                    Reset();
                }
            }
            //if (localMessagesReceived == 1)
            //try
            //{
            //    await UnsubscribeAsync(message.MessageType);
            //    int a = 23;
            //}
            //catch (Exception)
            //{
            //}
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Optional override to create listeners
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //Pub-sub listener:
            yield return
                new ServiceInstanceListener(p => new SubscriberCommunicationListener(this, p),
                    "StatelessSubscriberCommunicationListener");
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var brokerServiceName = await _brokerServiceLocator.LocateAsync();

            //subscribe to messages by their type name:
            string setting = GetConfigurationValue(Context, Messagesettings, "MessageTypeCount");
            if (string.IsNullOrWhiteSpace(setting) || !int.TryParse(setting, out _messageTypeCount))
            {
                return;
            }
            int amount;
            setting = GetConfigurationValue(Context, Messagesettings, "Amount");
            if (string.IsNullOrWhiteSpace(setting) || !int.TryParse(setting, out amount))
            {
                return;
            }
            _messagesExpectedCount = amount;

            bool useConcurrentBroker = false;
            setting = GetConfigurationValue(Context, Messagesettings, "UseConcurrentBroker");
            if (!string.IsNullOrWhiteSpace(setting))
            {
                bool.TryParse(setting, out useConcurrentBroker);
            }

            for (int i = 0; i < _messageTypeCount; i++)
            {
                string messageTypeName = $"DataContract{i}";
                await SubscribeAsync(messageTypeName, useConcurrentBroker);
                ServiceEventSource.Current.ServiceMessage(this,
                    $"Subscribing to {amount} instances of Message Type {messageTypeName}.");
            }

            Reset();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
            }

            if (_messagesReceived != null)
            {
                ServiceEventSource.Current.ServiceMessage(this,
                    $"Instance {Context.InstanceId} stopping. Total counts:{string.Join(", ", _messagesReceived.Select(m => $"Message Type '{m.Key}' - {m.Value.Count}"))}.");
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private static string GetConfigurationValue(ServiceContext context, string sectionName, string parameterName)
        {
            var configSection = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var section = (configSection?.Settings.Sections.Contains(sectionName) ?? false) ? configSection?.Settings.Sections[sectionName] : null;
            string endPointType = (section?.Parameters.Contains(parameterName) ?? false) ? section.Parameters[parameterName].Value : null;
            return endPointType;
        }

        private void Reset()
        {
            _stopwatch = null;
            _messagesReceived = new Dictionary<string, HashSet<Guid>>();
            for (int i = 0; i < _messageTypeCount; i++)
            {
                string messageTypeName = $"DataContract{i}";
                _messagesReceived[messageTypeName] = new HashSet<Guid>();
            }
            _messagesReceivedCount = 0;
        }

        private async Task SubscribeAsync(string messageTypeName, bool useConcurrentBroker)
        {
            var builder = new UriBuilder(Context.CodePackageActivationContext.ApplicationName);
            if (useConcurrentBroker)
            {
                builder.Path += "/ConcurrentBrokerService";
            }
            else
            {
                builder.Path += "/BrokerService";
            }
            var brokerSvcLocation = builder.Uri;

            ServiceEventSource.Current.ServiceMessage(this, $"Using Broker Service at '{brokerSvcLocation}'.");

            var brokerService = await ServiceFabric.PubSubActors.PublisherActors.PublisherActorExtensions.GetBrokerServiceForMessageAsync(messageTypeName, brokerSvcLocation);
            var serviceReference = SubscriberServiceExtensions.CreateServiceReference(Context, Partition.PartitionInfo);
            await brokerService.RegisterServiceSubscriberAsync(serviceReference, messageTypeName);

            ServiceEventSource.Current.ServiceMessage(this, $"Subscribing to Message Type {messageTypeName}.");
        }

        private async Task UnsubscribeAsync(string messageTypeName)
        {
            var brokerServiceName = await _brokerServiceLocator.LocateAsync();

            var brokerService = await ServiceFabric.PubSubActors.PublisherActors.PublisherActorExtensions.GetBrokerServiceForMessageAsync(messageTypeName, brokerServiceName);
            var serviceReference = SubscriberServiceExtensions.CreateServiceReference(Context, Partition.PartitionInfo);
            await brokerService.UnregisterServiceSubscriberAsync(serviceReference, messageTypeName, false);

            ServiceEventSource.Current.ServiceMessage(this, $"Unsubscribing from Message Type {messageTypeName}.");
        }

        #endregion Private Methods
    }
}