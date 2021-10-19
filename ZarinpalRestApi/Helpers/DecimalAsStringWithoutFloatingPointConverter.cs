using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZarinpalRestApi.Helpers
{
    internal class DecimalAsStringWithoutFloatingPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                return token.ToObject<decimal>();
            }
            if (token.Type == JTokenType.String)
            {
                // customize this to suit your needs
                return Decimal.Parse(token.ToString(), CultureInfo.InvariantCulture);
            }
            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
            {
                return null;
            }

            throw new JsonSerializationException("Unexpected token type: " + token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataValue = (decimal?) value;
            if (dataValue.HasValue)
                //writer.WriteRawValue(dataValue.Value.ToString("0", CultureInfo.InvariantCulture));
                writer.WriteValue(dataValue.Value.ToString("0", CultureInfo.InvariantCulture));
            else
                writer.WriteNull();
        }
    }
}