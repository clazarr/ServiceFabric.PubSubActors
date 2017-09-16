using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabric.PubSubActors
{
    /// <summary>
    /// Acts as a registry for Subscriber Actors and Services that publishing Actors and Services can publish to.
    /// </summary>
    [ServiceContract]
    public interface IBrokerService : Microsoft.ServiceFabric.Services.Remoting.IService
    {
        #region Public Methods

        /// <summary>
        /// Registers an Actor as a subscriber for messages.
        /// </summary>
        /// <param name="actor">Reference to the actor to register.</param>
        /// <param name="messageTypeName">The full type name of the message to subscribe to.</param>
        [OperationContract]
        Task RegisterSubscriberAsync(ActorReference actor, string messageTypeName);

        /// <summary>
        /// Registers an Actor as a subscriber for messages that can be correlated to the subscriber.
        /// </summary>
        /// <param name="actor">Reference to the actor to register.</param>
        /// <param name="messageTypeName">The full type name of the message to subscribe to.</param>
        /// <param name="correlationId">The correlation identifier to use to match messages to a specific subscriber (i.e., a simple message filter).</param>
        /// <returns>Task.</returns>
        [OperationContract]
        Task RegisterCorrelatedSubscriberAsync(ActorReference actor, string messageTypeName, string correlationId);

        /// <summary>
        /// Unregisters an Actor as a subscriber for messages.
        /// </summary>
        /// <param name="messageTypeName">The full type name of the message to subscribe to.</param>
        /// <param name="actor">Reference to the actor to unregister.</param>
        /// <param name="flushQueue">Publish any remaining messages.</param>
        [OperationContract]
        Task UnregisterSubscriberAsync(ActorReference actor, string messageTypeName, bool flushQueue);

        /// <summary>
        /// Registers a service as a subscriber for messages.
        /// </summary>
        /// <param name="messageTypeName">The full type name of the message to subscribe to.</param>
        /// <param name="service">Reference to the Service to register.</param>
        [OperationContract]
        Task RegisterServiceSubscriberAsync(Interfaces.ServiceReference service, string messageTypeName);

        /// <summary>
        /// Registers a service as a subscriber for messages that can be correlated to the subscriber.
        /// </summary>
        /// <param name="service">Reference to the Service to register.</param>
        /// <param name="messageTypeName">The full type name of the message to subscribe to.</param>
        /// <param name="correlationId">The correlation identifier to use to match messages to a specific subscriber (i.e., a simple message filter).</param>
        /// <returns>Task.</returns>
        [OperationContract]
        Task RegisterCorrelatedServiceSubscriberAsync(Interfaces.ServiceReference service, string messageTypeName, string correlationId);

        /// <summary>
        /// Unregisters a service as a subscriber for messages.
        /// </summary>
        /// <param name="messageTypeName">The full type name of the message to subscribe to.</param>
        /// <param name="service">Reference to the Service to unregister.</param>
        /// <param name="flushQueue">Publish any remaining messages.</param>
        [OperationContract]
        Task UnregisterServiceSubscriberAsync(Interfaces.ServiceReference service, string messageTypeName, bool flushQueue);

        /// <summary>
        /// Takes a published message and forwards it (indirectly) to all Subscribers.
        /// </summary>
        /// <param name="message">The message to publish</param>
        /// <returns></returns>
        [OperationContract]
        Task PublishMessageAsync(Interfaces.MessageWrapper message);

        #endregion Public Methods
    }
}