using Brumak_Shared.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Brumak_Shared.Network
{
    public static class FrameSerializer
    {
        private static readonly Logger _logger = new("Shared", typeof(FrameSerializer));

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static string Serialize(INetworkFrame frame)
        {
            return JsonSerializer.Serialize(frame, frame.GetType(), Options);
        }

        public static INetworkFrame? Deserialize(string json)
        {
            try
            {
                var document = JsonDocument.Parse(json);
                if (!document.RootElement.TryGetProperty("type", out var typeElement) &&
                    !document.RootElement.TryGetProperty("Type", out typeElement))
                {
                    _logger.Log("Type property not found in JSON");
                    return null;
                }

                var typeValue = typeElement.GetString();

                return typeValue switch
                {
                    _ => null
                };
            }
            catch (Exception ex)
            {
                _logger.Log($"Error deserializing frame: {ex.Message}");
                _logger.Log($"JSON received: {json}");
                return null;
            }
        }
    }
}
