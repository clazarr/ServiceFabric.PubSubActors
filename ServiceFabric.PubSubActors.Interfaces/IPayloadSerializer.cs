namespace ServiceFabric.PubSubActors.Interfaces
{
    using System;

    /// <summary>
    /// Converts objects to strings and back again.
    /// </summary>
    public interface IPayloadSerializer
    {
        #region Public Methods

        /// <summary>
        /// Converts the provided object instance into a string representation.
        /// </summary>
        /// <param name="payload"></param>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <returns>Serialized object instance in a string representation.</returns>
        /// <exception cref="ArgumentNullException">Object instance is <c>null</c>.</exception>
        string Serialize<TPayload>(TPayload payload);

        /// <summary>
        /// Converts the provided string representation into an object instance using design type resolution and type casting.
        /// </summary>
        /// <param name="serializedData">String representation of a serialized object instance.</param>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <returns>Deserialized object instance of the specified type.</returns>
        /// <exception cref="ArgumentNullException">String representation is <c>null</c> or empty.</exception>
        TPayload Deserialize<TPayload>(string serializedData);

        /// <summary>
        /// Converts the provided string representation into an object instance using runtime type resolution with no type casting.
        /// </summary>
        /// <param name="serializedData">String representation of a serialized object instance.</param>
        /// <param name="serializedType">Type of the serialized object instance.</param>
        /// <returns>Deserialized object instance for the specified type.</returns>
        /// <exception cref="ArgumentNullException">
        /// String representation is <c>null</c> or empty
        /// -or- Type information is <c>null</c>
        /// </exception>
        object Deserialize(string serializedData, Type serializedType);

        #endregion Public Methods
    }
}