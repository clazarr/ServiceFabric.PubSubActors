using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ServiceFabric.PubSubActors.Interfaces;
using ServiceFabric.PubSubActors.SubscriberServices;

namespace ServiceFabric.PubSubActors.State
{
    /// <summary>
    /// Persistable reference to a Service.
    /// </summary>
    [DataContract]
    public class ServiceReferenceWrapper : ReferenceWrapper
    {
        #region Public Constructors

        /// <summary>
        /// Creates a new instance, for Serializer use only.
        /// </summary>
        [Obsolete("Only for Serializer use.")]
        public ServiceReferenceWrapper()
        {
        }

        /// <summary>
        /// Creates a new instance using the provided <see cref="ServiceReference" />.
        /// </summary>
        /// <param name="serviceReference"></param>
        /// <param name="correlationId">The optional correlation identifier to use to match messages for this service (i.e., a simple message filter).</param>
        public ServiceReferenceWrapper(ServiceReference serviceReference, string correlationId = null)
            : base(correlationId)
        {
            if (serviceReference == null) throw new ArgumentNullException(nameof(serviceReference));

            ServiceReference = serviceReference;
        }

        #endregion Public Constructors

        #region Public Properties

        public override string Name
        {
            get { return ServiceReference.Description; }
        }

        /// <summary>
        /// Gets the wrapped <see cref="ServiceReference" />
        /// </summary>
        [DataMember]
        public ServiceReference ServiceReference { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ServiceReferenceWrapper other)
        {
            if (other?.ServiceReference?.PartitionGuid == null) return false;
            return Equals(other.ServiceReference.PartitionGuid, ServiceReference.PartitionGuid);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Equals(obj as ServiceReferenceWrapper);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode - need to support Serialization.
            return ServiceReference.PartitionGuid.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public override bool Equals(ReferenceWrapper other)
        {
            return Equals(other as ServiceReferenceWrapper);
        }

        /// <summary>
        /// Attempts to publish the message to a listener.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override Task PublishAsync(MessageWrapper message)
        {
            var client = SubscriberServicePartitionClient.Create(ServiceReference);
            return client.ReceiveMessageAsync(message);
        }

        #endregion Public Methods
    }
}