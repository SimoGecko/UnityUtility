// (c) Simone Guggiari 2024

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

////////// PURPOSE: Editor that executes some C# code provided as string //////////

/// NOTES: this requires the file CodeCompiler.cs containing CodeCompiler, CustomDynamicDriver, CustomReportPrinter
/// obtained from https://github.com/SebLague/Runtime-CSharp-Test, as well as mcs.dll under Assets/Plugins
/// Have also a look at https://github.com/JakubNei/mcs-ICodeCompiler

#if SEDITOR
namespace sxg
{
    public class CommandBarEditor : MyEditorWindow
    {
        // -------------------- VARIABLES --------------------

        // PUBLIC
        public UnityEngine.Object obj;
        string userCode;


        // PRIVATE


        // REFERENCES


        // -------------------- BASE METHODS --------------------

        [MenuItem("Window/Tools/Command Bar")]
        static void ShowWindow()
        {
            var window = GetWindow<CommandBarEditor>();
            // d_Exposure, d_Favorite, d_UnityEditor.ConsoleWindow, d_Import, d_PlayButton, align_horizontally_left_active, d_TextAsset Icon,
            Texture icon = EditorGUIUtility.IconContent("d_Exposure").image;
            window.titleContent = new GUIContent("Command Bar", icon);
        }

        // -------------------- CUSTOM METHODS --------------------

        protected override void OnGUI()
        {
            base.OnGUI();
            EditorGUILayout.BeginHorizontal();
            userCode = EditorGUILayout.TextArea(userCode, new GUIStyle(EditorStyles.textField));
            Event e = Event.current;
            try
            {
                if (GUILayout.Button("Run", GUILayout.Width(60)) ||
                    (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return))
                    ExecuteCode(userCode, obj);
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        /*
        public static object Runcode(string userCode) // static is extra
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(new CompilerParameters(), userCode);
            System.Type classType = results.CompiledAssembly.GetType("MyClass");
            System.Reflection.MethodInfo method = classType.GetMethod("MyMethod");
            return method.Invoke(null, null);
        }
        */

        private static void ExecuteCode(string userCode, UnityEngine.Object obj)
        {
            userCode = SyntacticSugar(userCode);
            Debug.Log($"> {userCode}");
            string code = @$"
                using System; using System.Collections; using System.Collections.Generic; using System.Linq;
                using UnityEngine; using UnityEditor; using UnityEngine.UI; using UnityEngine.Tilemaps;
                using sxg; using sxg.aw;

                public class MyClass {{
                    public static void MyMethod(UnityEngine.Object obj){{
                        {userCode};
                    }}
                }}
            ";
#if CCOMPILER
            var assembly = Compile(code);
            var method = assembly?.GetType("MyClass")?.GetMethod("MyMethod");
            if (method == null)
                return;
            var del = (Action<UnityEngine.Object>)Delegate.CreateDelegate(typeof(Action<UnityEngine.Object>), method);
            del.Invoke(obj);
#endif
        }

        private static string SyntacticSugar(string userCode)
        {
            // some non-C# syntax used to make my life easier. desired syntax:
            // print("hi") => Debug.Log("hi")
            // game.GameObject1.child.transform.position => GameObject.Find("GameObject1/child").transform.position
            // game.GameObject1:MyComponent.value => GameObject.Find("GameObject1").GetComponent<MyComponent>().value
            // (TileBase)assets.Tiles.map_tiles_0 => AssetDatabase.LoadAssetAtPath<TileBase>("Assets/Tiles/map_tiles_0.asset")

            List<(string, string)> replacements = new()
            {
                (@"\bprint\b\(", "Debug.Log("),
                (@"\bwarn\b\(", "Debug.LogWarning("),
                (@"game\.([\w/]+):(\w+)", @"GameObject.Find(""$1"").GetComponent<$2>()"),
                (@"game\.([\w/]+)", @"GameObject.Find(""$1"")"),
                (@"script\.([\w/]+)", @"GameObject.FindObjectOfType<$1>()"),
                (@"\((\w+)\)assets\.([\w./]+)", @"($1)AssetDatabase.LoadAssetAtPath<$1>(""Assets/$2"")")
            };

            foreach (var replace in replacements)
                userCode = new Regex(replace.Item1).Replace(userCode, replace.Item2);

            return userCode;
        }

#if CCOMPILER
        private static Assembly Compile(string source)
        {
            var options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    options.ReferencedAssemblies.Add(assembly.Location);
                }
            }

            var compiler = new CSharpCompiler.CodeCompiler();
            var result = compiler.CompileAssemblyFromSource(options, source);

            if (result.Errors.Count > 0)
            {
                StringBuilder sb = new();
                foreach (var err in result.Errors)
                {
                    sb.AppendLine(err.ToString());
                }
                Debug.LogError(sb.ToString());
            }

            return result.CompiledAssembly;
        }
#endif
        // QUERIES


        // OTHER

    }
}
#endif