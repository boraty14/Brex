using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Core.Memory
{
    public class SceneLoadService : IDisposable
    {
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes = new();
        private string _lastLoadedScene = string.Empty;
        
        public void LoadScene(string sceneKey, bool unloadLastLoadedScene = true)
        {
            if (_loadedScenes.ContainsKey(sceneKey))
            {
                Debug.LogError($"Scene {sceneKey} is already loaded");
                return;
            }

            LoadSceneInternal(sceneKey, unloadLastLoadedScene).Forget();
        }

        public void UnloadScene(string sceneKey)
        {
            if (!_loadedScenes.ContainsKey(sceneKey))
            {
                Debug.LogError($"Scene {sceneKey} is not loaded");
                return;
            }

            UnloadSceneInternal(sceneKey).Forget();
        }

        private async UniTaskVoid LoadSceneInternal(string sceneKey, bool unloadLastLoadedScene)
        {
            var handle = Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Additive,
                SceneReleaseMode.OnlyReleaseSceneOnHandleRelease);
            await handle.ToUniTask();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Given scene {sceneKey} could not loaded.");
                return;
            }

            _loadedScenes.TryAdd(sceneKey, handle);

            if (unloadLastLoadedScene && !string.IsNullOrEmpty(_lastLoadedScene))
            {
                UnloadScene(_lastLoadedScene);
            }

            _lastLoadedScene = sceneKey;
        }

        private async UniTaskVoid UnloadSceneInternal(string sceneKey)
        {
            var loadHandle = _loadedScenes[sceneKey];

            if (!loadHandle.Result.Scene.isLoaded)
            {
                return;
            }

            var unloadHandle = Addressables.UnloadSceneAsync(loadHandle, false);
            await unloadHandle.ToUniTask();

            if (unloadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Given scene {sceneKey} could not unloaded.");
                return;
            }

            _loadedScenes.Remove(sceneKey);
        }

        public void Dispose()
        {
            foreach (var loadedSceneKey in _loadedScenes.Keys)
            {
                UnloadScene(loadedSceneKey);
            }
        }
    }
}