﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core;
using UnityEngine;

namespace CombatSystem
{
    public class Sword : MeleeWeapon, ITaggable
    {
        [SerializeField] private List<string> m_Tags = new List<string>() { "Physical" };

        public ReadOnlyCollection<string> tags => m_Tags.AsReadOnly();
    }
}