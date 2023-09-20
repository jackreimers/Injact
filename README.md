# Injact

Injact is a simple, easy to use dependency injection container for Godot 4 written in C#.

## 📦 Installation
> **Note**  
> Self contained package/plugin is planned.
1. Clone the repository into somewhere in your projects `res://` directory, I recommend a separate folder such as `res://addons`.

## 🛠️ Project Setup
1. Add a node to your scene and attach the `Context.cs` script to it.
2. Set your desired settings for your scene context in the inspector.
3. Create an installer and add it to the context - see [Installers](#-installers) for more information.

## 🧩 Context
The context acts as the root of the dependency injection container, it is responsible for creating the container and binding installers to it.  
There should never be more than one context in a scene.

### Settings 
`Search For Nodes`  
- If enabled, Injact will search for all nodes in the scene and attempt to inject into them.  
- Default: `true`

`Search For Installers`  
- If enabled, Injact will search for installers in the scene and add them to the context.
- Default: `false`

> **Note**  
> If you are using `Inject Into Nodes` or `Search For Installers` the container will need to search the entire scene tree which will not be performant on larger projects.  
> Consider manually setting nodes and installers if you are experiencing performance issues.

`Nodes`  
- A list of nodes to be injected into at startup.
- Leave empty if using `Search for Nodes`.

`Installers`  
- A list of the node installers to be used by the context, see [Installers](#-installers) for more information.
- Leave empty if using `Search for Installers`.
  
`Logging Levels`
- Used to set the logging level of the container.
- Default: `none`
  
`Profiling Level`
- Used to set the profiling level of the container.
- Default: `none`

## 🧰 Installers
There are two types of installers used to bind dependencies to the container.

`Installer` 
- Intended for binding native C# classes to the container.
- Does not interact with the Godot editor and can only be created in code.  

`NodeInstaller` 
- Intended for binding Godot nodes to the container, but can also be used for native C# classes.
- Interacts with the Godot editor and must exist as a node in the scene tree.

#### Creating an Installer
1. Create a new class that inherits from `Installer` or `NodeInstaller`.
2. Override the `InstallBindings` method.

#### Creating Bindings
Classes can be bound to containers using the following syntax.

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

## 🪄 Bindings
When binding classes or objects you can use chained methods to specify how the object should be bound to the container.  
Below are all the available methods and their use cases.

### Bind\<TConcrete\>  
Bindings set via `Bind<TConcrete>` can only be resolved via `TConcrete`.
- `TConcrete` must be a non-abstract concrete class.

```csharp
public override void InstallBindings()
{
    Container.Bind<MyClass>();
}
```

### Bind\<TInterface, TConcrete\>
Bindings set via `Bind<TInterface, TConcrete>` can be resolved via `TInterface` or `TConcrete`.
- `TInterface` can be any interface that `TConcrete` implements.
- `TConcrete` must be a non-abstract concrete class.

```csharp
public override void InstallBindings()
{
    Container.Bind<IClass, MyClass>();
}
```

### BindFactory\<TFactory, TObject\>
Used to bind a factory to the container.
- `TFactory` must be a class that inherits from `Factory<TObject>` and **cannot** be an interface.
- `TObject` is the object the factory will create.

See [Factories](#-factories) for more information.

### WhenInjectedInto\<TValue>
Used to control what classes a binding can be resolved by. 
- If the binding is requested by a type not specified in `WhenInjectedInto` the injection will fail.
- Can be chained to add multiple classes.

```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .WhenInjectedInto<MyClass>()
        .WhenInjectedInto(typeof(MyOtherClass)); 
}
```

### FromInstance
Used to bind an already created object to the container.
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
Similar to `FromInstance` but for Godot nodes.
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
Used to set the binding lifetime to singleton.
- All classes that request this type will get the same instance.

```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .AsSingleton();
}
```

### AsTransient
Sets the binding lifetime to transient.
- All classes that request this type will get a new instance.
- This is default and calling it is redundant, but can help with readability.

```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .AsTransient();
}
```

### Immediate
Used to set the bound object to be created immediately. 
- Used in conjunction with `AsSingleton`.
- When a binding is set to `Immediate` an instance of it will be created immediately and not when it is first requested.

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
Used to set the bound object to be created only when it is first requested.
- Used in conjunction with `AsSingleton`. 
- When a binding is set to `Delayed` an instance of it will not be created until it is first requested.
- This is default and calling it is redundant, but can help with readability.

```csharp
public override void InstallBindings()
{
    Container
        .Bind<IClass, MyClass>()
        .AsSingleton()
        .Delayed();
}
```

## 💉 Injection
Injact supports constructor, field, property and method injection via the `[Inject]` and `[InjectOptional]` attributes.  
There are different use cases for each type, but ultimately it is up to you which you use.

### Attributes
- If the requested type is not found when using `[Inject]` an exception will be thrown.
- If the requested type is not found when using `[InjectOptional]` the injected value will be null and no exception will be thrown.

### Key Points
- Injection occurs on `_EnterTree()`. 
- You should not attempt to access injected values until or after `_Ready()`.
- Fields, properties and methods **do not** need to be public to be injected into.
- Properties **do not** need a setter to be injected into.
- Fields **can** be injected into if they are readonly. 

### Limitations️
- Constructor injection **is not** supported for Godot nodes.
- Injection will not occur if an object is created using `new` instead of being created by the container - see [Factories](#-factories) for more information.

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
> Properties can be injected into regardless of whether or not they have a setter.  

Decorate a property with `[Inject]` or `[InjectOptional]` to have it injected into.

```csharp
public class MyClass : Node
{
    //Both of these properties will be injected into
    [Inject] public IClass Property1 { get; }
    [Inject] public IOtherClass Property2 { get; set; }
}
```

### Method Injection
Decorate a method of any type or name with `[Inject]` or `[InjectOptional]` to have it injected into.

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

### Lazy Injection
Lazy injection is not currently supported but is a planned feature.

## 🏭 Factories
Factories can be used to create objects at runtime whilst ensuring they get their dependencies injected.  
If you are creating an object that expects dependencies at runtime, you should always use a factory to create it.

### Creating a Factory
You can create a factory similarly to creating an installer.

1. Create a class that inherits from `Factory<TValue>`.
2. Provide the type of object the factory will create as a generic type parameter.

```csharp
public class MyClass 
{
    //Code removed for brevity...
    
    Factory : Factory<MyClass> { }
}
```
> **Note**  
> In this example the factory is created as an inner class of `MyClass`, but it can be created anywhere.  

3. Bind the factory to the container.

```csharp
public override void InstallBindings()
{
    Container.BindFactory<MyClass.Factory>();
}
```

### Using Factories
Once a factory is bound to the container, you can inject it into any class and use it to create objects.  
Factories can be resolved using their concrete type or their interface, i.e. `MyClass.Factory` or `IFactory<MyClass>`.

#### Creating Objects
Once the factory has been resolved, call the `Create()` method to return a new object with its dependencies injected.

```csharp
public class MyClass : Node
{
    [Inject] private readonly IFactory<MyClass> _factory;
    
    public MyClass CreateMyClass()
    {
        return _factory.Create();
    }
}
```

#### Creating Objects with Parameters
Passing parameters to factories during creation is not currently supported but is a planned feature.

### Custom Factories
If you need to create a factory that does something more complex than just creating an object, you can create a custom factory by inheriting from `Factory<T>` and overriding the `Create()` method, or implementing `IFactory<T>`.
