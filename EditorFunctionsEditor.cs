// (c) Simone Guggiari 2024

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

////////// PURPOSE: Shows all functions tagged with attribute [EditorButton] into a collapsable list of containers for easy calling //////////

namespace sxg.mgp
{
#if SEDITOR
    public class EditorFunctionsEditor : MyEditorWindow
    {
        // -------------------- VARIABLES --------------------

        // public


        // private
        Vector2 scrollPos;

        // references
        List<Container> containers;


        // -------------------- BASE METHODS --------------------

        [MenuItem("Window/Tools/Editor Functions")]
        static void ShowWindow() => GetWindow<EditorFunctionsEditor>();


        // -------------------- CUSTOM METHODS --------------------

        // commands
        protected override void OnGUI()
        {
            base.OnGUI();

            if (containers.IsNullOrEmpty())
                containers = BuildContainersAndFunctions();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var container in containers)
            {
                container.visible = EditorGUILayout.Foldout(container.visible, container.name);
                if (container.visible)
                {
                    EditorGUI.indentLevel++;
                    foreach (var function in container.functions)
                    {
                        ButtonDrawerHelper.ShowAndRunMethod(container.obj, function.mi, (EditorButtonAttribute)function.attribute);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndScrollView();
        }


        // queries
        static List<Container> BuildContainersAndFunctions()
        {
            List<Container> ans = new();
            var methods = ReflectionUtility.GetMethodsWithAttribute(typeof(EditorButtonAttribute));
            foreach (MethodInfo mi in methods)
            {
                string typeName = Utility.AddSpacesBeforeCapitalLetters(mi.DeclaringType.Name);
                var attribute = mi.GetCustomAttribute<EditorButtonAttribute>();
                string memberName = attribute.GetLabel(mi);

                var container = ans.Find(t => t.name == typeName);
                if (container == null)
                {
                    object obj = FindObjectOfType(mi.DeclaringType);
                    container = new() { name = typeName, obj = obj };
                    ans.Add(container);
                }
                if (container.functions.Find(f => f.name == memberName) == null) // avoids dupes from derived types
                {
                    var function = new Function() { name = memberName, mi = mi, attribute = attribute };
                    container.functions.Add(function);
                }
            }
            ans.Sort((t1, t2) => t1.name.CompareTo(t2.name));
            return ans;
        }


        // other
        class Container
        {
            public string name;
            public bool visible;
            public object obj;
            public List<Function> functions = new();
        }

        class Function
        {
            public string name;
            public MethodInfo mi;
            public Attribute attribute;
        }

    }
#endif
}