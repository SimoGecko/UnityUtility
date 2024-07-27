// (c) Simone Guggiari 2024

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

////////// PURPOSE: Shows all functions tagged with attribute [EditorButton] into a collapsable list of containers for easy calling //////////

namespace sxg
{
#if SEDITOR
    public class EditorFunctionsEditor : MyEditorWindow
    {
        // -------------------- VARIABLES --------------------

        // PUBLIC
        string searchQuery;


        // PRIVATE
        Vector2 scrollPos;

        // REFERENCES
        List<Container> containers;


        // -------------------- BASE METHODS --------------------

        [MenuItem("Window/Tools/Editor Functions")]
        static void ShowWindow() => GetWindow<EditorFunctionsEditor>();


        // -------------------- CUSTOM METHODS --------------------

        // COMMANDS
        protected override void OnGUI()
        {
            base.OnGUI();

            if (containers.IsNullOrEmpty())
                containers = BuildContainersAndFunctions();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            searchQuery = EditorGUILayout.TextField("Search:", searchQuery);
            if (string.IsNullOrEmpty(searchQuery))
            {
                foreach (var container in containers)
                {
                    container.visible = EditorGUILayout.Foldout(container.visible, container.name);
                    if (container.visible)
                    {
                        EditorGUI.indentLevel++;
                        foreach (var function in container.functions)
                            ButtonDrawerHelper.ShowAndRunMethod(container.obj, function.mi, (EditorButtonAttribute)function.attribute, true);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                foreach (var container in containers)
                {
                    bool containerMatch = container.name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);
                    bool functionsMatch = container.functions.Any(f => f.name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
                    if (containerMatch || functionsMatch)
                    {
                        EditorGUILayout.Foldout(true, container.name);
                        EditorGUI.indentLevel++;
                        if (containerMatch)
                        {
                            foreach (var function in container.functions)
                                ButtonDrawerHelper.ShowAndRunMethod(container.obj, function.mi, (EditorButtonAttribute)function.attribute, true);
                        }
                        else
                        {
                            // only draw those that matched
                            foreach (var function in container.functions.Where(f => f.name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                                ButtonDrawerHelper.ShowAndRunMethod(container.obj, function.mi, (EditorButtonAttribute)function.attribute, true);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }


        // QUERIES
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
#if UNITY_6000_0_OR_NEWER
                    object obj = FindAnyObjectByType(mi.DeclaringType);
#else
                    object obj = FindObjectOfType(mi.DeclaringType);
#endif
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


        // OTHER
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