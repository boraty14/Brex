namespace Game.Scripts.Core.Components
{
    public abstract class MonoSingletonPersistent<T> : MonoSingleton<T> where T : MonoSingletonPersistent<T>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            DontDestroyOnLoad(gameObject);
        }
    }
}