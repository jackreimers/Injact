namespace Injact
{
    public class Factory<TValue>
    {
        private readonly DiContainer _container;

        public Factory(DiContainer container)
        {
            _container = container;
        }

        public TValue Create()
        {
            return _container.Resolve<TValue>(null);
        }
    }
}