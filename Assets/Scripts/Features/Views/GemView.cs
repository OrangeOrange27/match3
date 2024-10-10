using Core.Config;
using DG.Tweening;
using Features.Data;
using Features.Data.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Zenject;

namespace Features.Views
{
    public delegate void OnTouchDown(Entity entity);

    public delegate void OnTouchUp(Entity entity);

    public delegate void OnDestructionAnimationComplete(GemView view);

    public class GemView : MonoBehaviour
    {
        [SerializeField] private float destroyDuration = 0.3f;

        public event OnTouchDown OnTouchDown;
        public event OnTouchUp OnTouchUp;
        public event OnDestructionAnimationComplete OnDestructionAnimationComplete;

        private int entityId;
        private EntityManager entityManager;
        private Entity entity;
        private float3 originPosition;
        private string prefabName;

        private GameObject gemGameObject;
        private Transform gemTransform;
        private Vector3 defaultScale;
        private AssetsCatalogue catalogue;

        public Entity Entity => entity;

        [Inject]
        private void Inject(AssetsCatalogue catalogue)
        {
            this.catalogue = catalogue;
        }

        private void Awake()
        {
            gemTransform = transform;
            defaultScale = gameObject.transform.localScale;
        }

        private void OnMouseDown()
        {
            OnTouchDown?.Invoke(entity);
        }

        private void OnMouseUp()
        {
            OnTouchUp?.Invoke(entity);
        }

        public void DestroyAnimation()
        {
            gameObject.transform
                .DOScale(0.1f, destroyDuration)
                .OnComplete(() =>
                {
                    gameObject.transform.localScale = defaultScale;
                    OnDestructionAnimationComplete?.Invoke(this);
                });
        }

        public void Init(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public void SetData(Entity entity, float3 originPosition)
        {
            this.entity = entity;
            this.originPosition = originPosition;

            var gemComponent = entityManager.GetComponentData<GemComponent>(entity);
        }

        public void UpdatePosition()
        {
            var translation = entityManager.GetComponentData<LocalTransform>(entity);
            gemTransform.localPosition = translation.Position;
        }
    }
}