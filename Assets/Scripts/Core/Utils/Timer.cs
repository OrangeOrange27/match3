using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Utils
{
    public class Timer
    {
        public int CurrentTime => GetTimeSpanToTarget().TotalSeconds > 0 ? (int)GetTimeSpanToTarget().TotalSeconds : 0;

        public bool IsPaused { get; private set; }

        public event Action<int> OnTimeChanged;
        
        private CancellationTokenSource timerCancellationToken;
        private DateTime targetTime;

        public async void Start(int seconds, Action callback)
        {
            timerCancellationToken = new CancellationTokenSource();
            targetTime = DateTime.UtcNow.AddSeconds(seconds);

            await RunTimerTask(timerCancellationToken.Token);
            callback?.Invoke();
        }

        public void Stop()
        {
            timerCancellationToken?.Cancel();
            OnTimeChanged = null;
        }

        public void Pause()
        {
            IsPaused = true;
        }
        
        public void Resume()
        {
            IsPaused = false;
        }
        
        private TimeSpan GetTimeSpanToTarget()
        {
            var delta = targetTime - DateTime.UtcNow;
            return delta;
        }

        private async Task RunTimerTask(CancellationToken token)
        {
            var delta = GetTimeSpanToTarget();
            while (delta.TotalSeconds > 0)
            {
                if (IsPaused)
                {
                    targetTime = targetTime.AddSeconds(.1); // to make delta constant
                }
                else
                {
                    OnTimeChanged?.Invoke(delta.TotalSeconds > 0 ? (int)delta.TotalSeconds : 0);
                }

                delta = GetTimeSpanToTarget();
                await Task.Delay(TimeSpan.FromSeconds(.1f), cancellationToken: token);
            }
        }
    }
}