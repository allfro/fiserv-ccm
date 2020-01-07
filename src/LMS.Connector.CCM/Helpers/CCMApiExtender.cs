using System.Net.Http;
using LMS.Core.Rest;
using Newtonsoft.Json;

namespace LMS.Connector.CCM.Helpers
{
    public class CCMApiExtender : API
    {
        public CCMApiExtender()
        { }

        public CCMApiExtender(HttpClient httpClient) : base(httpClient)
        { }

        public override string SerializeRequest<RequestType>(RequestType request)
        {
            if (request != null)
            {
                //// Camel-case the property name when serialized (e.g. ThisPropertyName will be serialized to thisPropertyName)
                //var serializerSettings = new JsonSerializerSettings()
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //};

                //var serialized = JsonConvert.SerializeObject(request, serializerSettings);
                var serialized = JsonConvert.SerializeObject(request, new JsonConverter[] { new CCMJsonConverter() });

                return serialized;
            }

            return string.Empty;
        }

        public override ResponseType DeSerializeResponse<ResponseType>(string response)
        {
            var responseType = JsonConvert.DeserializeObject<ResponseType>(response, new JsonConverter[] { new CCMJsonConverter() });

            return responseType;
        }
    }
}
