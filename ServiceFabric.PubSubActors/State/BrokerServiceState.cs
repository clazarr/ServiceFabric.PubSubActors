using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;

namespace ServiceFabric.PubSubActors.State
{
    [DataContract]
    internal sealed class BrokerServiceState
    {
        #region Public Fields

        [DataMember]
        public readonly string MessageTypeName;

        #endregion Public Fields

        #region Private Fields

        private static readonly IEnumerable<Reference> Empty = ImmutableList<Reference>.Empty;

        #endregion Private Fields

        #region Public Constructors

        public BrokerServiceState(string messageTypeName, IEnumerable<Reference> subscribers = null)
        {
            MessageTypeName = messageTypeName;
            Subscribers = subscribers != null ? subscribers.ToImmutableList() : Empty;
        }

        #endregion Public Constructors

        #region Public Properties

        [DataMember]
        public IEnumerable<Reference> Subscribers { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Returns a cloned instance with the same subscribers as the original, plus the new <paramref name="subscriber" />
        /// </summary>
        /// <param name="current"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public static BrokerServiceState AddSubscriber(BrokerServiceState current, Reference subscriber)
        {
            if (current == null) throw new ArgumentNullException(nameof(current));
            if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));
            if (current.Subscribers.Any(s => s.ServiceOrActorReference.Equals(subscriber.ServiceOrActorReference)))
            {
                return current;
            }

            var clone = new BrokerServiceState(current.MessageTypeName, ((ImmutableList<Reference>)current.Subscribers).Add(subscriber));
            return clone;
        }

        /// <summary>
        /// Returns a cloned instance with the same subscribers as the original, minus the new <paramref name="subscriber" />
        /// </summary>
        /// <param name="current"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public static BrokerServiceState RemoveSubscriber(BrokerServiceState current, Reference subscriber)
        {
            if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));

            return RemoveSubscriber(current, subscriber.ServiceOrActorReference);
        }

        /// <summary>
        /// Returns a cloned instance with the same subscribers as the original, minus the new <paramref name="subscriber" />
        /// </summary>
        /// <param name="current"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public static BrokerServiceState RemoveSubscriber(BrokerServiceState current, ReferenceWrapper subscriber)
        {
            if (current == null) throw new ArgumentNullException(nameof(current));
            if (subscriber == null) throw new ArgumentNullException(nameof(subscriber));

            if (current.Subscribers.All(s => !s.ServiceOrActorReference.Equals(subscriber)))
            {
                return current;
            }

            var clone = new BrokerServiceState(current.MessageTypeName, ((ImmutableList<Reference>)current.Subscribers).RemoveAll(s => s.ServiceOrActorReference.Equals(subscriber)));
            return clone;
        }

        #endregion Public Methods

        #region Private Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Convert the deserialized collection to an immutable collection
            Subscribers = Subscribers.ToImmutableList();
        }

        #endregion Private Methods
    }
}