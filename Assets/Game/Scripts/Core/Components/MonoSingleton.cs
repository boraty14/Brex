using UnityEngine;

namespace Game.Scripts.Core.Components
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T I { get; private set; }
        private void Awake()
        {
            I = this as T;
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
    }
}