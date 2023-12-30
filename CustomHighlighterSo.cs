// (c) Simone Guggiari 2023

using System.Collections.Generic;
using UnityEngine;

////////// PURPOSE: Stores data for custom color highlighting. //////////

namespace sxg.hl
{
    public class CustomHighlighterSo : ScriptableObject
    {
        public List<Rule> rules = new();
    }
}