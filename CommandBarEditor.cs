// (c) Simone Guggiari 2024

using UnityEngine;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Text;

////////// PURPOSE: Editor that executes some C# code provided as string //////////

/// NOTES: this requires the file CodeCompiler.cs containing CodeCompiler, CustomDynamicDriver, CustomReportPrinter
/// obtained from https://github.com/SebLague/Runtime-CSharp-Test, as well as mcs.dll under Assets/Plugins
/// Have also a look at https://github.com/JakubNei/mcs-ICodeCompiler

namespace sxg.aw
{
    public class CommandBarEditor : MyEditorWindow
    {
        // -------------------- VARIABLES --------------------

        // PUBLIC
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
                    ExecuteCode(userCode);
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

        private static void ExecuteCode(string userCode)
        {
            userCode = SyntacticSugar(userCode);
            Debug.Log($"> {userCode}");
            string code = @$"
                using UnityEngine; using UnityEditor; using UnityEngine.Tilemaps;
                public class MyClass {{
                    public static void MyMethod(){{
                        {userCode};
                    }}
                }}
            ";
            var assembly = Compile(code);
            var method = assembly?.GetType("MyClass")?.GetMethod("MyMethod");
            if (method == null)
                return;
            var del = (Action)Delegate.CreateDelegate(typeof(Action), method);
            del.Invoke();
        }

        private static string SyntacticSugar(string userCode)
        {
            // some non-C# syntax used to make my life easier. desired syntax:
            // print("hi") => Debug.Log("hi")
            // game.GameObject1.child.transform.position => GameObject.Find("GameObject1/child").transform.position
            // game.GameObject1:MyComponent.value => GameObject.Find("GameObject1").GetComponent<MyComponent>().value
            // (TileBase)assets.Tiles.map_tiles_0 => AssetDatabase.LoadAssetAtPath<TileBase>("Assets/Tiles/map_tiles_0.asset")
            userCode = userCode
                .Replace("print", "Debug.Log")
                .Replace("warn", "Debug.LogWarning");
            userCode = new Regex(@"game\.([\w/]+):(\w+)").Replace(userCode, @"GameObject.Find(""$1"").GetComponent<$2>()");
            userCode = new Regex(@"game\.([\w/]+)").Replace(userCode, @"GameObject.Find(""$1"")");
            return userCode;
        }

        private static Assembly Compile(string source)
        {
            var options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;

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
                    //Debug.LogError(err);
                }
                Debug.LogError(sb.ToString());
            }

            return result.CompiledAssembly;
        }

        // QUERIES


        // OTHER

    }
}
