using NAudio.Vorbis;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Brumak_Client.Audio
{
    public class AudioChannel : IDisposable
    {
        public string Name { get; }

        private float _volume = 1.0f;
        private float _masterVolume = 1.0f;

        private WaveOutEvent? _output;
        private VorbisWaveReader? _reader;
        private bool _loop;

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Math.Clamp(value, 0f, 1f);
                ApplyVolume();
            }
        }

        public AudioChannel(string name) => Name = name;

        internal void SetMasterVolume(float master)
        {
            _masterVolume = Math.Clamp(master, 0f, 1f);
            ApplyVolume();
        }

        private void ApplyVolume()
        {
            if (_output != null)
                _output.Volume = _volume * _masterVolume;
        }

        public void Play(string path, bool loop = false)
        {
            Stop();

            _loop = loop;

            try
            {
                var uri = new Uri("pack://application:,,," + path, UriKind.Absolute);
                var streamInfo = Application.GetResourceStream(uri)
                    ?? throw new FileNotFoundException($"Recurso no encontrado: {path}");

                var ms = new MemoryStream();
                streamInfo.Stream.CopyTo(ms);
                ms.Position = 0;

                _reader = new VorbisWaveReader(ms);
                _output = new WaveOutEvent();
                _output.Init(_reader);
                _output.Volume = _volume * _masterVolume;
                _output.PlaybackStopped += OnPlaybackStopped;
                _output.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AudioChannel:{Name}] Error al reproducir '{path}': {ex.Message}");
            }
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (!_loop) return;

            _reader!.Position = 0;
            _output!.Play();
        }

        public void Stop()
        {
            _output?.Stop();
            _output?.Dispose();
            _reader?.Dispose();
            _output = null;
            _reader = null;
        }

        public void Dispose() => Stop();
    }
}