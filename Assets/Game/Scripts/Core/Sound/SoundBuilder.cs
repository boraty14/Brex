using UnityEngine;

namespace Game.Scripts.Core.Sound
{
    public class SoundBuilder
    {
        private readonly SoundService _soundService;
        private SoundData _soundData;
        private Vector3 _position = Vector3.zero;
        private bool _randomPitch;

        public SoundBuilder(SoundService soundService)
        {
            _soundService = soundService;
        }
        
        public SoundBuilder WithSoundData(SoundData soundData)
        {
            _soundData = soundData;
            return this;
        }
        
        public SoundBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }
        
        public SoundBuilder WithRandomPitch()
        {
            _randomPitch = true;
            return this;
        }

        public void Play()
        {
            if (!_soundService.CanPlaySound(_soundData))
            {
                return;
            }

            var soundEmitter = _soundService.Get();
            soundEmitter.Init(_soundData, _soundService);
            soundEmitter.transform.position = _position;

            if (_randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (_soundService.Counts.TryGetValue(_soundData, out var count))
            {
                _soundService.Counts[_soundData] = count + 1;
            }
            else
            {
                _soundService.Counts[_soundData] = 1;
            }
            
            soundEmitter.Play();
        }
    }
}