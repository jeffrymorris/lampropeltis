using System;
using Newtonsoft.Json;

namespace Couchbase.Configuration.Server
{
    public class VBucketServerMapConverter : JsonConverter 
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (IVBucketServerMap);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
           return serializer.Deserialize<VBucketServerMap>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
