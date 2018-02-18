using System.Collections.Generic;
using System.IO;
using Nancy;
using Nancy.IO;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;

namespace Lockbox.Api.Framework.Serialization
{
    public class JsonNetSerializer : ISerializer
    {
        private readonly JsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class.
        /// </summary>
        public JsonNetSerializer()
        {
            this.serializer = JsonSerializer.CreateDefault();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class,
        /// with the provided <paramref name="serializer"/>.
        /// </summary>
        /// <param name="serializer">Json converters used when serializing.</param>
        public JsonNetSerializer(JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to serialise</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanSerialize(MediaRange mediaRange)
        {
            return Helpers.IsJsonType(mediaRange);
        }

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
        public IEnumerable<string> Extensions
        {
            get { yield return "json"; }
        }

        /// <summary>
        /// Serialize the given model with the given contentType
        /// </summary>
        /// <param name="mediaRange">Content type to serialize into</param>
        /// <param name="model">Model to serialize</param>
        /// <param name="outputStream">Output stream to serialize to</param>
        /// <returns>Serialised object</returns>
        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(new UnclosableStreamWrapper(outputStream))))
            {
                this.serializer.Serialize(writer, model);
            }
        }
    }
}