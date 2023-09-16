using UnityEngine;

namespace Injact
{
    public abstract class ScriptableObjectInstaller : ScriptableObject, IInstaller
    {
        [Inject] public DiContainer Container { get; } = null!;

        public abstract void InstallBindings();
    }
}