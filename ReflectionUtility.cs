// (c) Simone Guggiari 2020-2024

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

////////// PURPOSE: Various reflection utility functions //////////

namespace sxg
{
    public static class ReflectionUtility
    {
        public static MethodInfo[] GetMethodsWithAttribute(System.Type attributeType, bool allowInstance = false, bool allowPrivate = true)
        {
            MethodInfo[] methods = null;

            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.ManifestModule.Name == "Assembly-CSharp.dll") // the assembly containing monobehaviors
                {
                    BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
                    if (allowInstance) flags |= BindingFlags.Instance;
                    if (allowPrivate) flags |= BindingFlags.NonPublic;
                    methods = assembly.GetTypes()
                            .SelectMany(t => t.GetMethods(flags))
                            .Where(m => m.GetCustomAttributes(attributeType, false).Length > 0)
                            .ToArray();
                    return methods;
                }
            }
            return null;
        }

        public static MethodInfo GetStaticMethodWithAttributeNamed(System.Type attributeType, string functionName)
        {
            // TODO: cache the methods
            MethodInfo[] methods = GetMethodsWithAttribute(attributeType, false, true);
            if (methods != null)
            {
                foreach (MethodInfo method in methods)
                {
                    if (method.Name == functionName)
                    {
                        return method;
                    }
                }
            }
            return null;
        }
    }
}