using System;
using System.Collections.Generic;
using System.Linq;

namespace Injact;

public class Bindings : Dictionary<Type, Binding> { }

public class DiContainer
{
    private readonly Bindings _bindings = new();
    private readonly Dictionary<Type, object> _instances = new();
    private readonly Queue<IBindingStatement> _pendingBindings = new();
    
    private readonly LoggingFlags _loggingFlags;
    private readonly ProfilingFlags _profilingFlags;

    private Injector _injector;
    private ILogger _logger;
    private IProfiler _profiler;
    
    public DiContainer(LoggingFlags loggingFlags, ProfilingFlags profilingFlags)
    {
        _loggingFlags = loggingFlags;
        _profilingFlags = profilingFlags;
        
        InstallDefaultBindings();
    }

    private void InstallDefaultBindings()
    {
        _injector = new Injector(this);
        _logger = new Logger<DiContainer>(_loggingFlags);
        _profiler = new Profiler(_logger, _profilingFlags);

        Bind<DiContainer>()
            .FromInstance(this)
            .AsSingleton();

        Bind<Injector>()
            .FromInstance(_injector)
            .WhenInjectedInto<Context>()
            .AsSingleton();
        
        Bind<IProfiler>()
            .FromInstance(_profiler)
            .AsSingleton();

        ProcessPendingBindings();
    }

    public ObjectBindingStatement Bind<TConcrete>()
    {
        //TODO: Validate that TConcrete is not an interface
        return BindInternal<TConcrete, TConcrete>();
    }

    public ObjectBindingStatement Bind<TInterface, TConcrete>() where TConcrete : TInterface
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
            BindingFlags = BindingFlags.None
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
        Assert.IsNotExistingBinding(_bindings, typeof(TFactory));
        Assert.IsNotExistingBinding(_bindings, typeof(IFactory<TObject>));

        var statement = new FactoryBindingStatement
        {
            InterfaceType = typeof(IFactory<TObject>),
            ConcreteType = typeof(TFactory),
            ObjectType = typeof(TObject),
            BindingFlags = BindingFlags.Factory
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
            
            if (bindingStatement.BindingFlags.HasFlag(BindingFlags.Factory))
            {
                var factoryStatement = (FactoryBindingStatement)bindingStatement;
                Assert.IsValidFactoryBindingStatement(factoryStatement);

                //Add a binding for the factory interface and the concrete type so it can be injected from either
                _bindings.Add(factoryStatement.InterfaceType, binding);
                _bindings.Add(factoryStatement.ConcreteType, binding);
            }

            else
            {
                var objectStatement = (ObjectBindingStatement)bindingStatement;
                Assert.IsValidObjectBindingStatement(objectStatement);
                
                _bindings.Add(bindingStatement.InterfaceType, binding);
                
                //There should never be a non singleton binding that has an instance set
                if (!objectStatement.BindingFlags.HasFlag(BindingFlags.Singleton))
                    continue;

                _instances.Add(
                    objectStatement.InterfaceType,
                    objectStatement.Instance != null || objectStatement.BindingFlags.HasFlag(BindingFlags.Immediate)
                        ? objectStatement.Instance ?? Create(objectStatement.ConcreteType, objectStatement.ConcreteType)
                        : null
                );
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

            if (requestedType.IsAssignableFrom(typeof(ILogger)))
                return ResolveLogger<TInterface>(requestingType);
            
            Assert.IsExistingBinding(_bindings, requestedType);
            Assert.IsLegalInjection(_bindings, requestedType, requestingType);
            
            var isSingleton = _instances.TryGetValue(requestedType, out var instance);
            var hasInstance = instance != null;

            instance ??= Create(requestedType, requestingType);
            _injector.InjectInto(instance);

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

    private TInterface ResolveLogger<TInterface>(Type requestingType)
    {
        Assert.IsNotNull(requestingType, "Requesting type cannot be null when resolving logger!");
        
        var generic = typeof(Logger<>);
        var constructed = generic.MakeGenericType(requestingType);
        var constructor = constructed.GetConstructor(new[] { typeof(LoggingFlags) });

        var hasInstance = _instances.TryGetValue(constructed, out var instance);
        if (hasInstance)
            return (TInterface)instance;
        
        instance = constructor?.Invoke(new object[] { _loggingFlags });
        Assert.IsNotNull(instance, "Failed to create logger instance!");
        
        _instances.Add(constructed, instance);
                
        return (TInterface)instance;
    }

    private object Create(Type requestedType, Type requestingType)
    {
        Assert.IsNotNull(requestedType, $"Requested type cannot be null when calling {nameof(Create)}!");
        Assert.IsExistingBinding(_bindings, requestedType);

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