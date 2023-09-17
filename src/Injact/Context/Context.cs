using System.Diagnostics;
using Godot;
using Injact.Internal;

namespace Injact
{
    public partial class Context : Node
    {
        [Export] private bool injectIntoNodes = true;
        [Export] private bool loggingEnabled = true;
        [Export] private bool profilingEnabled = true;
        [Export] private NodeInstaller[] installers;

        private DiContainer _container;
        private Injector _injector;

        public override void _EnterTree()
        {
            _container = new DiContainer(loggingEnabled, profilingEnabled);
            _injector = _container.Resolve<Injector>(this);

            foreach (var installer in installers)
            {
                _injector.InjectInto(installer);
                installer.InstallBindings();
            }

            _container.ProcessPendingBindings();

            if (injectIntoNodes)
                ResolveAllInScene();

            base._EnterTree();
        }

        private void ResolveAllInScene()
        {
            var profile = GodotHelpers.ProfileIf(profilingEnabled, "Resolved all scene nodes in {0}ms.");
            var nodes = GodotHelpers.GetAllChildNodes(GetTree().Root);
            
            GodotHelpers.PrintIf(loggingEnabled, $"Found {nodes.Count} nodes in scene.");

            foreach (var node in nodes)
                _injector.InjectInto(node);
            
            profile?.Invoke();
        }
    }
}