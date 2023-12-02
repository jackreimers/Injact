namespace Injact.Godot;

public static class ObjectBindingStatementExtensions
{
    public static ObjectBindingStatement FromNode(this ObjectBindingStatement binding, Node node)
    {
        Guard.Against.Null(node, $"Null node reference on {binding.InterfaceType.Name} binding!");
            
        binding.Flags |= StatementFlags.Singleton;
        binding.Instance = node;
        return binding;
    }
}