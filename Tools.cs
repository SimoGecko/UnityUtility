// (c) Simone Guggiari 2020-2022

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using UnityEngine;

////////// PURPOSE: Various Tools functions //////////

namespace sxg
{
    public static class Tools
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Print Num Children")]
        public static void PrintNumberOfChildren()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            int numChildren = selected.transform.childCount;
            Debug.Log($"{selected.name} has {numChildren} children");
        }

        [UnityEditor.MenuItem("Tools/Clean Transforms")]
        public static void CleanTransforms()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            selected.transform.ForeachDescendant(t => t.Clean(0.001f), true);
        }

#endif // UNITY_EDITOR
    }
}