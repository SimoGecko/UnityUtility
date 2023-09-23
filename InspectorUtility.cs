// (c) Simone Guggiari 2020-2023

using System.Collections.Generic;

using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using Unity.Netcode;
//using Mvvm.Extensions;


#if (SEDITOR && UNITY_EDITOR)
using UnityEditor;
#endif

////////// DESCRIPTION //////////

//[ContextMenu("")]
//[MenuItem("Tools/")]


namespace sxg
{
#if (SEDITOR && UNITY_EDITOR)

    // PURPOSE: Shows a property as readonly
    // USAGE:
    // [ReadOnly] public string testString;
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }


    // PURPOSE: Used together with buttons to place them side by side
    // USAGE:
    // [LayoutBeginHorizontal] void MyFunc();
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class LayoutBeginHorizontal : PropertyAttribute
    {
        public LayoutBeginHorizontal()
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class LayoutEndHorizontal : PropertyAttribute
    {
        public LayoutEndHorizontal()
        {
        }
    }

    // PURPOSE: Tags a function as something that should only exist in the editor
    // USAGE:
    // [EditorFunction] void MyFunc();
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class EditorFunctionAttribute : PropertyAttribute
    {
    }

    // PURPOSE: Button to add to a method
    // USAGE:
    // [EditorButton("OptionalName")] void MyFunc();
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class EditorButtonAttribute : PropertyAttribute
    {
        public EditorButtonAttribute(string label = "", string tooltip = "")
        {
            this.Label = label;
            this.Tooltip = tooltip;
        }
        public string Label { get; private set; }
        public string Tooltip { get; private set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TabHierarchyAttribute : PropertyAttribute
    {
    }

    [CustomEditor(typeof(NetworkBehaviour), true), CanEditMultipleObjects]
    public class MyNetworkBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var mono = target as NetworkBehaviour;
            base.OnInspectorGUI();
            ButtonDrawerHelper.DrawButtonsForType(mono.GetType(), mono);
        }
    }

    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class MyMonoBehaviourEditor : Editor
    {
        static int tabIndex = 0;

        public override void OnInspectorGUI()
        {
            var mono = target as MonoBehaviour;
            if (Attribute.GetCustomAttribute(mono.GetType(), typeof(TabHierarchyAttribute), true) == null)
            {
                base.OnInspectorGUI();
                ButtonDrawerHelper.DrawButtonsForType(mono.GetType(), mono);
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(mono), mono.GetType(), false);
                GUI.enabled = true;

                List<Type> types = GetTypesOf(mono.GetType());
                tabIndex = Mathf.Clamp(tabIndex, 0, types.Count);
                tabIndex = GUILayout.Toolbar(tabIndex, types.Select(t => t.Name).ToArray());
                //EditorGUILayout.BeginVertical();
                //tabsSelected = GUILayout.SelectionGrid(tabsSelected, tabs, 2); // shows a grid
                //EditorGUILayout.EndVertical();

                Type type = types[tabIndex];

                var members = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach(var member in members)
                {
                    var property = (serializedObject).FindProperty(member.Name);
                    if (property != null)
                    {
                        EditorGUILayout.PropertyField(property);
                    }
                }
                serializedObject.ApplyModifiedProperties();
                ButtonDrawerHelper.DrawButtonsForType(type, mono);
            }
        }

        List<Type> GetTypesOf(Type type)
        {
            List<Type> ans = new();
            while (type != typeof(MonoBehaviour))
            {
                ans.Add(type);
                type = type.BaseType;
            }
            ans.Reverse();
            return ans;
        }


    }

    static class ButtonDrawerHelper
    {
        public static void DrawButtonsForType(Type type, MonoBehaviour mono)
        {
            //var methods = typeof(MonoBehaviour) // base
            var methods = type // derived
                .GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(o => Attribute.IsDefined(o, typeof(EditorButtonAttribute)));

            foreach (var memberInfo in methods)
            {
                EditorButtonAttribute attribute = memberInfo.GetCustomAttribute<EditorButtonAttribute>();
                string label = string.IsNullOrEmpty(attribute.Label) ? Prettify(memberInfo.Name) : attribute.Label;

                if (memberInfo.GetCustomAttribute<LayoutBeginHorizontal>() != null)
                {
                    GUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(new GUIContent(label, attribute.Tooltip)))
                {
                    var method = memberInfo as MethodInfo;
                    method.Invoke(mono, null);
                }

                if (memberInfo.GetCustomAttribute<LayoutEndHorizontal>() != null)
                {
                    GUILayout.EndHorizontal();
                }
            }
        }

        public static string Prettify(string methodName)
        {
            // Check that is starts with EDITOR_
            if (methodName.StartsWith("EDITOR_"))
            {
                methodName = methodName[7..];
            }
            else
            {
                //Debug.LogWarning($"Editor method {methodName} should start with EDITOR_");
            }
            return Utility.AddSpacesBeforeCapitalLetters(methodName);
        }
    }

    static class ContextMenuUtility
    {
        static string GetString(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            return string.Format("{0:F6},{1:F6},{2:F6},{3:F6},{4:F6},{5:F6},{6:F6},{7:F6},{8:F6}",
                pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, scale.x, scale.y, scale.z);
            //return $"{pos}, {rot}, {scale}";
        }
        static bool GetValues(string value, out Vector3 pos, out Vector3 rot, out Vector3 scale)
        {
            pos = Vector3.zero;
            rot = Vector3.zero;
            scale = Vector3.one;
            string[] tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 9)
                return false;
            float[] f = new float[9];
            for (int i = 0; i < 9; i++)
            {
                if (!float.TryParse(tokens[i], out float f1))
                {
                    return false;
                }
                f[i] = f1;
            }
            pos = new Vector3(f[0], f[1], f[2]);
            rot = new Vector3(f[3], f[4], f[5]);
            scale = new Vector3(f[6], f[7], f[8]);
            return true;
        }
        //[MenuItem("CONTEXT/Transform/Copy Local")]
        public static void CopyLocal(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            GUIUtility.systemCopyBuffer = GetString(t.localPosition, t.localEulerAngles, t.localScale);
        }
        //[MenuItem("CONTEXT/Transform/Copy Global")]
        public static void CopyGlobal(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            GUIUtility.systemCopyBuffer = GetString(t.position, t.eulerAngles, t.lossyScale);
        }
        //[MenuItem("CONTEXT/Transform/Paste Local")]
        public static void PasteLocal(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            if (GetValues(GUIUtility.systemCopyBuffer, out Vector3 pos, out Vector3 rot, out Vector3 scale))
            {
                t.localPosition = pos;
                t.localEulerAngles = rot;
                t.localScale = scale;
            }
        }
        //[MenuItem("CONTEXT/Transform/Paste Global")]
        public static void PasteGlobal(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            if (GetValues(GUIUtility.systemCopyBuffer, out Vector3 pos, out Vector3 rot, out Vector3 scale))
            {
                t.position = pos;
                t.eulerAngles = rot;
                if (t.parent == null)
                {
                    t.localScale = scale;
                }
                else
                {
                    t.localScale = scale.Div(t.parent.lossyScale);
                }
            }
        }
    }


#else
    public class ReadOnlyAttribute : Attribute
    {
    }
    public class LayoutBeginHorizontal : Attribute
    {
    }
    public class LayoutEndHorizontal : Attribute
    {
    }
    public class EditorFunctionAttribute : Attribute
    {
    }
    public class EditorButtonAttribute : Attribute
    {
    }
    public class TabHierarchyAttribute : Attribute
    {
    }

#endif
}