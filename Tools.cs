﻿// (c) Simone Guggiari 2020-2024

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
#if SNETCODE
using Unity.Netcode;
#endif

////////// PURPOSE: Various Tools functions //////////

namespace sxg
{
    public static class Tools
    {
#if SEDITOR
        [MenuItem("Tools/Print Num Children")]
        public static void PrintNumberOfChildren()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            int numChildren = selected.transform.childCount;
            Debug.Log($"{selected.name} has {numChildren} children");
        }

        [UnityEditor.MenuItem("Tools/Print Num Descendants")]
        public static void PrintNumberOfDescendants()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            int numDescendants = selected.transform.DescendantCount();
            Debug.Log($"{selected.name} has {numDescendants} descendants");
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
            Transform selected = UnityEditor.Selection.activeGameObject.transform;
            List<Vector3> pos = new();
            List<Quaternion> rot = new();
            foreach (var t in selected.GetDescendants(includeSelf: true))
            {
                pos.Add(t.position);
                rot.Add(t.rotation);
            }
            int i = 0;
            foreach (var t in selected.GetDescendants(includeSelf: true))
            {
                t.localScale = Vector3.one;
                t.position = pos[i];
                t.rotation = rot[i];
                ++i;
            }
        }

        [MenuItem("Tools/Sort Children/Name")]
        public static void SortChildren()
        {
            foreach (GameObject go in Selection.gameObjects)
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
                Utility.PrintChildren(go.transform, sb, false);
                Debug.Log(sb.ToString());
            }
        }
        [MenuItem("Tools/Print Descendants")]
        public static void PrintDescendants()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                StringBuilder sb = new();
                Utility.PrintChildren(go.transform, sb, true);
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
        [MenuItem("Tools/Transform/Round To 1_16")]  public static void RoundTo1_16(){ RoundTo(1f / 16f); }
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
                    Utility.MRound(go.transform, fraction);
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
        [MenuItem("Tools/Splay Children/Custom")] public static void SplayChildren_Custom() { SplayChildren(0.5f, 10); }


        public static void SplayChildren(float amount, int rows = -1)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                Utility.SplayChildren(go.transform, amount, rows);
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

        [MenuItem("Tools/Recalculate Skinned Mesh Bounds")]
        public static void RecalcSkinnedBounds()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                SkinnedMeshRenderer sk = go.GetComponent<SkinnedMeshRenderer>();
                Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
                if (sk != null)
                {
                    //Transform t = sk.rootBone;
                    Bounds b = sk.sharedMesh.bounds;
                    sk.localBounds = new Bounds(rot * b.center, rot * b.size);
                }
            }
        }

        //[MenuItem("Tools/FindReferences")]
        //public static void FindReferences()
        //{
        //    foreach (GameObject go in Selection.gameObjects)
        //    {
        //        go.transform.ForeachDescendant(t =>
        //        {
        //            //foreach (Component c in t.GetComponents<Component>())
        //            {
        //                UnityEngine.Object[] deps = EditorUtility.CollectDependencies(t.GetComponents<Component>());
        //                foreach (var o in deps)
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
        //    foreach (CutsceneAnimation t in ts)
        //    {
        //        ans += $"{t.gameObject.name}\n";
        //    }
        //    Debug.Log(ans);
        //}

        [MenuItem("Tools/Measure Distance")]
        public static void MeasureDistance()
        {
            if (Selection.gameObjects.Length != 2)
                Debug.Log("Need to select 2 objects to measure the distance");
            float dist = Vector3.Distance(Selection.gameObjects[0].transform.position, Selection.gameObjects[1].transform.position);
            Debug.Log($"Distance={dist}");
        }

        [MenuItem("Assets/Create/Shader/Custom Shader Mine", false, 100)]
        static void CreateCustomShader(MenuCommand menuCommand)
        {
            string editorpath = "c:/Program Files/Unity/Hub/Editor/2022.3.12f1/"; // TODO: find indirection
            string path = "Editor/Data/Resources/ScriptTemplates/";
            string file = "80-Shader__Custom Shader-NewCustomShader.shader.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile($"{editorpath}/{path}/{file}", "NewCustomShader.shader");
        }

        [MenuItem("Tools/Reserialize all Prefabs")]
        private static void ReserializeAllPrefabs()
        {
            // from https://www.reddit.com/r/Unity3D/comments/aabf4u/how_can_you_reserialize_all_prefabs_in_unity_2018x/
            var paths = AssetDatabase.GetAllAssetPaths().Where(path => path.Contains(".prefab"));
            foreach (string path in paths)
            {
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (!PrefabUtility.IsPartOfImmutablePrefab(prefabAsset))
                {
                    PrefabUtility.SavePrefabAsset(prefabAsset);
                }
            }
        }

        [MenuItem("Tools/Reserialize all ScriptableObjects")]
        private static void ReserializeAllScriptableObjects()
        {
            var paths = AssetDatabase.GetAllAssetPaths().Where(path => path.Contains(".asset"));
            //.Where(path => AssetDatabase.LoadAssetAtPath<T>(path) != null);
            AssetDatabase.ForceReserializeAssets(paths);
        }

        [MenuItem("Tools/Reserialize all Yaml Files")]
        private static void ReserializeAllYamlFiles()
        {
            string[] extensions = new string[]
            {
                ".anim", ".asset", ".brush", ".controller", ".flare", ".fontsettings", ".giparams", ".guiskin", ".lighting", ".mask",
                ".mat", ".meta", ".mixer", ".overrideController", ".playable", ".prefab", ".preset", ".renderTexture", ".scenetemplate",
                ".shadervariants", ".signal", ".spriteatlas", ".spriteatlasv2", ".terrainlayer", ".unity", ".physicMaterial", ".physicsMaterial2D",
                // need to also include raw assets
                ".cs", ".png",
            };
            IEnumerable<string> paths = AssetDatabase.GetAllAssetPaths()
                .Where(path => !(path.Contains("3rdParty") || path.StartsWith("Packages/")));
                //.Where(path => extensions.Any(ext => path.EndsWith(ext)))
            AssetDatabase.ForceReserializeAssets(paths);
            Debug.Log($"Reserialized {paths.Count()} paths.");
        }

        [MenuItem("Tools/Apply All Prefab Changes")]
        static void ApplyAllPrefabChanges()
        {
            var prefabs = GetAllPrefabsInScene();
            foreach (GameObject prefab in prefabs)
                PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.AutomatedAction);
        }

        static IEnumerable<GameObject> GetAllPrefabsInScene()
        {
            List<GameObject> list = new();
            void RecurseImpl(GameObject go)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (prefab != null)
                {
                    list.Add(go); // NOTE: not the prefab
                }
                else // only top-level objects
                {
                    foreach (var child in go.transform.GetChildren())
                    {
                        RecurseImpl(child.gameObject);
                    }
                }
            }
            GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in rootGameObjects)
                RecurseImpl(root);
            return list;
        }


#if SNETCODE
        [MenuItem("Tools/Find NetworkObjects")]
        public static void FindNetworkObjects()
        {
            NetworkObject[] objs = GameObject.FindObjectsOfType<NetworkObject>();
            foreach (NetworkObject obj in objs)
            {
                Debug.Log("Network object", obj);
            }
        }

        [MenuItem("Tools/Fix NetworkObjects in Scene")]
        public static void FixNetworkObjectsInScene()
        {
            // from https://forum.unity.com/threads/scene-objects-assigned-same-globalobjectidhash-value.1287302/
            var networkObjects = GameObject.FindObjectsOfType<NetworkObject>(true);
            foreach (var networkObject in networkObjects)
            {
                if (!networkObject.gameObject.scene.isLoaded)
                    continue;

                var serializedObject = new SerializedObject(networkObject);
                var hashField = serializedObject.FindProperty("GlobalObjectIdHash");

                // Ugly hack. Reset the hash and apply it.
                // This implicitly marks the field as dirty, allowing it to be saved as an override.
                hashField.uintValue = 0;
                serializedObject.ApplyModifiedProperties();
                // Afterwards, OnValidate will kick in and return the hash to it's real value, which will be saved now.
            }
        }
#endif

#endif // SEDITOR
    }
}