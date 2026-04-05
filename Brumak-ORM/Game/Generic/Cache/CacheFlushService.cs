using Brumak_Shared.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Generic.Cache
{
    public class CacheFlushService
    {
        private static readonly Logger _logger = new("ORM", typeof(CacheFlushService), showLogs: true, saveLogs: false);

        private readonly List<ICacheFlusher> _flushers = [];
        private readonly TimeSpan _interval;
        private CancellationTokenSource _cts = new();

        public CacheFlushService(TimeSpan interval)
        {
            _interval = interval;
        }

        public void Register(ICacheFlusher flusher)
        {
            _flushers.Add(flusher);
            _logger.Log($"CacheFlusher registered: {flusher.Name}");
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => RunAsync(_cts.Token));
            _logger.Log($"CacheFlushService started (interval: {_interval.TotalSeconds}s)");
        }

        public void Stop()
        {
            _cts.Cancel();
            _logger.Log("CacheFlushService stopped");
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_interval, token);
                    await FlushAllAsync();
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.Log($"Error during cache flush: {ex.Message}");
                }
            }
        }

        private async Task FlushAllAsync()
        {
            foreach (var flusher in _flushers)
            {
                try
                {
                    await flusher.FlushAsync();
                    _logger.Log($"Flushed: {flusher.Name}");
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error flushing {flusher.Name}: {ex.Message}");
                }
            }
        }
    }
}
