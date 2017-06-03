namespace ServiceFabric.PubSubActors.Interfaces
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Generic message format. Contains message CLR type (assembly qualified full name) and serialized payload. If you know the Message Type you can
    /// deserialize the payload into that object.
    /// </summary>
    [DataContract]
    public partial class MessageWrapper : IExtensibleDataObject
    {
        #region Private Fields

        private ExtensionDataObject extensionDataObject;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Indicates whether this message was relayed.
        /// </summary>
        [DataMember]
        public bool IsRelayed { get; set; }

        /// <summary>
        /// CLR Type Full Name of serialized payload.
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public string MessageType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.AssemblyQualifiedMessageType))
                {
                    return this.AssemblyQualifiedMessageType;
                }

                int index = this.AssemblyQualifiedMessageType.IndexOf(',');
                if (index <= 0)
                {
                    // Assume full type name only was specified, without the assembly qualifier suffix.
                    return this.AssemblyQualifiedMessageType;
                }

                // Parse out the leading full name portion of the type ("typefullname, assemblyfullname").
                string name = this.AssemblyQualifiedMessageType.Substring(0, index).TrimEnd();
                return name;
            }
        }

        /// <summary>
        /// Serialized object.
        /// </summary>
        [DataMember]
        public string Payload { get; set; }

        /// <summary>
        /// CLR Type Assembly Qualified Full Name of serialized payload ("typefullname, assemblyfullname").
        /// </summary>
        /// <remarks>Used for deserialization when type to deserialize is not known in advance.</remarks>
        [DataMember(Order = 2)]
        public string AssemblyQualifiedMessageType { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier associated with this message or <c>null</c> if the message cannot be correlated.
        /// </summary>
        /// <value>The correlation identifier associated with this message or <c>null</c> if the message cannot be correlated.</value>
        [DataMember(Order = 2)]
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <value>The extension data.</value>
        ExtensionDataObject IExtensibleDataObject.ExtensionData { get => this.extensionDataObject; set => this.extensionDataObject = value; }

        #endregion Public Properties
    }
}