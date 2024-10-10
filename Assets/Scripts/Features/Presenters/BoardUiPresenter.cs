using DG.Tweening;
using Features.Models;
using Features.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace Features.Presenters
{
    /// <summary>
    /// Main ui in board states responsible for updating score and turns 
    /// </summary>
    public class BoardUiPresenter : MonoBehaviour
    {
        [Inject] private SignalBus signalBus;

        [Inject] private PlayerScoreModel playerScoreModel;

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Vector3 punchScale = new Vector3(1.2f, 1.2f, 1.2f);

        private void Start()
        {
            signalBus.Subscribe<Match3Signals.PlayerScoreChangedSignal>(OnPlayerScoreChanged);
            signalBus.Subscribe<Match3Signals.PlayerTimeChangedSignal>(OnPlayerTimeChanged);
        }

        private void OnPlayerTimeChanged()
        {
            timeText.text = playerScoreModel.Time.ToString();
        }

        private void OnPlayerScoreChanged()
        {
            scoreText.text = playerScoreModel.Score.ToString();
            scoreText.transform.DOPunchScale(punchScale, .3f, 5, .3f);
        }

        public void OnRestartClick()
        {
            signalBus.Fire(new ExitToMapSignal());
        }
    }
}