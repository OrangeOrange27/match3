using Core.Config;
using Core.Utils;
using Features.Config;
using Features.Data;
using Features.Models;
using Features.Signals;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Features.Commands
{
    /// <summary>
    /// Initial command for Board State responsible for creating board and resetting models
    /// </summary>
    [UsedImplicitly]
    public class CreateBoardCommand
    {
        [Inject] private SignalBus signalBus;

        [Inject] private GameStateModel gameStateModel;

        [Inject] private BoardModel boardModel;

        [Inject] private PlayerScoreModel playerScoreModel;

        [Inject] private EntityManager entityManager;

        [Inject] private AssetsCatalogue assetsCatalogue;
        
        private Timer boardTimer;

        public void Execute(Match3Signals.CreateBoardSignal createBoardSignal)
        {
            Debug.Log("Create board -> " + gameStateModel.State);
            var levelConfig = gameStateModel.GetCurrentLevelConfig();
            playerScoreModel.Reset();
            boardModel.ResetBoard(levelConfig);
            BoardCalculator.InitBoardSize(boardModel.BoardWidth, boardModel.BoardHeight);
            CreateGems(levelConfig);
            StartBoardTimer();
            
            signalBus.Subscribe<ExitToMapSignal>(OnExit);
            
            signalBus.Fire<Match3Signals.OnBoardCreatedSignal>();
        }

        private void OnExit()
        {
            boardTimer.Stop();
        }

        private void CreateGems(LevelConfig levelConfig)
        {
            for (int x = 0; x < levelConfig.Width; x++)
            {
                for (int y = 0; y < levelConfig.Height; y++)
                {
                    var gemColor = EntitiesHelper.GetRandomColor();
                    var boardPosition = new int2(x, y);
                    var position = BoardCalculator.ConvertBoardPositionToTransformPosition(boardPosition);
                    var gemEntity = EntitiesHelper.CreateGem(gemColor,
                        position,
                        boardPosition);
                    signalBus.Fire(new Match3Signals.GemCreatedSignal(gemEntity, gemColor, position));
                    boardModel.SetEntityAt(x, y, gemEntity);
                }
            }
        }
        
        private void StartBoardTimer()
        {
            boardTimer = new Timer();
            boardTimer.Start(gameStateModel.GetCurrentLevelConfig().Time, OnTimerEnd);
            boardTimer.OnTimeChanged += OnTimerTick;
        }
        
        private void OnTimerTick(int time)
        {
            signalBus.Fire<Match3Signals.PlayerTimeChangedSignal>();
            playerScoreModel.Time = time;
        }

        private void OnTimerEnd()
        {
            signalBus.Fire<Match3Signals.OutOfTimeSignal>();
            boardTimer.OnTimeChanged -= OnTimerTick;
        }
    }
}