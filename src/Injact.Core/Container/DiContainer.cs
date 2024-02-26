namespace Injact;

public class Bindings : Dictionary<Type, Binding> { }

public class Instances : Dictionary<Type, object?> { }

public class DiContainer
{
    private readonly ILogger _logger;
    private readonly IProfiler _profiler;
    private readonly ContainerOptions _options;

    private readonly Bindings _bindings = new();
    private readonly Instances _instances = new();
    private readonly Queue<IBindingStatement> _pendingBindings = new();

    private Injector _injector = null!;

    public DiContainer() : this(new ContainerOptions { LoggingProvider = new DefaultLoggingProvider() }) { }

    public DiContainer(ContainerOptions options)
    {
        _logger = options.LoggingProvider.GetLogger<DiContainer>();
        _profiler = new Profiler(_logger);
        _options = options;

        InstallDefaultBindings();
    }

    private void InstallDefaultBindings()
    {
        _injector = new Injector(this);

        Bind<DiContainer>()
            .FromInstance(this)
            .AsSingleton()
            .Finalise();

        Bind<Injector>()
            .FromInstance(_injector)
            .AsSingleton()
            .Finalise();

        Bind<EditorValueMapper>()
            .AsSingleton()
            .Finalise();

        Bind<IProfiler>()
            .FromInstance(_profiler)
            .AsSingleton()
            .Finalise();

        ProcessPendingBindings();
        _logger.LogInformation("Dependency injection container initialised.", _options.LogDebugging);
    }

    public ObjectBindingBuilder Bind<TConcrete>()
        where TConcrete : class
    {
        return BindInternal<TConcrete, TConcrete>();
    }

    public ObjectBindingBuilder Bind<TInterface, TConcrete>()
        where TConcrete : class, TInterface
    {
        return BindInternal<TInterface, TConcrete>();
    }

    private ObjectBindingBuilder BindInternal<TInterface, TConcrete>()
        where TConcrete : class, TInterface
    {
        Guard.Against.Assignable<TInterface, IFactory>("Cannot bind factory as object!");
        Guard.Against.Assignable<TConcrete, IFactory>("Cannot bind factory as object!");
        Guard.Against.Condition(_bindings.ContainsKey(typeof(TInterface)), $"Type {typeof(TInterface)} already bound!");

        return new ObjectBindingBuilder(BindCallback).WithType<TInterface, TConcrete>();
    }

    public FactoryBindingBuilder BindFactory<TFactory, TObject>()
        where TFactory : IFactory
    {
        return BindFactoryInternal<TFactory, TFactory, TObject>();
    }

    public FactoryBindingBuilder BindFactory<TInterface, TFactory, TObject>()
        where TInterface : IFactory
        where TFactory : TInterface
    {
        return BindFactoryInternal<TInterface, TFactory, TObject>();
    }

    private FactoryBindingBuilder BindFactoryInternal<TInterface, TFactory, TObject>()
        where TInterface : IFactory
        where TFactory : TInterface
    {
        Guard.Against.Condition(_bindings.ContainsKey(typeof(TInterface)), $"Type {typeof(TInterface)} already bound!");
        Guard.Against.Condition(_bindings.ContainsKey(typeof(TFactory)), $"Type {typeof(TFactory)} already bound!");

        return new FactoryBindingBuilder(BindCallback).WithType<TInterface, TFactory, TObject>();
    }

    private void BindCallback(IBindingStatement statement)
    {
        _pendingBindings.Enqueue(statement);
    }

    public void ProcessPendingBindings()
    {
        if (_pendingBindings.Count == 0)
        {
            return;
        }

        var pendingInstances = new List<ObjectBindingStatement>();
        var pendingInjections = new List<object>();

        while (_pendingBindings.Count > 0)
        {
            var bindingStatement = _pendingBindings.Dequeue();
            var binding = new Binding(bindingStatement.ConcreteType, bindingStatement.AllowedInjectionTypes);

            if (bindingStatement.Flags.HasFlag(StatementFlags.Factory))
            {
                var factoryBindingStatement = Guard.Against.InvalidFactoryBindingStatement(bindingStatement);
                _bindings.Add(factoryBindingStatement.InterfaceType, binding);

                if (bindingStatement.InterfaceType != bindingStatement.ConcreteType)
                {
                    _bindings.Add(factoryBindingStatement.ConcreteType, binding);
                }
            }

            else
            {
                var objectBindingStatement = Guard.Against.InvalidObjectBindingStatement(bindingStatement);
                _bindings.Add(bindingStatement.InterfaceType, binding);

                if (!objectBindingStatement.Flags.HasFlag(StatementFlags.Singleton))
                {
                    continue;
                }

                if (objectBindingStatement.Instance != null)
                {
                    pendingInjections.Add(objectBindingStatement.Instance);
                }

                else if (objectBindingStatement.Flags.HasFlag(StatementFlags.Immediate))
                {
                    pendingInstances.Add(objectBindingStatement);
                }

                _instances.Add(
                    objectBindingStatement.InterfaceType,
                    objectBindingStatement.Instance
                );
            }
        }

        //Perform creation after all bindings have been processed to prevent immediate bindings from requesting unbound dependencies
        foreach (var pending in pendingInstances)
        {
            //Check instance hasn't already been created by another object requesting it
            if (_instances[pending.InterfaceType] != null)
            {
                continue;
            }

            _instances[pending.InterfaceType] = Create(pending.ConcreteType);
        }

        _injector.InjectInto(pendingInjections);
    }

