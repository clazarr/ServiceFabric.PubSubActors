namespace ServiceFabric.PubSubActors.Interfaces
{
    using System;

    /// <summary>
    /// Generic message format. Contains message CLR type (full name) and serialized payload. If you know the Message Type you can deserialize the
    /// payload into that object.
    /// </summary>
    public partial class MessageWrapper
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IPayloadSerializer" /> to use when setting the <see cref="Payload" />. Defaults to <see
        /// cref="DefaultPayloadSerializer" /> which uses Json.Net.
        /// </summary>
        public static IPayloadSerializer PayloadSerializer { get; set; } = new DefaultPayloadSerializer();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Convert the provided <paramref name="message" /> into a <see cref="MessageWrapper" />
        /// </summary>
        /// <param name="message"></param>
        /// <param name="correlationId">The optional correlation identifier to associate with this message.</param>
        /// <returns></returns>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        public static MessageWrapper CreateMessageWrapper<TMessage>(TMessage message, string correlationId = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            IPayloadSerializer serializer = PayloadSerializer ?? new DefaultPayloadSerializer();
            MessageWrapper wrapper = new MessageWrapper
            {
                AssemblyQualifiedMessageType = GetNameForType<TMessage>(message),
                Payload = serializer.Serialize(message),
                CorrelationId = correlationId
            };
            return wrapper;
        }

        /// <summary>
        /// Gets the name for the specified message object instance.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message object instance.</param>
        /// <returns>The name for the specified type.</returns>
        public static string GetNameForType<TMessage>(TMessage message)
        {
            if (message == null)
            {
                return GetNameForType<TMessage>();
            }

            return message.GetType().AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the name for the specified type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <returns>The name for the specified type.</returns>
        public static string GetNameForType<TMessage>()
        {
            return typeof(TMessage).AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the type for the specified assembly qualified full type name.
        /// </summary>
        /// <typeparam name="name">The assembly qualified full type name.</typeparam>
        /// <returns>The type corresponding to the specified name or <c>null</c> if the type could not be identified.</returns>
        public static Type GetTypeForName(string name)
        {
            return Type.GetType(name, throwOnError: false, ignoreCase: true);
        }

        /// <summary>
        /// Returns the type representing the message payload or <c>null</c> if unable to identify type.
        /// </summary>
        /// <returns>The type representing the message payload or <c>null</c> if unable to identify type.</returns>
        /// <remarks>This is useful to check before unwrapping messages if you might receive more than one message type.</remarks>
        public Type GetMessageType()
        {
            return GetTypeForName(this.AssemblyQualifiedMessageType);
        }

        /// <summary>
        /// Determines whether the message payload is of the specified type based on the assembly qualified full type name of the message payload.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message payload to check for.</typeparam>
        /// <returns><c>true</c> if the payload is of the specified type; otherwise, <c>false</c>.</returns>
        /// <remarks>This is useful to check before unwrapping messages if you might receive more than one message type.</remarks>
        public bool IsTypeOf<TMessage>()
        {
            return string.Equals(this.AssemblyQualifiedMessageType, GetNameForType<TMessage>(), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Convert the payload to its original message form using design type resolution and type casting.
        /// </summary>
        /// <returns>The unwrapped, typed message object.</returns>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        public TMessage UnwrapMessage<TMessage>()
        {
            if (string.IsNullOrWhiteSpace(this.Payload))
            {
                throw new ArgumentNullException(nameof(this.Payload));
            }

            IPayloadSerializer serializer = PayloadSerializer ?? new DefaultPayloadSerializer();
            return serializer.Deserialize<TMessage>(this.Payload);
        }

        /// <summary>
        /// Convert the payload to its original message form using runtime type resolution with no type casting.
        /// </summary>
        /// <returns>The unwrapped, untyped message object.</returns>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        public object UnwrapMessage()
        {
            if (string.IsNullOrWhiteSpace(this.Payload))
            {
                throw new ArgumentNullException(nameof(this.Payload));
            }

            IPayloadSerializer serializer = PayloadSerializer ?? new DefaultPayloadSerializer();
            return serializer.Deserialize(this.Payload, this.GetMessageType());
        }

        #endregion Public Methods
    }
}