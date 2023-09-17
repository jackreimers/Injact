using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public static Action ProfileIf(bool condition, string message)
    {
        if (!condition)
            return null;
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        return () =>
        {
            stopwatch.Stop();
            GD.Print($"[Injact] {string.Format(message, stopwatch.ElapsedMilliseconds)}");
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