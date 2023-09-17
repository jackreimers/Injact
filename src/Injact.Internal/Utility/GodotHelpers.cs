using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Injact.Internal;

public class GodotHelpers
{
    public static void PrintIf(bool condition, string message)
    {
        if (condition)
            GD.Print($"[Injact] {message}");
    }

    public static void WarnIf(bool condition, string message)
    {
        if (condition)
            GD.PushWarning($"[Injact] {message}");
    }

    public static Action<object[]> ProfileIf(bool condition, string message)
    {
        if (!condition)
            return null;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        return (args) =>
        {
            stopwatch.Stop();
            args = args != null
                ? new object[] { stopwatch.ElapsedMilliseconds }.Concat(args).ToArray()
                : new object[] { stopwatch.ElapsedMilliseconds };
            
            GD.Print($"[Injact] {string.Format(message, args)}");
        };
    }

    public static List<Node> GetAllChildNodes(Node startNode)
    {
        var nodes = new List<Node>();
        GetAllChildNodesRecursive(startNode, nodes);
        return nodes;
    }

    private static void GetAllChildNodesRecursive(Node startNode, List<Node> nodes)
    {
        var childNodes = startNode.GetChildren();
        nodes.AddRange(childNodes);

        foreach (var childNode in childNodes)
            GetAllChildNodesRecursive(childNode, nodes);
    }
}