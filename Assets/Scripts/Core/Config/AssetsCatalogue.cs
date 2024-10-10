using Features.Config;
using Features.Data;
using Features.Views;
using UnityEngine;

namespace Core.Config
{
    /// <summary>
    /// Catalogue of game assets
    /// </summary>
    [CreateAssetMenu(fileName = "New AssetsCatalogue", menuName = "AssetCatalogue/AssetsCatalogue")]
    public class AssetsCatalogue : ScriptableObject
    {
        [SerializeField] private GemAssetConfig[] gemAssetConfigs;

        public GemAssetConfig[] GetGemAssetConfigs()
        {
            return gemAssetConfigs;
        }
        
        public GemView GetGem(GemColor gemColor)
        {
            foreach (var cfg in gemAssetConfigs)
            {
                if (cfg.Color == gemColor)
                    return cfg.Prefab;
            }

            return null;
        }      
        
        public Sprite GetGemDestroyEffectSprite(GemColor gemColor)
        {
            foreach (var cfg in gemAssetConfigs)
            {
                if (cfg.Color == gemColor)
                    return cfg.DestroyEffectSprite;
            }

            return null;
        }
    }
}