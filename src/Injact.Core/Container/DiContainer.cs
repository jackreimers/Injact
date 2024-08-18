namespace Injact;

public class Bindings : Dictionary<Type, Binding> { }

public class Instances : Dictionary<Type, object?> { }

public class DiContainer
{
    private readonly ILogger _logger;
    private readonly IProfiler _profiler;
    private readonly ContainerOptions _containerOptions;
    private readonly Bindings _bindings = new();
    private readonly Instances _instances = new();
    private readonly Queue<IBindingStatement> _pendingBindings = new();

    private Injector injector = null!;

    /// <summary>
    /// Create a new instance of the dependency injection container.
    /// </summary>
    public DiContainer()
        : this(new ContainerOptions { LoggingProvider = new DefaultLoggingProvider() }) { }

    /// <summary>
    /// Create a new instance of the dependency injection container.
    /// </summary>
    /// <param name="containerOptions">Settings to control container features.</param>
    public DiContainer(ContainerOptions containerOptions)
    {
        _logger = containerOptions.LoggingProvider.GetLogger<DiContainer>();
        _profiler = new Profiler(_logger);
        _containerOptions = containerOptions;

        InstallDefaultBindings();
    }

    private void InstallDefaultBindings()
    {
        injector = new Injector(this);

        Bind<DiContainer>()
            .FromInstance(this)
            .AsSingleton()
            .Finalise();

        Bind<Injector>()
            .FromInstance(injector)
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
        _logger.LogInformation("Dependency injection container initialised.", _containerOptions.LogDebugging);
    }

    /// <summary>
    /// Add options from a JSON file to the container.
    /// </summary>
    /// <param name="section">The name of the section in the JSON file.</param>
    /// <param name="path">The JSON file path relative to the project root.</param>
    /// <typeparam name="T">The options object to deserialise into.</typeparam>
    /// <remarks>Method will search for appsettings.json in the project root if path is null.</remarks>
    public void AddOptions<T>(string? section = null, string? path = null)
    {
        var workingDirectory = Environment.CurrentDirectory;
        var appsettingsPath = !string.IsNullOrWhiteSpace(path)
            ? Path.Combine(workingDirectory, path)
            : Path.Combine(workingDirectory, "appsettings.json");

        if (!File.Exists(appsettingsPath))
        {
            throw new OptionsExeption($"No JSON file found at \"{appsettingsPath}\".");
        }

        try
        {
            var file = File.ReadAllText(appsettingsPath);
            var json = JsonDocument.Parse(file);
            var value = json.RootElement;

            if (section != null)
            {
                var properties = section.Split(':');

                foreach (var property in properties)
                {
                    if (!value.TryGetProperty(property, out var next))
                    {
                        throw new OptionsExeption($"Section \"{section}\" not found in JSON file.");
                    }

                    value = next;
                }
            }

            //TODO: This is failing silently when the section does not exist in the file
            var options = value.Deserialize<T>();
            if (options == null)
            {
                throw new OptionsExeption($"No options found for section \"{section}\".");
            }

            Bind<IOptions<T>>()
                .FromInstance(new Options<T>(options))
                .AsSingleton()
                .Finalise();

            ProcessPendingBindings();
        }

        catch (Exception exception)
        {
            throw new OptionsExeption($"Failed to load options for section \"{section}\".\n{exception.Message}");
        }
    }

    /// <summary>
    /// Create a binding using a concrete type.
    /// </summary>
    /// <typeparam name="TConcrete">The type of the object being bound.</typeparam>
    public ObjectBindingBuilder Bind<TConcrete>()
        where TConcrete : class
    {
        return BindInternal<TConcrete, TConcrete>();
    }

    /// <summary>
    /// Create a binding using an interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface the object is being bound as.</typeparam>
    /// <typeparam name="TConcrete">The concrete type of the object being bound.</typeparam>
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

