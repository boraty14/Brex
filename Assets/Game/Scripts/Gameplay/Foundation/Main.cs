using Game.Scripts.Core.Components;
using Game.Scripts.Core.Sound;
using Game.Scripts.Gameplay.Models;
using Game.Scripts.Gameplay.Services;
using Game.Scripts.Gameplay.Units;
using UnityEngine;

namespace Game.Scripts.Gameplay.Foundation
{
    public class Main : MonoSingleton<Main>
    {
        [SerializeField] private Transform _panelParent;
        [SerializeField] private SoundSettings _soundSettings;
        [SerializeField] private Transform _soundParent;
        
        public static ModelProvider Models { get; private set; }
        public static ServiceProvider Services { get; private set; }
        public static UnitProvider Units { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            Clean();
            Models = new ModelProvider();
            Services = new ServiceProvider(_panelParent, _soundSettings, _soundParent);
            Units = new UnitProvider();
        }

        private void OnDestroy()
        {
            Clean();
        }

        private void Clean()
        {
            Models?.Dispose();
            Services?.Dispose();
            Units?.Dispose();
        }
    }
}