using System.Runtime.Serialization;

namespace ServiceFabric.PubSubActors.State
{
    [DataContract]
    public class Reference
    {
        #region Public Fields

        [DataMember] public readonly string QueueName;

        [DataMember] public readonly string DeadLetterQueueName;

        #endregion Public Fields

        #region Public Constructors

        public Reference(ReferenceWrapper serviceOrActorReference, string queueName, string deadLetterQueueName)
        {
            ServiceOrActorReference = serviceOrActorReference;
            QueueName = queueName;
            DeadLetterQueueName = deadLetterQueueName;
        }

        #endregion Public Constructors

        #region Public Properties

        [DataMember]
        public ReferenceWrapper ServiceOrActorReference { get; private set; }

        #endregion Public Properties
    }
}