using System;
using System.Collections.Generic;
using System.Linq;
using Injact.Internal;
using UnityEngine;

namespace Injact
{
    public class DiContainer
    {
        private readonly Dictionary<Type, Binding> _bindings = new();
        private readonly Dictionary<Type, object> _instances = new();
        private readonly Queue<BindingStatement> _pendingBindings = new();

        private Injector injector;

        public DiContainer()
        {
            InstallDefaultBindings();
            IsInitialised = true;
        }

        public bool IsInitialised { get; private set; }

        private void InstallDefaultBindings()
        {
            injector = new Injector(this);

            Bind<DiContainer>().FromInstance(this).AsSingleton();
            Bind<Injector>().FromInstance(injector).AsSingleton();

            ProcessPendingBindings();
        }

        public BindingStatement Bind<TInterface>()
            => BindInternal<TInterface, TInterface>();

        public BindingStatement Bind<TInterface, TConcrete>()
            => BindInternal<TInterface, TConcrete>();

        private BindingStatement BindInternal<TInterface, TConcrete>()
        {
            Assert.IsAssignable<TInterface, TConcrete>();
            Assert.IsNotExistingBinding(_bindings, typeof(TInterface));

            var bindingInfo = new BindingStatement
            {
                InterfaceType = typeof(TInterface),
                ConcreteType = typeof(TConcrete)
            };

            _pendingBindings.Enqueue(bindingInfo);
            return bindingInfo;
        }

        public void ProcessPendingBindings()
        {
            if (_pendingBindings.Count == 0)
                return;

            while (_pendingBindings.Count > 0)
            {
                var bindingStatement = _pendingBindings.Dequeue();
                var binding = new Binding(bindingStatement.InterfaceType, bindingStatement.ConcreteType, bindingStatement.AllowedInjectionTypes);

                Assert.IsValidBindingStatement(bindingStatement);

                _bindings.Add(bindingStatement.InterfaceType, binding);

                //There should never be a non singleton binding that has an instance set
                if (!bindingStatement.Flags.HasFlag(BindingFlags.Singleton))
                    continue;

                _instances.Add(
                    bindingStatement.InterfaceType,
                    bindingStatement.Instance != null || bindingStatement.Flags.HasFlag(BindingFlags.Immediate)
                        ? bindingStatement.Instance ?? Create(bindingStatement.ConcreteType, null)
                        : null
                );
            }
        }

        public object Resolve(Type requestedType, Type requestingType, bool throwOnNotFound = true)
            => ResolveInternal<object>(requestedType, requestingType, throwOnNotFound);

        public object Resolve(Type requestedType, object requestingObject, bool throwOnNotFound = true)
            => ResolveInternal<object>(requestedType, requestingObject.GetType(), throwOnNotFound);

        public TInterface Resolve<TInterface>(Type requestingType, bool throwOnNotFound = true)
            => ResolveInternal<TInterface>(typeof(TInterface), requestingType, throwOnNotFound);

        public TInterface Resolve<TInterface>(object requestingObject, bool throwOnNotFound = true)
            => ResolveInternal<TInterface>(typeof(TInterface), requestingObject.GetType(), throwOnNotFound);

        private TInterface ResolveInternal<TInterface>(Type requestedType, Type requestingType, bool throwOnNotFound)
        {
            try
            {
                Assert.IsNotNull(requestedType, "Requested type cannot be null!");
                Assert.IsExistingBinding(_bindings, requestedType);
                Assert.IsLegalInjection(_bindings, requestedType, requestingType);

                var isSingleton = _instances.TryGetValue(requestedType, out var instance);
                var hasInstance = instance != null;

                instance ??= Create(requestedType, requestingType);
                injector.InjectInto(instance);

                if (isSingleton && !hasInstance)
                    _instances[requestedType] = instance;

                return (TInterface)instance;
            }

            catch (DependencyException)
            {
                if (throwOnNotFound)
                    throw;

                return default;
            }
        }

        private object Create(Type requestedType, Type requestingType)
        {
            Assert.IsNotNull(requestedType, $"Requested type cannot be null when calling {nameof(Create)}!");
            Assert.IsExistingBinding(_bindings, requestedType);
            Assert.IsNotAssignable(typeof(MonoBehaviour), requestedType);

            var binding = _bindings[requestedType];
            var constructor = ReflectionUtils.GetConstructor(binding.ConcreteType);
            var parameterTypes = constructor.GetParameters();

            Assert.IsNotCircular(_bindings, binding, parameterTypes);

            var parameters = parameterTypes
                .Select(s => Resolve(s.ParameterType, requestingType))
                .ToArray();

            return constructor.Invoke(parameters);
        }
    }
}