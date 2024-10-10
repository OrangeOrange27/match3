using System.Collections.Generic;
using Core.Config;
using Core.Utils;
using Features.Config;
using Features.Data;
using Features.Data.Components;
using Features.Models;
using Features.Signals;
using Features.Views;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace Features.Presenters
{
    /// <summary>
    /// Main presenter for the board responsible for managing gem views
    /// </summary>
    public class BoardPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject gemsContainer;

        [SerializeField] private GameObject spawnParticlesPrefab;

        private readonly Dictionary<Entity, GemView> gemViews = new();
        
        private DiContainer diContainer;
        private EntityManager entityManager;
        private SignalBus signalBus;
        private AssetsCatalogue assetsCatalogue;

        private GameStateModel gameStateModel;
        private BoardModel boardModel;
        private float boardScale;
        
        private Dictionary<GemColor, GemView> colorToPrefabMap;

        [Inject]
        public void Inject(EntityManager entityManager, SignalBus signalBus, DiContainer diContainer,
            GameStateModel gameStateModel,
            BoardModel boardModel, AssetsCatalogue assetsCatalogue)
        {
            this.gameStateModel = gameStateModel;
            this.boardModel = boardModel;
            this.entityManager = entityManager;
            this.signalBus = signalBus;
            this.assetsCatalogue = assetsCatalogue;
            this.diContainer = diContainer;
            
            CreateColorToPrefabMap(assetsCatalogue.GetGemAssetConfigs());
        }

        private void OnEnable()
        {
            signalBus.Subscribe<Match3Signals.GemCreatedSignal>(OnGemCreated);
            signalBus.Subscribe<Match3Signals.GemDestroyedSignal>(OnGemDestroyed);
            signalBus.Subscribe<Match3Signals.OnBoardCreatedSignal>(OnBoardCreated);
            signalBus.Subscribe<Match3Signals.OnPlayerTurnStart>(OnTurnStarts);
            signalBus.Subscribe<Match3Signals.StartFallSignal>(OnTurnEnds);
        }

        private void OnDisable()
        {
            signalBus.Unsubscribe<Match3Signals.GemCreatedSignal>(OnGemCreated);
            signalBus.Unsubscribe<Match3Signals.GemDestroyedSignal>(OnGemDestroyed);
            signalBus.Unsubscribe<Match3Signals.OnBoardCreatedSignal>(OnBoardCreated);
            signalBus.Unsubscribe<Match3Signals.OnPlayerTurnStart>(OnTurnStarts);
            signalBus.Unsubscribe<Match3Signals.StartFallSignal>(OnTurnEnds);

            Dispose();
        }

        private void LateUpdate()
        {
            foreach (var view in gemViews.Values)
            {
                view.UpdatePosition();
            }
        }
        
        private void CreateColorToPrefabMap(IEnumerable<GemAssetConfig> configs)
        {
            colorToPrefabMap = new Dictionary<GemColor, GemView>();
            foreach (var cfg in configs)
            {
                colorToPrefabMap.Add(cfg.Color, cfg.Prefab);
            }
        }

        private void OnBoardCreated()
        {
            var scaleX = 8 / BoardCalculator.GetBoardWidthSize();
            var scaleY = 11 / BoardCalculator.GetBoardHeightSize();
            boardScale = Mathf.Min(scaleX, scaleY);
            gemsContainer.transform.localScale = new Vector3(boardScale, boardScale, 1);
        }

        private void OnTurnStarts()
        {
            Debug.Log("On Turn Starts");

            foreach (KeyValuePair<Entity, GemView> keyValuePair in gemViews)
            {
                keyValuePair.Value.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        private void OnTurnEnds()
        {
            Debug.Log("On Turn Ends");

            foreach (KeyValuePair<Entity, GemView> keyValuePair in gemViews)
            {
                keyValuePair.Value.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private void OnGemCreated(Match3Signals.GemCreatedSignal signal)
        {
            var startingPosition = signal.Position;

            var gemView =
                diContainer.InstantiatePrefabForComponent<GemView>(colorToPrefabMap[signal.GemColor],
                    gemsContainer.transform);
            gemView.Init(entityManager);
            gemView.SetData(signal.Entity, startingPosition);
            gemViews.Add(signal.Entity, gemView);
            gemView.OnDestructionAnimationComplete += OnDestructionAnimationComplete;
        }

        private void OnGemDestroyed(Match3Signals.GemDestroyedSignal signal)
        {
            var gemColor = entityManager.GetComponentData<GemComponent>(signal.Entity).Color;
            var gemView = gemViews[signal.Entity];

            var gameObject = Instantiate(spawnParticlesPrefab, this.transform);
            var view = gameObject.GetComponent<ParticleEffectView>();
            view.Show(assetsCatalogue.GetGemDestroyEffectSprite(gemColor));
            Vector3 position = gemView.transform.position;
            position.z = -5;
            gameObject.transform.position = position;
            var originalScale = gameObject.transform.localScale;
            gameObject.transform.localScale = new Vector3(originalScale.x * boardScale, originalScale.y * boardScale,
                originalScale.z);

            gemView.DestroyAnimation();
            gemViews.Remove(signal.Entity);
        }

        private void OnDestructionAnimationComplete(GemView view)
        {
            view.OnDestructionAnimationComplete -= OnDestructionAnimationComplete;
            Destroy(view.gameObject);
        }

        public void Dispose()
        {
            gemViews.Clear();
        }
    }
}