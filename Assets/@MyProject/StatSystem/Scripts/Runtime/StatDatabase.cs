﻿using System.Collections.Generic;
using UnityEngine;

namespace StatSystem
{
    [CreateAssetMenu(fileName = "StatDatabase", menuName = "StatSystem/StatDatabase", order = 0)]
    public class StatDatabase : ScriptableObject
    {
        public List<StatDefinition> stats = new List<StatDefinition>();
        public List<StatDefinition> attributes = new List<StatDefinition>();
        public List<StatDefinition> primaryStats = new List<StatDefinition>();
    }
}