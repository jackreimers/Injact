using UnityEngine;

namespace Injact
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        [Inject] public DiContainer Container { get; set; }

        public abstract void InstallBindings();
    }
}