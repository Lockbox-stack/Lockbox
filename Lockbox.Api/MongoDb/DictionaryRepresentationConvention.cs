using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;

namespace Lockbox.Api.MongoDb
{
    public class DictionaryRepresentationConvention : ConventionBase, IMemberMapConvention
    {
        private readonly DictionaryRepresentation _dictionaryRepresentation;
        public DictionaryRepresentationConvention(DictionaryRepresentation dictionaryRepresentation)
        {
            _dictionaryRepresentation = dictionaryRepresentation;
        }
        public void Apply(BsonMemberMap memberMap)
        {
            memberMap.SetSerializer(ConfigureSerializer(memberMap.GetSerializer()));
        }
        private IBsonSerializer ConfigureSerializer(IBsonSerializer serializer)
        {
            var dictionaryRepresentationConfigurable = serializer as IDictionaryRepresentationConfigurable;
            if (dictionaryRepresentationConfigurable != null)
            {
                serializer = dictionaryRepresentationConfigurable.WithDictionaryRepresentation(_dictionaryRepresentation);
            }

            var childSerializerConfigurable = serializer as IChildSerializerConfigurable;
            return childSerializerConfigurable == null
                ? serializer
                : childSerializerConfigurable.WithChildSerializer(ConfigureSerializer(childSerializerConfigurable.ChildSerializer));
        }
    }
}