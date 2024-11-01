using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Game.Scripts.Core.Unit
{
    public class UnitManager<T> : IDisposable where T : UnitBase
    {
        private readonly Transform _unitParent;
        private AsyncOperationHandle<GameObject> _prefabHandle;
        private T _prefabResult;
        private ObjectPool<T> _pool;
        private readonly List<T> _units = new();

        public UnitManager(Transform unitParent)
        {
            _unitParent = unitParent;
        }

        private ObjectPool<T> Pool
        {
            get
            {
                if (_pool == null)
                {
                    InitPool();
                }

                return _pool;
            }
            set => _pool = value;
        }

        public async UniTask LoadUnit()
        {
            if (_prefabHandle.IsValid() && _prefabHandle.IsDone)
            {
                return;
            }

            _prefabHandle = Addressables.LoadAssetAsync<GameObject>(typeof(T).Name);
            await _prefabHandle.ToUniTask();
            _prefabResult = _prefabHandle.Result.GetComponent<T>();
        }

        public void ReleaseUnit()
        {
            ClearUnits();
            Addressables.Release(_prefabHandle);
        }

        private void InitPool(int initial = 1, int max = 100, bool collectionChecks = false)
        {
            Pool = new ObjectPool<T>(
                CreateSetup,
                GetSetup,
                ReleaseSetup,
                DestroySetup,
                collectionChecks,
                initial,
                max);
        }

        protected virtual T CreateSetup() => Object.Instantiate(_prefabResult, _unitParent);
        protected virtual void GetSetup(T unit) => unit.gameObject.SetActive(true);
        protected virtual void ReleaseSetup(T unit) => unit.gameObject.SetActive(false);
        protected virtual void DestroySetup(T unit) => Object.Destroy(unit.gameObject);

        public T AddUnit()
        {
            T unit = Pool.Get();
            _units.Add(unit);
            return unit;
        }

        public void RemoveUnit(T unit)
        {
            int index = _units.IndexOf(unit);
            if (index == -1)
            {
                return;
            }

            int lastIndex = GetCount() - 1;
            _units[index] = _units[lastIndex];
            _units.RemoveAt(lastIndex);
            Pool.Release(unit);
        }

        public T AddSingle()
        {
            int unitCount = GetCount();
            if (unitCount != 0)
            {
                Debug.LogError($"{typeof(T)} is not single, unit count {unitCount}");
                ClearUnits();
            }

            return AddUnit();
        }

        public T GetSingle()
        {
            int unitCount = GetCount();
            if (unitCount != 1)
            {
                Debug.LogError($"{typeof(T)} is not single, unit count {unitCount}");
            }

            return _units[0];
        }

        public void ClearUnits()
        {
            int unitCount = GetCount();
            for (int i = unitCount - 1; i >= 0; i--)
            {
                RemoveUnit(_units[i]);
            }
        }

        public IReadOnlyCollection<T> GetUnits() => _units;
        public int GetCount() => _units.Count;
        public bool IsEmpty() => GetCount() == 0;

        public void Dispose()
        {
            ReleaseUnit();
        }
    }
}