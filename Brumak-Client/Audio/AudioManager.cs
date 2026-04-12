namespace Brumak_Client.Audio
{
    public class AudioManager
    {
        public static readonly AudioManager Instance = new();

        public AudioChannel Master { get; } = new("Master");
        public AudioChannel BGM { get; } = new("BGM");
        public AudioChannel BGE { get; } = new("BGE");
        public AudioChannel BGS { get; } = new("BGS");
        public AudioChannel SE { get; } = new("SE");
        public AudioChannel Voice { get; } = new("Voice");
        public AudioChannel UI { get; } = new("UI");

        public IEnumerable<AudioChannel> AllChannels =>
            [BGM, BGE, BGS, SE, Voice, UI];

        private AudioManager() => LoadSettings();

        public void LoadSettings()
        {
            var s = AudioSettingsManager.Load();

            Master.Volume = s.Master;
            BGM.Volume = s.BGM;
            BGE.Volume = s.BGE;
            BGS.Volume = s.BGS;
            SE.Volume = s.SE;
            Voice.Volume = s.Voice;
            UI.Volume = s.UI;

            foreach (var ch in AllChannels)
                ch.SetMasterVolume(s.Master);
        }

        public void SaveSettings()
        {
            AudioSettingsManager.Save(new AudioSettings
            {
                Master = Master.Volume,
                BGM = BGM.Volume,
                BGE = BGE.Volume,
                BGS = BGS.Volume,
                SE = SE.Volume,
                Voice = Voice.Volume,
                UI = UI.Volume
            });
        }

        public void SetMasterVolume(float volume)
        {
            Master.Volume = volume;
            foreach (var ch in AllChannels)
                ch.SetMasterVolume(volume);
        }

        public void PlayBGM(string path, bool loop = true) => BGM.Play(path, loop);
        public void PlaySE(string path) => SE.Play(path);
        public void PlayUI(string path) => UI.Play(path);
        public void StopAll()
        {
            foreach (var ch in AllChannels) ch.Stop();
        }
    }
}