﻿using System;
using System.Fabric;
using System.Fabric.Query;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ServiceFabric.PubSubActors.Interfaces;

namespace ServiceFabric.PubSubActors.PublisherActors
{
    /// <summary>
    /// Common operations of <see cref="ServiceFabric.PubSubActors.PublisherActors"/>
    /// </summary>
    public static class PublisherActorExtensions
    {
        #region Private Fields

        /////broker service code
        private static ServicePartitionList _cachedPartitions;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="message"></param>
        /// <param name="applicationName">
        /// The name of the SF application that hosts the <see cref="BrokerActor"/>. If not provided, actor.ApplicationName will be used.
        /// </param>
        /// <returns></returns>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        public static async Task PublishMessageAsync<TMessage>(this ActorBase actor, TMessage message, string applicationName = null)
        {
            if (actor == null) throw new ArgumentNullException(nameof(actor));
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(applicationName))
            {
                applicationName = actor.ApplicationName;
            }

            var brokerActor = GetBrokerActorForMessage(message, applicationName);
            var wrapper = MessageWrapper.CreateMessageWrapper<TMessage>(message);
            await brokerActor.PublishMessageAsync(wrapper);
        }

        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="message"></param>
        /// <param name="brokerServiceName">The name of the SF Service of type <see cref="BrokerService"/>.</param>
        /// <returns></returns>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        [Obsolete("Use ServiceFabric.PubSubActors.Helpers.PublisherActorHelper for testability")]
        public static async Task PublishMessageToBrokerServiceAsync<TMessage>(this ActorBase actor, TMessage message, Uri brokerServiceName = null)
        {
            if (actor == null) throw new ArgumentNullException(nameof(actor));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (brokerServiceName == null)
            {
                brokerServiceName = await PublisherServices.PublisherServiceExtensions.DiscoverBrokerServiceNameAsync(new Uri(actor.ApplicationName));
                if (brokerServiceName == null)
                {
                    throw new InvalidOperationException("No brokerServiceName was provided or discovered in the current application.");
                }
            }

            var wrapper = MessageWrapper.CreateMessageWrapper<TMessage>(message);
            var brokerService = await GetBrokerServiceForMessageAsync(message, brokerServiceName);
            await brokerService.PublishMessageAsync(wrapper);
        }

        /// <summary>
        /// Resolves the <see cref="ServicePartitionKey"/> to send the message to, based on message type.
        /// </summary>
        /// <param name="message">The message to publish</param>
        /// <param name="brokerServiceName"></param>
        /// <returns></returns>
        public static async Task<ServicePartitionKey> GetPartitionForMessageAsync(object message, Uri brokerServiceName)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (brokerServiceName == null) throw new ArgumentNullException(nameof(brokerServiceName));

            string messageTypeName = (message.GetType().FullName);

            if (_cachedPartitions == null)
            {
                var fabricClient = new FabricClient();
                _cachedPartitions = await fabricClient.QueryManager.GetPartitionListAsync(brokerServiceName);
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

        /// <summary>
        /// Gets the <see cref="IBrokerService"/> instance for the provided <paramref name="message"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="brokerServiceName">Uri of BrokerService instance</param>
        /// <returns></returns>
        public static Task<IBrokerService> GetBrokerServiceForMessageAsync(object message, Uri brokerServiceName)
        {
            return GetBrokerServiceForMessageAsync(message.GetType().FullName, brokerServiceName);
        }

        /// <summary>
        /// Gets the <see cref="IBrokerService"/> instance for the provided <paramref name="messageTypeName"/>
        /// </summary>
        /// <param name="messageTypeName">Full type name of message object.</param>
        /// <param name="brokerServiceName">Uri of BrokerService instance</param>
        /// <returns></returns>
        public static async Task<IBrokerService> GetBrokerServiceForMessageAsync(string messageTypeName, Uri brokerServiceName)
        {
            var resolvedPartition = await GetPartitionForMessageAsync(messageTypeName, brokerServiceName);
            var brokerService = ServiceProxy.Create<IBrokerService>(brokerServiceName, resolvedPartition, listenerName: BrokerServiceBase.ListenerName);
            return brokerService;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the <see cref="BrokerActor"/> instance for the provided <paramref name="message"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        private static IBrokerActor GetBrokerActorForMessage(object message, string applicationName)
        {
            ActorId actorId = new ActorId(message.GetType().FullName);
            IBrokerActor brokerActor = ActorProxy.Create<IBrokerActor>(actorId, applicationName, nameof(IBrokerActor));
            return brokerActor;
        }

        #endregion Private Methods
    }
}