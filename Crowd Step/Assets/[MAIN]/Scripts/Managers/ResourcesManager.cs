using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallyGames.Shop;
using SmallyGames.Settings;
using SmallyGames.Menus;
using Sirenix.OdinInspector;


namespace CrowdStep
{
    [CreateAssetMenu(menuName = "SmallyGames/Resources/Resources Manager", fileName = "Resources Manager")]
    public class ResourcesManager : ScriptableObject
    {
        [SerializeField, BoxGroup("Money Data")]
        private CoinManager coinsManager;
        public CoinManager CoinsManager => coinsManager;

        [SerializeField, BoxGroup("Shop Category")]
        private Category characterCat;
        public Category CharacterCat => characterCat;

        [SerializeField, BoxGroup("Shop Category")]
        private Category levelCat;
        public Category LevelCat => levelCat;

        [SerializeField, BoxGroup("Settings Data")]
        private SettingsData settings;
        public SettingsData Settings => settings;
    }
}
