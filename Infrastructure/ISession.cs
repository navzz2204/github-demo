using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace ToyStoreOnlineWeb.Infrastructure
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            session.SetString(key, JsonConvert.SerializeObject(value, settings));
        }
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

    }
}