using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Game.Scripts.Core.Panel
{
    public class PanelService : IDisposable
    {
        private readonly Dictionary<Type, PanelReference> _loadedPanels = new();
        private readonly Transform _panelParent;

        public PanelService(Transform panelParent)
        {
            _panelParent = panelParent;
        }

        public async UniTask LoadPanel<T>() where T : PanelBase
        {
            var panelKey = typeof(T);
            var panelHandle = Addressables.LoadAssetAsync<GameObject>(typeof(T).Name);
            await panelHandle;
            var panelObject = Object.Instantiate(panelHandle.Result, _panelParent);
            _loadedPanels.Add(panelKey,new PanelReference
            {
                PanelHandle = panelHandle,
                PanelObject = panelObject
            });
        }
        
        public void UnloadPanel<T>() where T : PanelBase
        {
            var panelKey = typeof(T);
            if (!_loadedPanels.TryGetValue(panelKey, out var panelReference))
            {
                Debug.LogError($"Panel {panelKey} is not found");
                return;
            }

            Object.Destroy(panelReference.PanelObject);
            Addressables.Release(panelReference.PanelHandle);
            _loadedPanels.Remove(panelKey);
        }
        
        public async UniTask<T> ShowPanel<T>(bool isImmediate = false) where T : PanelBase
        {
            var panelKey = typeof(T);
            if (!_loadedPanels.TryGetValue(panelKey, out var panelReference))
            {
                Debug.LogError($"Panel {panelKey} is not found");
                return null;
                
            }

            var panel = panelReference.PanelObject.GetComponent<T>();
            await panel.Show(isImmediate);
            return panel;
        }

        public async UniTask<T> HidePanel<T>(bool isImmediate = false) where T : PanelBase
        {
            var panelKey = typeof(T);
            if (!_loadedPanels.TryGetValue(panelKey, out var panelReference))
            {
                Debug.LogError($"Panel {panelKey} is not found");
                return null;
            }

            var panel = panelReference.PanelObject.GetComponent<T>();
            await panel.Hide(isImmediate);
            return panel;
        }

        public T GetPanel<T>() where T : PanelBase
        {
            var panelKey = typeof(T);
            if (_loadedPanels.TryGetValue(panelKey, out var panelReference))
            {
                return panelReference.PanelObject.GetComponent<T>();
            }

            Debug.LogError($"Panel {panelKey} is not found");
            return null;
        }

        public void Dispose()
        {
            foreach (var panelReference in _loadedPanels.Values)
            {
                Addressables.Release(panelReference.PanelHandle);
            }
        }
    }

    public class PanelReference
    {
        public AsyncOperationHandle<GameObject> PanelHandle;
        public GameObject PanelObject;
    }
}