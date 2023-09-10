using UnityEngine;

namespace Injact
{
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        [Inject] public DiContainer Container { get; } = null!;

        public abstract void InstallBindings();
    }
}