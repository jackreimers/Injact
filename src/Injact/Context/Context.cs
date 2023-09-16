using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Injact
{
    public class Context : MonoBehaviour
    {
        [SerializeField] private List<MonoInstaller> monoInstallers = new();
        [SerializeField] private List<ScriptableObjectInstaller> scriptableInstallers = new();

        private DiContainer _container;
        private Injector _injector;

        public List<Installer> Installers { get; set; } = new();
        public List<MonoInstaller> MonoInstallers => monoInstallers;

        private void Awake()
        {
            _container = new DiContainer();
            _injector = _container.Resolve<Injector>(this);

            foreach (var installer in monoInstallers)
            {
                _injector.InjectInto(installer);
                installer.InstallBindings();
            }

            _container.ProcessPendingBindings();
            ResolveAllInScene();
        }

        private void ResolveAllInScene()
        {
            foreach (var monoBehaviour in FindObjectsOfType<MonoBehaviour>().Where(s => s.enabled))
                _injector.InjectInto(monoBehaviour);
        }
    }
}