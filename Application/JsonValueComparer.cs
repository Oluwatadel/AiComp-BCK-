using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AiComp.Application
{
    public class JsonValueComparer<T> : ValueComparer<T>
    {
        public JsonValueComparer() : base(
       (c1, c2) => JsonConvert.SerializeObject(c1) == JsonConvert.SerializeObject(c2),
       c => c == null ? 0 : JsonConvert.SerializeObject(c).GetHashCode(),
       c => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(c)))
        { }
    }
}
