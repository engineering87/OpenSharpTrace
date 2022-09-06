// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OpenSharpTrace.Utilities
{
    public static class Serialization
    {
        private const string empyJson = "{}";

        public static string ToJson(this object value)
        {
            if (value == null) return empyJson;
            if (value.ToString() == string.Empty) return empyJson;

            try
            {
                JsonSerializerOptions option = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var result = JsonSerializer.Serialize(value, option);

                if (string.IsNullOrEmpty(result))
                    return empyJson;
                return result;
            }
            catch
            {
                return empyJson;
            }
        }
    }
}
