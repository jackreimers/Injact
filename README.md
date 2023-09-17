# Injact

Injact is a simple, easy to use dependency injection container for Godot 4 written in C#.

## Installation
> **Note**  
> Self contained package/plugin is planned.
1. Clone the repository into somewhere in your projects `res://` directory, I recommend a separate folder such as `res://addons`.

## Project Setup
1. Add a node to your scene and attach the `Context.cs` script to it.
2. Set your desired settings for your scene context in the inspector.
   - `Inject Into Nodes` 
     - If enabled, Injact will attempt to inject dependencies into all nodes in the scene.
     - Default: `true`
   - `Search For Installers` 
     - If enabled, Injact will search for installers in the scene and add them to the context. This saves you having to manually set installers in the inspector, but will be less performant on larger projects.
     - Default: `false`
   - `Installers` 
     - A list of the node installers to be used by the context, see below for more information.
     - Leave empty if using `Search for Installers`.
   - `Logging Enabled` 
     - If enabled, Injact will log general information to the console.
     - Default: `false`
   - `Profiling Enabled` 
     - If enabled, Injact will log performance information to the console.
     - Default: `false`
3. Create an installer and add it to the context - see [Installers](#installers) for more information.

## Installers
There are two types of installers used to bind dependencies to the container.

`Installer` 
- Used for binding native C# classes to the container.
- Do not interact with the Godot editor and can only be created in code.  

`NodeInstaller` 
- Used for binding Godot nodes to the container.
- Interacts with the Godot editor and must exist as a node in the scene tree.

#### Creating an Installer
1. Create a new class that inherits from `Installer` or `NodeInstaller`.
2. Override the `InstallBindings` method.

#### Binding Classes to the Container
Classes can be bound to containers using the following syntax:

```csharp
public class MyInstaller : Installer
{
    public override void InstallBindings()
    {
        // Bind a class to the container.
        Container.Bind<MyClass>();
    }
}
```

```csharp
public class MyNodeInstaller : NodeInstaller
{
    [Export] private Node myNode;
    
    public override void InstallBindings()
    {
        // Bind a class to the container.
        Container
            .Bind<NodeType>()
            .FromNode(myNode);
    }
}
```

## Bindings
When binding classes or objects you can use chained methods to specify how the object should be bound to the container.  

### Bind\<TConcrete\>  
- `TConcrete` must be a non-abstract concrete class.
- Binding can only be resolved via `TConcrete`.

```csharp
public override void InstallBindings()
{
    Container.Bind<MyClass>();
}
```

### Bind\<TInterface, TConcrete\>
- `TInterface` can be any interface that `TConcrete` implements.
- `TConcrete` must be a non-abstract concrete class.
- Binding can be resolved via `TInterface` or `TConcrete`.
```csharp
public override void InstallBindings()
{
    Container.Bind<IClass, MyClass>();
}
```

### FromInstance
- Binds an existing instance of an object to the container.
- Instance bindings must be singletons and calling `FromInstance` will automatically set the binding as a singleton.
```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .FromInstance(new MyClass());
}
```

### FromNode
- Similar to `FromInstance` but for Godot nodes.
- The script attached to the node must be assignable to the type being bound.
- Node bindings must be singletons and calling `FromNode` will automatically set the binding as a singleton.
```csharp
public class MyNodeInstaller : NodeInstaller
{
    [Export] private Node myNode;
    
    public override void InstallBindings()
    {
        Container
            .Bind<IClass>()
            .FromNode(myNode);
    }
}
```

### AsSingleton
- Sets the binding lifetime to singleton, meaning all classes that request this type will get the same instance.
```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .AsSingleton();
}
```

### AsTransient
- Sets the binding lifetime to transient, meaning all classes that request this type will get a new instance.
- This is default and calling `AsTransient` is redundant, but can help with readability.
```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .AsTransient();
}
```

### Immediate
- Used in conjunction with `AsSingleton`.
- When a binding is set to `Immediate` an instance of it will be created immediately and not when it is first requested.
- This can be useful when you need to create the object as soon as the game starts, even if there is nothing that is requesting it.
```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .AsSingleton()
        .Immediate();
}
```

### Delayed
- When a binding is set to `Delayed` an instance of it will not be created until it is first requested.
- This is default and calling `Delayed` is redundant, but can help with readability.
```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .AsSingleton()
        .Delayed();
}
```

## Injection
Injact supports constructor, field, property and method injection via the `[Inject]` and `[InjectOptional]` attributes.  
There are different use cases for each type, but ultimately it is up to you which you use.

### Key Points
#### Lifecycle
- Injection occurs on `_EnterTree()`. 
- You should not attempt to access injected values until or after `_Ready()`.

#### Members
- Fields, properties and methods **do not** need to be public to be injected into.
- Properties **do not** need a setter to be injected into.
- Fields **can** be injected into if they are readonly. 

#### Requirements
- When using `[Inject]` the type must be bound to the container else an exception will be thrown.
- When using `[InjectOptional]` the type does not need to be bound to the container, and the injected value will be null if not found.

#### Limitations
- Constructor injection **is not** supported for Godot nodes.
- Injection will not occur if an object is created using `new` instead of being created by the container - see [Factories](#factories) for more information.

### Constructor Injection
> **Warning**  
> Constructor injection **is not** supported for Godot nodes.

When using constructor injection you do not need to add a `[Inject]` attribute to the constructor, unless you have multiple constructors.  
By default, Injact will use the constructor with the most parameters.

```csharp
public class MyClass
{
    private readonly IClass _class;
    
    public MyClass(IClass class)
    {
        _class = class;
    }
}
```

In the above example, provided the required binding is set up, `MyClass` will be injected with an instance of `IClass` when it is created.

### Field Injection
Decorate a field with `[Inject]` or `[InjectOptional]` to have it injected into.

```csharp
public class MyClass : Node
{
    [Inject] private readonly IClass _class;
}
```

### Property Injection
> **Note**  
> Properties can be injected into regardless of whether they have a setter.  

Decorate a property with `[Inject]` or `[InjectOptional]` to have it injected into.

```csharp
public class MyClass : Node
{
    [Inject] public IClass Class { get; set; }
}
```

### Method Injection
Decorate a method with `[Inject]` or `[InjectOptional]` to have it injected into.  
The method can be named anything.

```csharp
public class MyClass : Node
{
    private IClass _class;
    
    [Inject] 
    public void Inject(IClass injected) 
    {
        _class = injected;
    }
}
```

## Factories
Factories can be used to create objects at runtime whilst ensuring they get their dependencies injected.  
If you are creating an object that expects dependencies at runtime, you should always use a factory to create it.