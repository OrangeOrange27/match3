using System;
using Features.Data;
using Features.Views;
using UnityEngine;

namespace Features.Config
{
    [Serializable]
    public struct GemAssetConfig
    {
        public GemColor Color;
        public GemView Prefab;
        public Sprite DestroyEffectSprite;
    }
}