// (c) Simone Guggiari 2020-2022

using System.Collections.Generic;

using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
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

    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class EditorButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mono = target as MonoBehaviour;
            //var methods = typeof(MonoBehaviour) // base
            var methods = mono.GetType() // derived
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

        string Prettify(string methodName)
        {
            // Check that is starts with EDITOR_
            if (methodName.StartsWith("EDITOR_"))
            {
                methodName = methodName[7..];
            }
            else
            {
                Debug.LogWarning($"Editor method {methodName} should start with EDITOR_");
            }
            return Utility.AddSpacesBeforeCapitalLetters(methodName);
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

#endif
}