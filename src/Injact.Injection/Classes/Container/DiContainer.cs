using System;
using System.Collections.Generic;
using System.Linq;
using Injact.Engine;
using Injact.Godot.Profiling;
using Injact.Profiling;
using Injact.Utility;
using Strategy.Plugins.Injact.src.Injact.Injection.Classes.Configuration;

namespace Injact.Injection;

public class Bindings : Dictionary<Type, Binding> { }

public class DiContainer
{
    private readonly Bindings _bindings = new();
    private readonly Dictionary<Type, object> _instances = new();
    private readonly Queue<IBindingStatement> _pendingBindings = new();
    private readonly Queue<ObjectBindingStatement> _pendingInstances = new();

    private readonly ILogger _logger;
    private readonly IProfiler _profiler;
    private readonly ContainerOptions _options;

    private Injector _injector;

    public DiContainer() : this(new ContainerOptions()) { }

    public DiContainer(ContainerOptions options)
    {
        _logger = new Logger<DiContainer>();
        _profiler = new Profiler(_logger);
        _options = options;

        InstallDefaultBindings();
    }

    private void InstallDefaultBindings()
    {
        _injector = new Injector(this);

        Bind<DiContainer>()
            .FromInstance(this)
            .AsSingleton();

        Bind<Injector>()
            .FromInstance(_injector)
            .AsSingleton();

        Bind<EditorValueMapper>()
            .AsSingleton();

        Bind<IProfiler>()
            .FromInstance(_profiler)
            .AsSingleton();

        ProcessPendingBindings();
    }

    public ObjectBindingStatement Bind<TConcrete>() where TConcrete : class
    {
        return BindInternal<TConcrete, TConcrete>();
    }

    public ObjectBindingStatement Bind<TInterface, TConcrete>() where TConcrete : class, TInterface
    {
        return BindInternal<TInterface, TConcrete>();
    }

    private ObjectBindingStatement BindInternal<TInterface, TConcrete>()
    {
        Guard.Against.Unassignable<TInterface, TConcrete>();
        Guard.Against.ExistingBinding(_bindings, typeof(TInterface));

        var bindingInfo = new ObjectBindingStatement
        {
            InterfaceType = typeof(TInterface),
            ConcreteType = typeof(TConcrete),
            Flags = StatementFlags.None
        };

        _pendingBindings.Enqueue(bindingInfo);
        return bindingInfo;
    }

    public FactoryBindingStatement BindFactory<TFactory, TObject>() where TFactory : IFactory<TObject>
    {
        return BindFactoryInternal<TFactory, TObject>();
    }

    private FactoryBindingStatement BindFactoryInternal<TFactory, TObject>()
    {
        Guard.Against.ExistingBinding(_bindings, typeof(TFactory));
        Guard.Against.ExistingBinding(_bindings, typeof(IFactory<TObject>));

        var statement = new FactoryBindingStatement
        {
            InterfaceType = typeof(IFactory<TObject>),
            ConcreteType = typeof(TFactory),
            ObjectType = typeof(TObject),
            Flags = StatementFlags.Factory
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

            if (bindingStatement.Flags.HasFlag(StatementFlags.Factory))
            {
                var factoryStatement = (FactoryBindingStatement)bindingStatement;
                Guard.Against.InvalidFactoryBindingStatement(factoryStatement);

                //Add a binding for the factory interface and the concrete type so it can be injected from either
                _bindings.Add(factoryStatement.InterfaceType, binding);
                _bindings.Add(factoryStatement.ConcreteType, binding);
            }

            else
            {
                var objectStatement = (ObjectBindingStatement)bindingStatement;
                Guard.Against.InvalidObjectBindingStatement(objectStatement);

                _bindings.Add(bindingStatement.InterfaceType, binding);

                //There should never be a non singleton binding that has an instance set
                if (!objectStatement.Flags.HasFlag(StatementFlags.Singleton))
                    continue;

                if (objectStatement.Flags.HasFlag(StatementFlags.Immediate) && objectStatement.Instance == null)
                {
                    _pendingInstances.Enqueue(objectStatement);
                    continue;
                }

                _instances.Add(
                    objectStatement.InterfaceType,
                    objectStatement.Instance
                );
            }
        }

        //Perform creation after all bindings have been processed to prevent immediate bindings from requesting unbound dependencies
        while (_pendingInstances.Count > 0)
        {
            var statement = _pendingInstances.Dequeue();

            _instances.Add(
                statement.InterfaceType,
                Create(statement.ConcreteType)
            );
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
            Guard.Against.Null(requestedType, "Requested type cannot be null!");
            Guard.Against.IllegalInjection(_bindings, requestedType, requestingType);

            if (requestedType.IsAssignableFrom(typeof(ILogger)))
                return ResolveLogger<TInterface>(requestingType);

            if (requestedType.IsAssignableFrom(typeof(IFactory)))
                return ResolveFactory<TInterface>(requestingType);

            Guard.Against.MissingBinding(_bindings, requestedType);
            var binding = _bindings[requestedType];

            //A singleon will always have an entry in this dictionary, even if the value is null
            var isSingleton = _instances.TryGetValue(requestedType, out var instance);
            if (isSingleton)
            {
                if (instance == null)
                    _instances[requestedType] = Create(binding.ConcreteType);
                
                return (TInterface)_instances[requestedType];
            }
            
            return (TInterface)Create(binding.ConcreteType);
        }

        catch (DependencyException)
        {
            if (throwOnNotFound)
                throw;

            return default;
        }
    }
    
    private TInterface ResolveLogger<TInterface>(Type requestingType)
    {
        Guard.Against.Null(requestingType, "Requesting type cannot be null when resolving logger!");

        var constructed = typeof(Logger<>).MakeGenericType(requestingType);
        var constructor = constructed.GetConstructors().FirstOrDefault();

        var hasInstance = _instances.TryGetValue(constructed, out var instance);
        if (hasInstance)
            return (TInterface)instance;

        instance = constructor?.Invoke(null);
        Guard.Against.Null(instance, "Failed to create logger instance!");

        _instances.Add(constructed, instance);

        return (TInterface)instance;
    }

    private TInterface ResolveFactory<TInterface>(Type requestingType)
    {
        Guard.Against.Null(requestingType, "Requesting type cannot be null when resolving factory!");

        if (!_options.AllowOnDemandFactories)
            Guard.Against.MissingBinding(_bindings, typeof(TInterface));
        
        var type = typeof(Factory<>).MakeGenericType()
    }

    public object Create(Type requestedType)
    {
        Guard.Against.Null(requestedType, $"Requested type cannot be null when calling {nameof(Create)}!");
        Guard.Against.CircularDependency(_bindings, requestedType);

        var constructor = ReflectionHelpers.GetConstructor(requestedType);
        var parameterTypes = constructor.GetParameters();

        var parameters = parameterTypes
            .Select(s => Resolve(s.ParameterType, requestedType))
            .ToArray();

        return constructor.Invoke(parameters);
    }
}