using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Couchbase.Configuration.Server
{
    public  class VBucketMapConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (IVBucket[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var vBucketMap = (List<IVBucket>)Activator.CreateInstance(objectType);
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        var vBucket = new VBucket();
                        int index = 0;
                        if (reader.TokenType == JsonToken.Integer)
                        {
                            int? value = reader.ReadAsInt32();
                            if (index == 0)
                            {
                                if (value.HasValue)
                                {
                                    vBucket.Primary = value.Value;
                                }
                            }
                            else
                            {
                                if (value.HasValue)
                                {
                                    vBucket.Replicas.Add(value.Value);
                                }
                            }
                            index++;
                        }
                        vBucketMap.Add(vBucket);
                    }
                }
            }
            return vBucketMap;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
