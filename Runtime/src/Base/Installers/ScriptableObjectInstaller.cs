using UnityEngine;

namespace Injact
{
    public abstract class ScriptableObjectInstaller : ScriptableObject
    {
        [Inject] public DiContainer Container { get; set; }

        public abstract void InstallBindings();
    }
}