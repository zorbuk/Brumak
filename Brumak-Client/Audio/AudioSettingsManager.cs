using System.IO;
using System.Text.Json;

namespace Brumak_Client.Audio
{
    public static class AudioSettingsManager
    {
        private static readonly string Path = "audio_settings.json";

        public static AudioSettings Load()
        {
            try
            {
                if (!File.Exists(Path)) return new AudioSettings();
                var json = File.ReadAllText(Path);
                return JsonSerializer.Deserialize<AudioSettings>(json) ?? new AudioSettings();
            }
            catch { return new AudioSettings(); }
        }

        public static void Save(AudioSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(Path, json);
            }
            catch { }
        }
    }
}