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

        private readonly Queue<IBindingStatement> _pendingBindings = new();

        private Injector injector;

        public DiContainer()
        {
            InstallDefaultBindings();
        }

        private void InstallDefaultBindings()
        {
            injector = new Injector(this);

            Bind<DiContainer>().FromInstance(this).AsSingleton();
            Bind<Injector>().FromInstance(injector).AsSingleton();

            ProcessPendingBindings();
        }

        public ObjectBindingStatement Bind<TInterface>()
        {
            return BindInternal<TInterface, TInterface>();
        }

        public ObjectBindingStatement Bind<TInterface, TConcrete>()
        {
            return BindInternal<TInterface, TConcrete>();
        }

        private ObjectBindingStatement BindInternal<TInterface, TConcrete>()
        {
            Assert.IsAssignable<TInterface, TConcrete>();
            Assert.IsNotExistingBinding(_bindings, typeof(TInterface));

            var bindingInfo = new ObjectBindingStatement
            {
                InterfaceType = typeof(TInterface),
                ConcreteType = typeof(TConcrete),
                BindingType = BindingType.Object
            };

            _pendingBindings.Enqueue(bindingInfo);
            return bindingInfo;
        }

        public FactoryBindingStatement BindFactory<TInterface, TObject>()
        {
            return BindFactoryInternal<TInterface, TObject>();
        }

        private FactoryBindingStatement BindFactoryInternal<TFactory, TObject>()
        {
            Assert.IsNotExistingBinding(_bindings, typeof(TFactory));
            Assert.IsNotExistingBinding(_bindings, typeof(IFactory<TObject>));

            var statement = new FactoryBindingStatement
            {
                InterfaceType = typeof(IFactory<TObject>),
                ConcreteType = typeof(TFactory),
                ObjectType = typeof(TObject),
                BindingType = BindingType.Factory
            };

            _pendingBindings.Enqueue(statement);
            return statement;
        }

        public void ProcessPendingBindings()
        {
            if (_pendingBindings.Count == 0)
                return;

            while (_pendingBindings.Count > 0)
            {
                var bindingStatement = _pendingBindings.Dequeue();
                var binding = new Binding(bindingStatement.ConcreteType, bindingStatement.AllowedInjectionTypes);

                Assert.IsValidBindingStatement(bindingStatement);

                switch (bindingStatement.BindingType)
                {
                    case BindingType.Object:
                    {
                        _bindings.Add(bindingStatement.InterfaceType, binding);

                        //There should never be a non singleton binding that has an instance set
                        if (!bindingStatement.Flags.HasFlag(BindingFlags.Singleton) || bindingStatement is not ObjectBindingStatement concrete)
                            continue;

                        _instances.Add(
                            concrete.InterfaceType,
                            concrete.Instance != null || concrete.Flags.HasFlag(BindingFlags.Immediate)
                                ? concrete.Instance ?? Create(concrete.ConcreteType, null)
                                : null
                        );

                        break;
                    }

                    case BindingType.Factory:
                    {
                        var factoryStatement = bindingStatement as FactoryBindingStatement;
                        Assert.IsNotNull(factoryStatement, $"Failed to cast statement for {bindingStatement.InterfaceType} to {nameof(FactoryBindingStatement)}!");

                        _bindings.Add(factoryStatement!.InterfaceType, binding);
                        _bindings.Add(factoryStatement!.ConcreteType, binding);

                        break;
                    }

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public object Resolve(Type requestedType, Type requestingType, bool throwOnNotFound = true)
        {
            return ResolveInternal<object>(requestedType, requestingType, throwOnNotFound);
        }

        public object Resolve(Type requestedType, object requestingObject, bool throwOnNotFound = true)
        {
            return ResolveInternal<object>(requestedType, requestingObject.GetType(), throwOnNotFound);
        }

        public TInterface Resolve<TInterface>(Type requestingType, bool throwOnNotFound = true)
        {
            return ResolveInternal<TInterface>(typeof(TInterface), requestingType, throwOnNotFound);
        }

        public TInterface Resolve<TInterface>(object requestingObject, bool throwOnNotFound = true)
        {
            return ResolveInternal<TInterface>(typeof(TInterface), requestingObject.GetType(), throwOnNotFound);
        }

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
            var constructor = ReflectionHelpers.GetConstructor(binding.ConcreteType);
            var parameterTypes = constructor.GetParameters();

            Assert.IsNotCircular(_bindings, binding, parameterTypes);

            var parameters = parameterTypes
                .Select(s => Resolve(s.ParameterType, requestingType))
                .ToArray();

            return constructor.Invoke(parameters);
        }
    }
}