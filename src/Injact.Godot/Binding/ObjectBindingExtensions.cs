namespace Injact.Godot;

public static class ObjectBindingBuilderExtensions
{
    public static ObjectBindingBuilder FromNode(this ObjectBindingBuilder builder, Node node)
    {
        return builder.FromInstance(node);
    }
}
