﻿using System.Collections.Generic;
using Godot;

namespace Injact.Godot.Utility;

public class GodotHelpers
{
    public static Node[] GetAllChildNodes(Node startNode)
    {
        var nodes = new List<Node>();
        GetAllChildNodesRecursive(startNode, nodes);
        return nodes.ToArray();
    }

    private static void GetAllChildNodesRecursive(Node startNode, List<Node> nodes)
    {
        var childNodes = startNode.GetChildren();
        nodes.AddRange(childNodes);

        foreach (var childNode in childNodes)
            GetAllChildNodesRecursive(childNode, nodes);
    }
}