// (c) Simone Guggiari 2023

using System;
using UnityEditor;
using UnityEngine;

////////// PURPOSE: Allows you to highlight gameobjects in the Hierarchy and assets in the Project view with a color. //////////

namespace sxg.hl
{
    [InitializeOnLoad]
    public class CustomHighlighter : MonoBehaviour
    {
        static CustomHighlighter()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowOnGui;
            EditorApplication.projectWindowItemOnGUI += HandeProjectWindowOnGui;
        }

        private static void HandleHierarchyWindowOnGui(int instanceID, Rect selectionRect)
        {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            var rule = FindRule(go);
            if (rule != null)
                DrawHighlight(selectionRect, rule);
        }

        private static void HandeProjectWindowOnGui(string guid, Rect selectionRect)
        {
            var rule = FindRule(guid);
            if (rule != null)
                DrawHighlight(selectionRect, rule);
        }

        private static void DrawHighlight(Rect selectionRect, Rule rule)
        {
            Rect newRect = new(selectionRect.position + Vector2.one * 10, Vector2.one * 5.5f);
            EditorGUI.DrawRect(newRect, rule.color);
        }

        private static CustomHighlighterSo GetOrCreateSo()
        {
            const string path = "Assets/Editor/CustomHighlighterData.asset";
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                AssetDatabase.CreateFolder("Assets", "Editor");
            var so = AssetDatabase.LoadAssetAtPath<CustomHighlighterSo>(path);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<CustomHighlighterSo>();
                AssetDatabase.CreateAsset(so, path);
            }
            return so;
        }

        private static string Id(GameObject go) => go?.name ?? null;
        private static string Id(string guid) => guid;

        private static Rule FindRule(GameObject go) => FindRule(r => r.path == Id(go));
        private static Rule FindRule(string guid) => FindRule(r => r.path == Id(guid));

        private static Rule FindRule(System.Predicate<Rule> predicate)
        {
            CustomHighlighterSo so = GetOrCreateSo();
            if (so != null && so.rules != null)
            {
                var rule = so.rules.Find(predicate);
                return rule;
            }
            return null;
        }

        private static CustomHighlighterSo so => GetOrCreateSo();

        public static void SetRule(GameObject go, Color color)
        {
            var rule = FindRule(go);
            if (rule != null)
                rule.color = color;
            else
                so.rules.Add(new(Id(go), color));

            EditorUtility.SetDirty(so);
        }
        public static void SetRule(string guid, Color color)
        {
            var rule = FindRule(guid);
            if (rule != null)
                rule.color = color;
            else
                so.rules.Add(new(Id(guid), color));
            EditorUtility.SetDirty(so);
        }
        public static void ClearRule(GameObject go)
        {
            so.rules.RemoveAll(pair => pair.path == Id(go));
            EditorUtility.SetDirty(so);
        }
        public static void ClearRule(string guid)
        {
            so.rules.RemoveAll(pair => pair.path == Id(guid));
            EditorUtility.SetDirty(so);
        }


        //public static string Id(GameObject go) => go.name;//go.GetInstanceID().ToString();
        //string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID.ToString());
        //Object loadedAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        //static string Id(GameObject go) => go.GetInstanceID().ToString();
    }

    [System.Serializable]
    public class Rule
    {
        public string path;
        public Color color;

        public Rule(string path, Color color)
        {
            this.path = path;
            this.color = color;
        }
    }

    public class CustomHighlighterContextMenu : Editor
    {
        static Color red    => new Color32(255, 59, 48, 255);
        static Color orange => new Color32(255, 149, 2, 255);
        static Color yellow => new Color32(255, 204, 0, 255);
        static Color green  => new Color32(100, 218, 56, 255);
        static Color blue   => new Color32(28, 173, 248, 255);
        static Color purple => new Color32(203, 115, 225, 255);

        [MenuItem("GameObject/Highlight/Red",    false, 10)] static void Highlight_Go0(MenuCommand menuCommand) => Highlight(menuCommand, red);
        [MenuItem("GameObject/Highlight/Orange", false, 11)] static void Highlight_Go1(MenuCommand menuCommand) => Highlight(menuCommand, orange);
        [MenuItem("GameObject/Highlight/Yellow", false, 12)] static void Highlight_Go2(MenuCommand menuCommand) => Highlight(menuCommand, yellow);
        [MenuItem("GameObject/Highlight/Green",  false, 13)] static void Highlight_Go3(MenuCommand menuCommand) => Highlight(menuCommand, green);
        [MenuItem("GameObject/Highlight/Blue",   false, 14)] static void Highlight_Go4(MenuCommand menuCommand) => Highlight(menuCommand, blue);
        [MenuItem("GameObject/Highlight/Purple", false, 15)] static void Highlight_Go5(MenuCommand menuCommand) => Highlight(menuCommand, purple);
        [MenuItem("GameObject/Highlight/Clear",  false, 16)] static void Highlight_Go6(MenuCommand menuCommand) => ClearHighlight(menuCommand);

        [MenuItem("Assets/Highlight/Red",    false, 100)] static void Highlight_As0(MenuCommand menuCommand) => Highlight(menuCommand, red);
        [MenuItem("Assets/Highlight/Orange", false, 101)] static void Highlight_As1(MenuCommand menuCommand) => Highlight(menuCommand, orange);
        [MenuItem("Assets/Highlight/Yellow", false, 102)] static void Highlight_As2(MenuCommand menuCommand) => Highlight(menuCommand, yellow);
        [MenuItem("Assets/Highlight/Green",  false, 103)] static void Highlight_As3(MenuCommand menuCommand) => Highlight(menuCommand, green);
        [MenuItem("Assets/Highlight/Blue",   false, 104)] static void Highlight_As4(MenuCommand menuCommand) => Highlight(menuCommand, blue);
        [MenuItem("Assets/Highlight/Purple", false, 105)] static void Highlight_As5(MenuCommand menuCommand) => Highlight(menuCommand, purple);
        [MenuItem("Assets/Highlight/Clear",  false, 106)] static void Highlight_As6(MenuCommand menuCommand) => ClearHighlight(menuCommand);

        private static void Highlight(MenuCommand menuCommand, Color color)
        {
            GameObject go = menuCommand.context as GameObject;
            if (go)
            {
                CustomHighlighter.SetRule(go, color);
            }
            else if (Selection.assetGUIDs != null)
            {
                foreach (var guid in Selection.assetGUIDs)
                    CustomHighlighter.SetRule(guid, color);
            }
        }

        private static void ClearHighlight(MenuCommand menuCommand)
        {
            GameObject go = menuCommand.context as GameObject;
            if (go)
            {
                CustomHighlighter.ClearRule(go);
            }
            else if (Selection.assetGUIDs != null)
            {
                foreach (var guid in Selection.assetGUIDs)
                    CustomHighlighter.ClearRule(guid);
            }
        }
    }
}