    public TInterface Resolve<TInterface>(Type requestingType)
    {
        return ResolveInternal<TInterface>(typeof(TInterface), requestingType, true)!;
    }

    public TInterface? Resolve<TInterface>(Type requestingType, bool throwOnNotFound)
    {
        return ResolveInternal<TInterface>(typeof(TInterface), requestingType, throwOnNotFound);
    }

    public TInterface Resolve<TInterface>(object requestingObject)
    {
        return ResolveInternal<TInterface>(typeof(TInterface), requestingObject.GetType(), true)!;
    }

    public TInterface? Resolve<TInterface>(object requestingObject, bool throwOnNotFound)
    {
        return ResolveInternal<TInterface>(typeof(TInterface), requestingObject.GetType(), throwOnNotFound);
    }

    public object Resolve(Type requestedType, Type requestingType)
    {
        return ResolveInternal<object>(requestedType, requestingType, true)!;
    }

    public object? Resolve(Type requestedType, Type requestingType, bool throwOnNotFound)
    {
        return ResolveInternal<object>(requestedType, requestingType, throwOnNotFound);
    }

    public object Resolve(Type requestedType, object requestingObject)
    {
        return ResolveInternal<object>(requestedType, requestingObject.GetType(), true)!;
    }

    public object? Resolve(Type requestedType, object requestingObject, bool throwOnNotFound)
    {
        return ResolveInternal<object>(requestedType, requestingObject.GetType(), throwOnNotFound);
    }

    private TInterface? ResolveInternal<TInterface>(Type requestedType, Type requestingType, bool throwOnNotFound)
    {
        if (requestedType.IsAssignableTo(typeof(ILogger)))
        {
            return ResolveLogger<TInterface>(requestingType);
        }

        try
        {
            if (requestedType.IsAssignableTo(typeof(IFactory)) && _options.UseAutoFactories && !_bindings.ContainsKey(requestedType))
            {
                return ResolveFactory<TInterface>(requestedType);
            }

            Guard.Against.IllegalInjection(_bindings, requestedType, requestingType);
            Guard.Against.Condition(!_bindings.ContainsKey(requestedType), $"Type {requestedType} not bound!");

            var binding = _bindings[requestedType];

            var isSingleton = _instances.TryGetValue(requestedType, out var instance);
            if (!isSingleton)
            {
                return (TInterface)Create(binding.ConcreteType);
            }

            if (instance != null)
            {
                return (TInterface)instance;
            }

            var constructed = Create(binding.ConcreteType);
            _instances[binding.ConcreteType] = constructed;

            return (TInterface)constructed;
        }

        catch (DependencyException)
        {
            if (throwOnNotFound)
            {
                throw;
            }

            return default;
        }
    }

    private TInterface ResolveLogger<TInterface>(Type requestingType)
    {
        var constructed = _options.LoggingProvider.GetLoggerType().MakeGenericType(requestingType);
        var constructor = constructed.GetConstructors().FirstOrDefault();

        var hasInstance = _instances.TryGetValue(constructed, out var instance);
        if (hasInstance && instance != null)
        {
            return (TInterface)instance;
        }

        instance = Guard.Against.Null(constructor?.Invoke(null));
        _instances.Add(constructed, instance);

        return (TInterface)instance;
    }

    private TInterface ResolveFactory<TInterface>(Type requestedType)
    {
        if (!_options.UseAutoFactories)
        {
            Guard.Against.Condition(!_bindings.ContainsKey(requestedType), $"Type {requestedType} not bound!");
        }

        //TODO: This is not performing any caching, should probably do that
        var type = requestedType.GetGenericArguments().First();
        var factoryType = typeof(Factory<>).MakeGenericType(type);

        return (TInterface)Create(factoryType);
    }

    public object Create(Type requestedType)
    {
        return Create(requestedType, Array.Empty<object>());
    }

    public object Create(Type requestedType, params object[] args)
    {
        Guard.Against.CircularDependency(_bindings, _instances, requestedType);

        //TODO: Validate args against constructor parameters and warn when there are mismatches
        var typedArgs = args.ToDictionary(s => s.GetType(), s => s);
        var typedArgsWithInterfaces = typedArgs.ToDictionary(s => s.Key, s => s.Value);

        Guard.Against.Condition(args.Length != typedArgs.Count, "Cannot pass duplicate argument types to create method!");

        foreach (var type in typedArgs)
        {
            //TODO: Not sure this will be very performant, try to find a better way
            //TODO: Should also probably validate that there are no duplicate interfaces
            foreach (var implemented in type.Key.GetInterfaces())
            {
                typedArgsWithInterfaces.Add(implemented, type.Value);
            }
        }

        var constructor = ReflectionHelpers.GetConstructor(requestedType, args.Select(s => s.GetType()));
        var parameterInfos = constructor.GetParameters();
        var parameterTypes = parameterInfos
            .Select(s => s.ParameterType)
            .ToArray();

        var parameters = new List<object>();

        foreach (var type in parameterTypes)
        {
            var targetType = typedArgsWithInterfaces
                .Where(s => s.Key.IsAssignableFrom(type))
                .Select(s => s.Value)
                .FirstOrDefault();

            parameters.Add(targetType ?? Resolve(type, requestedType));
        }

        var constructed = constructor.Invoke(parameters.ToArray());
        _injector.InjectInto(constructed);

        return constructed;
    }
}