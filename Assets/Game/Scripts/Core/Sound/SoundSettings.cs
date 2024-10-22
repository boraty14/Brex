using UnityEngine;

namespace Game.Scripts.Core.Sound
{
    [CreateAssetMenu(menuName = "Game/Settings/Sound Settings", fileName = "Sound Settings")]
    public class SoundSettings : ScriptableObject
    {
        [Header("Sound Manager")]
        public SoundEmitter soundEmitterPrefab;
        public bool collectionCheck;
        public int defaultCapacity;
        public int maxPoolSize;
        public int maxSoundInstances;

        [Header("Sounds")]
        public SoundData gameStartSoundData;
    }
}