        return new ObjectBindingBuilder(BindCallback).WithType<TInterface, TConcrete>();
    }

    /// <summary>
    /// Create a factory binding using a concrete type.
    /// </summary>
    /// <typeparam name="TFactory">The type of the factory being bound.</typeparam>
    /// <typeparam name="TObject">The type of object the factory creates.</typeparam>
    public FactoryBindingBuilder BindFactory<TFactory, TObject>()
        where TFactory : IFactory
    {
        return BindFactoryInternal<TFactory, TFactory, TObject>();
    }

    /// <summary>
    /// Create a factory binding using an interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface the factory is being bound as.</typeparam>
    /// <typeparam name="TFactory">The type of the factory being bound.</typeparam>
    /// <typeparam name="TObject">The type of object the factory creates.</typeparam>
    /// <returns></returns>
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

    /// <summary>
    /// Create bindings for all pending binding statements.
    /// </summary>
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

            try
            {
                if (bindingStatement.Flags.HasFlag(StatementFlags.Factory))
                {
                    var factoryBindingStatement = Guard.Against.InvalidFactoryBindingStatement(bindingStatement);
                    _bindings.Add(factoryBindingStatement.InterfaceType, binding);
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

            catch (ArgumentException exception)
            {
                throw new DependencyException($"Binding already exists for {bindingStatement.InterfaceType}!", exception);
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

        injector.InjectInto(pendingInjections);
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
            if (requestedType.IsAssignableTo(typeof(IFactory)) && _containerOptions.UseAutoFactories && !_bindings.ContainsKey(requestedType))
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
        var constructed = _containerOptions.LoggingProvider.GetLoggerType().MakeGenericType(requestingType);
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
        if (!_containerOptions.UseAutoFactories)
        {
            Guard.Against.Condition(!_bindings.ContainsKey(requestedType), $"Type {requestedType} not bound!");
        }

        //TODO: This is not performing any caching, should probably do that
        var type = requestedType.GetGenericArguments().First();
        var factoryType = typeof(Factory<>).MakeGenericType(type);

        return (TInterface)Create(factoryType);
    }

    /// <summary>
    /// Create a new instance of a type.
    /// </summary>
    /// <param name="requestedType">The type to be created.</param>
    /// <param name="deferInitialisation">If true and the object implements <see cref="ILifecycleObject"/>, the container will not call Awake or Start.</param>
    /// <remarks>It's recommended to use a factory instead of calling this method directly.</remarks>
    public object Create(Type requestedType, bool deferInitialisation)
    {
        return Create(requestedType, deferInitialisation, Array.Empty<object>());
    }

    /// <summary>
    /// Create a new instance of a type.
    /// </summary>
    /// <param name="requestedType">The type to be created.</param>
    /// <param name="args">Arguments to be passed to the constructor of the created object.</param>
    /// <remarks>It's recommended to use a factory instead of calling this method directly.</remarks>
    public object Create(Type requestedType, params object[] args)
    {
        return Create(requestedType, false, args);
    }

    /// <summary>
    /// Create a new instance of a type.
    /// </summary>
    /// <param name="requestedType">The type to be created.</param>
    /// <param name="deferInitialisation">If true and the object implements <see cref="ILifecycleObject"/>, the container will not call Awake, Start or Enable.</param>
    /// <param name="args">Arguments to be passed to the constructor of the created object.</param>
    /// <remarks>It's recommended to use a factory instead of calling this method directly.</remarks>
    public object Create(Type requestedType, bool deferInitialisation, params object[] args)
    {
        Guard.Against.Condition(requestedType.IsInterface, "Cannot create an instance of an interface!");
        Guard.Against.CircularDependency(_instances, requestedType);

        //TODO: Validate args against constructor parameters and warn when there are mismatches
        var typedArgs = args.ToDictionary(s => s.GetType(), s => s);
        var typedArgsWithInterfaces = typedArgs.ToDictionary(s => s.Key, s => s.Value);
        var ignoredInterfaces = new List<Type>();

        Guard.Against.Condition(args.Length != typedArgs.Count, "Cannot pass duplicate argument types to create method!");

        foreach (var type in typedArgs)
        {
            foreach (var implemented in type.Key.GetInterfaces())
            {
                if (ignoredInterfaces.Contains(implemented))
                {
                    continue;
                }

                if (typedArgsWithInterfaces.ContainsKey(implemented))
                {
                    typedArgsWithInterfaces.Remove(implemented);
                    ignoredInterfaces.Add(implemented);
                    _logger.LogWarning($"Two or more provided arguments implement {implemented.Name}. This interface will be ignored.");

                    continue;
                }

                typedArgsWithInterfaces.Add(implemented, type.Value);
            }
        }

        var constructor = ReflectionHelper.GetConstructor(requestedType, args.Select(s => s.GetType()));
        var parameterInfos = constructor.GetParameters();
        var parameterTypes = parameterInfos
            .Select(s => s.ParameterType)
            .ToArray();

        var parameters = new List<object>();

        foreach (var type in parameterTypes)
        {
            var targetType = typedArgsWithInterfaces
                .Where(s => s.Key.IsAssignableTo(type))
                .Select(s => s.Value)
                .FirstOrDefault();

            parameters.Add(targetType ?? Resolve(type, requestedType));
        }

        var constructed = constructor.Invoke(parameters.ToArray());
        injector.InjectInto(constructed);

        if (constructed is not LifecycleObject lifecycleObject)
        {
            return constructed;
        }

        var lifecycleType = typeof(LifecycleObject);

        var updateMethod = Guard.Against.Null(requestedType.GetMethod("Update"));
        if (updateMethod.DeclaringType != lifecycleType)
        {
            var property = Guard.Against.Null(lifecycleType.GetField("_shouldRunUpdate", BindingFlags.NonPublic | BindingFlags.Instance));
            property.SetValue(constructed, true);
        }

        if (deferInitialisation)
        {
            return constructed;
        }

        lifecycleObject.Awake();
        lifecycleObject.Start();
        lifecycleObject.Enable();

        return lifecycleObject;
    }
}
