// (c) Simone Guggiari 2020-2024

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

////////// PURPOSE: Various reflection utility functions //////////

namespace sxg
{
    public static class ReflectionUtility
    {
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(System.Type attributeType, BindingFlags flags = BindingFlags.Default)
        {
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.ManifestModule.Name == "Assembly-CSharp.dll") // the assembly containing monobehaviors
                {
                    if (flags == BindingFlags.Default)
                        flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    var methods = assembly.GetTypes()
                            .SelectMany(t => t.GetMethods(flags))
                            .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0);
                    return methods;
                }
            }
            return null;
        }

        public static MethodInfo GetStaticMethodWithAttributeNamed(System.Type attributeType, string functionName)
        {
            // TODO: cache the methods
            var methods = GetMethodsWithAttribute(attributeType, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return methods?.Find(m => m.Name == functionName);
        }

        public static void FindFieldContaining(string str)
        {
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.ManifestModule.Name == "Assembly-CSharp.dll") // the assembly containing monobehaviors
                {
                    BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    var fields = assembly.GetTypes()
                            .SelectMany(t => t.GetFields(flags))
                            .Where(f => f.Name.Contains(str));
                    foreach (var field in fields)
                    {
                        Debug.Log($"{field.Name} ({field.DeclaringType.Name})");
                    }
                    //return fields.Select(field => $"{field.Name} ({field.DeclaringType.Name})");
                }
            }
        }
    }
}