namespace Injact
{
    public abstract class Installer
    {
        [Inject] public DiContainer Container { get; set; }

        public abstract void InstallBindings();
    }
}