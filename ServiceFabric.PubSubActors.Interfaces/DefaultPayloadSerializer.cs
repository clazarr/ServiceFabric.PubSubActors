namespace ServiceFabric.PubSubActors.Interfaces
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// The default serializer to use for <see cref="MessageWrapper.Payload" />.
    /// </summary>
    public class DefaultPayloadSerializer : IPayloadSerializer
    {
        #region Private Fields

        private readonly JsonSerializerSettings settings;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPayloadSerializer" /> class.
        /// </summary>
        public DefaultPayloadSerializer()
        {
            this.settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPayloadSerializer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException">settings is <c>null</c></exception>
        public DefaultPayloadSerializer(JsonSerializerSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc />
        public string Serialize<TMessage>(TMessage payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return JsonConvert.SerializeObject(payload, this.settings);
        }

        /// <inheritdoc />
        public TMessage Deserialize<TMessage>(string serializedData)
        {
            if (string.IsNullOrWhiteSpace(serializedData))
            {
                throw new ArgumentNullException(nameof(serializedData));
            }

            return JsonConvert.DeserializeObject<TMessage>(serializedData, this.settings);
        }

        /// <inheritdoc />
        public object Deserialize(string serializedData, Type serializedType)
        {
            if (string.IsNullOrWhiteSpace(serializedData))
            {
                throw new ArgumentNullException(nameof(serializedData));
            }

            if (serializedType == null)
            {
                throw new ArgumentNullException(nameof(serializedType));
            }

            return JsonConvert.DeserializeObject(serializedData, serializedType, this.settings);
        }

        #endregion Public Methods
    }
}