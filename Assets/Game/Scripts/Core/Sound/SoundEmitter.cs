using System.Collections;
using UnityEngine;

namespace Game.Scripts.Core.Sound
{
    public class SoundEmitter : MonoBehaviour
    {
        private SoundService _soundService;
        private AudioSource _audioSource;
        private Coroutine _playSoundCoroutine;
        private float _startPitch;
        
        public SoundData SoundData { get; private set; }

        private void Awake()
        {
            if (!TryGetComponent<AudioSource>(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            _startPitch = _audioSource.pitch;
        }

        public void Init(SoundData soundData, SoundService soundService)
        {
            SoundData = soundData;
            _soundService = soundService;
            _audioSource.clip = soundData.clip;
            _audioSource.outputAudioMixerGroup = soundData.mixerGroup;
            _audioSource.volume = soundData.volume;
            _audioSource.loop = soundData.loop;
            _audioSource.playOnAwake = soundData.playOnAwake;
        }

        public void Play()
        {
            if (_playSoundCoroutine != null)
            {
                StopCoroutine(_playSoundCoroutine);
            }

            _audioSource.Play();
            _playSoundCoroutine = StartCoroutine(WaitForSoundToEndRoutine());
        }

        public void Stop()
        {
            if (_playSoundCoroutine != null)
            {
                StopCoroutine(_playSoundCoroutine);
                _playSoundCoroutine = null;
            }

            _audioSource.Stop();
            _soundService.ReturnToPool(this);
        }

        private IEnumerator WaitForSoundToEndRoutine()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            _soundService.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            _audioSource.pitch = _startPitch + Random.Range(min, max);
        }
    }
}