namespace Injact.Godot;

public static class ObjectBindingStatementExtensions
{
    public static ObjectBindingStatement FromNode(this ObjectBindingStatement binding, Node node)
    {
        binding.Flags |= StatementFlags.Singleton;
        binding.Instance = node;
        
        return binding;
    }
}