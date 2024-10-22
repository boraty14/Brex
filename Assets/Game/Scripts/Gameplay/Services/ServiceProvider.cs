using System;
using Game.Scripts.Core.Memory;
using Game.Scripts.Core.Panel;
using Game.Scripts.Core.Sound;
using UnityEngine;

namespace Game.Scripts.Gameplay.Services
{
    public class ServiceProvider : IDisposable
    {
        public DynamicLoadService DynamicLoadService { get; private set; }
        public SceneLoadService SceneLoadService { get; private set; }
        public PanelService PanelService { get; private set; }
        public SoundService SoundService { get; private set; }

        public ServiceProvider(Transform panelParent, SoundSettings soundSettings, Transform soundParent)
        {
            DynamicLoadService = new DynamicLoadService();
            SceneLoadService = new SceneLoadService();
            PanelService = new PanelService(panelParent);
            SoundService = new SoundService(soundSettings, soundParent);
        }

        public void Dispose()
        {
            DynamicLoadService?.Dispose();
            SceneLoadService?.Dispose();
            PanelService?.Dispose();
            SoundService?.Dispose();
        }
    }
}