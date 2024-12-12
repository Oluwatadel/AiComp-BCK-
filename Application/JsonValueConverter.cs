using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace AiComp.Application
{
    public class JsonValueConverter<T> : ValueConverter<T, string>
    {
        public JsonValueConverter() : base(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<T>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }))
            { }
    }
}
