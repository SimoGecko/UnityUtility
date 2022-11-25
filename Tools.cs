// (c) Simone Guggiari 2020-2022

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEditor;

////////// PURPOSE: Various Tools functions //////////

namespace sxg
{
    public static class Tools
    {
#if (SEDITOR && UNITY_EDITOR)
        [MenuItem("Tools/Print Num Children")]
        public static void PrintNumberOfChildren()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            int numChildren = selected.transform.childCount;
            Debug.Log($"{selected.name} has {numChildren} children");
        }

        [MenuItem("Tools/Clean Transforms")]
        public static void CleanTransforms()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            //selected.transform.ForeachDescendant(t => t.Clean(0.001f), true);
        }

        [MenuItem("Tools/Apply Scale")]
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

        [MenuItem("Tools/Sort Children/Name")]
        public static void SortChildren()
        {
            foreach(GameObject go in Selection.gameObjects)
            {
                Utility.SortChildren(go.transform);
            }
        }
        [MenuItem("Tools/Sort Children/Recursive")]
        public static void SortChildrenRecursive()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                Utility.SortChildren(go.transform, recursive: true);
            }
        }

        [MenuItem("Tools/Print Children")]
        public static void PrintChildren()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                StringBuilder sb = new();
                Utility.PrintChildren(go.transform, sb);
                Debug.Log(sb.ToString());
            }
        }

        [MenuItem("Tools/PolygonCollider2D/Symmetrize")]
        public static void MakeSymmetricNope()
        {
            PolygonCollider2D polygonCollider = UnityEditor.Selection.activeGameObject?.GetComponent<PolygonCollider2D>() ?? null;
            if (polygonCollider != null)
            {
                polygonCollider.Symmetrize();
            }
        }

        [MenuItem("Tools/PolygonCollider2D/Flip")]
        public static void MakeFlip()
        {
            PolygonCollider2D polygonCollider = UnityEditor.Selection.activeGameObject?.GetComponent<PolygonCollider2D>() ?? null;
            if (polygonCollider != null)
            {
                polygonCollider.Flip();
            }
        }

        [MenuItem("Tools/Transform/Round To 1")]     public static void RoundTo1_1() { RoundTo(1f); }
        [MenuItem("Tools/Transform/Round To 0.5")]   public static void RoundTo1_2() { RoundTo(1f / 2f); }
        [MenuItem("Tools/Transform/Round To 0.25")]  public static void RoundTo1_4() { RoundTo(1f / 4f); }
        [MenuItem("Tools/Transform/Round To 0.125")] public static void RoundTo1_8() { RoundTo(1f / 8f); }
        [MenuItem("Tools/Transform/Round To 2")]     public static void RoundTo2()   { RoundTo(2f); }
        [MenuItem("Tools/Transform/Round To 4")]     public static void RoundTo4()   { RoundTo(4f); }
        [MenuItem("Tools/Transform/Round To 5")]     public static void RoundTo5()   { RoundTo(5f); }
        [MenuItem("Tools/Transform/Round To 10")]    public static void RoundTo10()  { RoundTo(10f); }

        public static void RoundTo(float fraction)
        {
            GameObject[] selected = UnityEditor.Selection.gameObjects;
            if (selected != null)
            {
                foreach (GameObject go in selected)
                {
                    Utility.RoundTo(go.transform, fraction);
                }
            }
        }

        [MenuItem("Tools/Sort Children/Volume")]
        public static void SortChildrenVolume()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                Utility.SortChildrenByVolume(go.transform);
            }
        }
        [MenuItem("Tools/Splay Children/1")]   public static void SplayChildren_1() { SplayChildren(1f); }
        [MenuItem("Tools/Splay Children/2")]   public static void SplayChildren_2() { SplayChildren(2f); }
        [MenuItem("Tools/Splay Children/5")]   public static void SplayChildren_5() { SplayChildren(5f); }
        [MenuItem("Tools/Splay Children/10")]  public static void SplayChildren_10() { SplayChildren(10f); }
        [MenuItem("Tools/Splay Children/15")]  public static void SplayChildren_15() { SplayChildren(15f); }
        [MenuItem("Tools/Splay Children/25")]  public static void SplayChildren_25() { SplayChildren(25f); }
        [MenuItem("Tools/Splay Children/50")]  public static void SplayChildren_50() { SplayChildren(50f); }
        [MenuItem("Tools/Splay Children/100")] public static void SplayChildren_100() { SplayChildren(100f); }


        public static void SplayChildren(float amount)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                Utility.SplayChildren(go.transform, amount);
            }
        }

        [MenuItem("Tools/Append Childcount")]
        public static void AppendChildcount()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                string name = go.name;
                if (name.IsRegexMatch("(.*) \\(\\d+\\)", out string[] tokens))
                    name = tokens[1];
                go.name = $"{name} ({go.transform.childCount})";
            }
        }

        //[MenuItem("Tools/FindReferences")]
        //public static void FindReferences()
        //{
        //    foreach (GameObject go in Selection.gameObjects)
        //    {
        //        go.transform.ForeachDescendant(t =>
        //        {
        //            //foreach(Component c in t.GetComponents<Component>())
        //            {
        //                UnityEngine.Object[] deps = EditorUtility.CollectDependencies(t.GetComponents<Component>());
        //                foreach(var o in deps)
        //                {
        //                    //EditorUtility.ReferenceEquals
        //                    //if (!o)
        //                    Debug.Log($"{t.name} has {o.name} dependent on it");
        //                }
        //            }
        //        }, true);
        //    }
        //}

        // NOTES: Simply use FindReferencesInScene
        //[MenuItem("Tools/Find All Components")]
        //public static void FindAllComponents()
        //{
        //    CutsceneAnimation[] ts = GameObject.FindObjectsOfType<CutsceneAnimation>();
        //    string ans = "";
        //    foreach(CutsceneAnimation t in ts)
        //    {
        //        ans += $"{t.gameObject.name}\n";
        //    }
        //    Debug.Log(ans);
        //}
#endif // (SEDITOR && UNITY_EDITOR)
    }
}