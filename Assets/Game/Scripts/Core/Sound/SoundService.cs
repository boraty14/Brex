using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.Scripts.Core.Sound
{
    public class SoundService : IDisposable
    {
        private readonly SoundSettings _soundSettings;
        private readonly Transform _soundParent;
        private readonly List<SoundEmitter> _activeSoundEmitters = new();
        public readonly Dictionary<SoundData, int> Counts = new();
        private IObjectPool<SoundEmitter> _soundEmitterPool;

        public SoundService(SoundSettings soundSettings, Transform soundParent)
        {
            _soundSettings = soundSettings;
            _soundParent = soundParent;
            InitPool();
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData soundData)
        {
            if (!Counts.TryGetValue(soundData, out var count))
            {
                return true;
            }

            return count < _soundSettings.maxSoundInstances;
        }

        public SoundEmitter Get() => _soundEmitterPool.Get();

        public void ReturnToPool(SoundEmitter soundEmitter) => _soundEmitterPool.Release(soundEmitter);

        public void StopAllSound()
        {
            var soundEmitters = _activeSoundEmitters.ToArray();
            foreach (var soundEmitter in soundEmitters)
            {
                soundEmitter.Stop();
            }
        }

        private void InitPool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _soundSettings.collectionCheck,
                _soundSettings.defaultCapacity,
                _soundSettings.maxPoolSize
            );
        }

        private SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Object.Instantiate(_soundSettings.soundEmitterPrefab, _soundParent);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitters.Add(soundEmitter);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            if (Counts.TryGetValue(soundEmitter.SoundData, out var count))
            {
                Counts[soundEmitter.SoundData] = count - 1;
            }
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitters.Remove(soundEmitter);
        }

        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Object.Destroy(soundEmitter.gameObject);
        }

        public void Dispose()
        {
        }
    }
}