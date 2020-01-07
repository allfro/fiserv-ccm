using System;
using LMS.Connector.CCM.Dto;
using LMS.Core.HostValues.Utility.Translator.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LMS.Connector.CCM.Helpers
{
    public class CCMJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            var typeOf = typeof(DtoBase).IsAssignableFrom(objectType);

            return typeOf;
        }

        /// <summary>
        /// Deserialize JSON using standard Json.NET functionality with the ability to import Host Value fields.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var deserializedObj = Activator.CreateInstance(objectType);
            var jsonObject = JObject.Load(reader);

            serializer.Populate(jsonObject.CreateReader(), deserializedObj);

            if (deserializedObj != null)
                PopulateHostValues((DtoBase)deserializedObj, jsonObject);

            return deserializedObj;
        }

        /// <summary>
        /// Gives the Json.NET Serializer the ability to inject Host Values to the serialized JSON (for REST operations).
        /// Automatically serializes property names into camel-case format at runtime (e.g. ThisPropertyName will be serialized to thisPropertyName).
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DtoBase obj = (DtoBase)value;
            JObject jObj = JObject.FromObject(
                obj,
                new JsonSerializer()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    /*** Camel-case the property name when serialized (e.g. ThisPropertyName will be serialized to thisPropertyName) ***/
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            );

            obj.HostValues.ExportHostValues(jObj, obj.HostValueParentNode);
            jObj.WriteTo(writer);
        }

        /// <summary>
        /// Adds each property on the deserialized object to the Host Value collection.
        /// </summary>
        /// <param name="deserializedObj"></param>
        /// <param name="jsonObject"></param>
        private static void PopulateHostValues(DtoBase deserializedObj, JObject jsonObject)
        {
            foreach (var prop in jsonObject.Properties())
            {
                deserializedObj.HostValues.ImportHostValues(new JSONHostValue(prop, deserializedObj.HostValueParentNode));
            }
        }
    }
}
