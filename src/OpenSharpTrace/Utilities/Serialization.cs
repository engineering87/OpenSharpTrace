// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenSharpTrace.Utilities
{
    public static class Serialization
    {
        /// <summary>
        /// Extension method for object JSON serialization
        /// </summary>
        /// <param name="value">Object to serialize</param>
        /// <returns></returns>
        public static string ToJson(this object value)
        {
            if (value == null) return emptyJson;
            if (value.ToString() == string.Empty) return emptyJson;

            try
            {
                JsonSerializerOptions option = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var result = JsonSerializer.Serialize(value, option);

                return string.IsNullOrWhiteSpace(result) ? emptyJson : result;
            } 
            catch
            {
                return emptyJson;
            }
        }

        private const string emptyJson = "{}";
    }
}
