﻿using AbilitySystem.Nodes;
using Core;
using Core.Editor;
using Core.Editor.Nodes;
using UnityEngine;

namespace AbilitySystem.Editor.Nodes
{
    [NodeType(typeof(AbilityLevelNode))]
    [Title("Ability System", "Ability Level")]
    public class AbilityLevelNodeView : NodeView
    {
        public AbilityLevelNodeView()
        {
            title = "Ability Level";
            node = ScriptableObject.CreateInstance<AbilityLevelNode>();
            output = CreateOutputPort();
        }
    }
}