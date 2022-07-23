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

        [UnityEditor.MenuItem("Tools/Apply Scale")]
        public static void ApplyScale()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            List<Vector3> pos = new();
            List<Quaternion> rot = new();
            selected.transform.ForeachDescendant(t =>
            {
                pos.Add(t.position);
                rot.Add(t.rotation);
            }, true);
            int i = 0;
            selected.transform.ForeachDescendant(t =>
            {
                t.localScale = Vector3.one;
                t.position = pos[i];
                t.rotation = rot[i];
                ++i;
            }, true);
        }

#endif // UNITY_EDITOR
    }
}