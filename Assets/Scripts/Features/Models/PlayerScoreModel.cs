using Features.Signals;
using JetBrains.Annotations;
using Zenject;

namespace Features.Models
{
    /// <summary>
    /// Model for the player score and time
    /// </summary>
    [UsedImplicitly]
    public class PlayerScoreModel : IInitializable
    {
        [Inject] private SignalBus signalBus;

        public int Score
        {
            get => score;
            set
            {
                if (score != value)
                {
                    score = value;
                    signalBus.Fire<Match3Signals.PlayerScoreChangedSignal>();
                }
            }
        }
        
        public int Time
        {
            get => time;
            set
            {
                if (time != value)
                {
                    time = value;
                    signalBus.Fire<Match3Signals.PlayerTimeChangedSignal>();
                }
            }
        }

        private int score;
        private int time;

        public void Initialize()
        {
            Reset();
        }

        public void Reset()
        {
            Score = 0;
        }
    }
}