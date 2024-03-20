// (c) Simone Guggiari 2024

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

////////// PURPOSE: This class is a generic container of attributes: key-value pairs with key a string and value a basic datatype //////////

// Supported types:
//     bool, int, float, string, Vector2/3/4, Quaternion, (CFrame), Color
//     (RaycastHit), Bounds, Rect, Ray
//     GameObject, (Transform), Component, (Mesh)
// 
// ROBLOX supports: string bool number UDim/2 BrickColor Color3 Vector2/3 CFrame NumberSequence ColorSequence NumberRange Font

// CFrame = (V3, quat), Bounds=(V3, V3), Rect=(float 4x), Ray=(V3, V3)

namespace sxg
{
    public enum EType { /*Invalid,*/ Bool, Int, Float, String, Vector2, Vector3, Vector4, Quaternion, /*CFrame,*/ Color, GameObject, /*Transform,*/ Component }

    public class AttributesComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        // -------------------- VARIABLES --------------------

        // public


        // private
        private Dictionary<string, object> attributes = new();
        [SerializeField][HideInInspector] private List<string> stringdata; // only used for serialization

        private static readonly int keyMaxLength = 100;
        private static readonly string keyRegexPattern = "[A-Za-z_][A-Za-z0-9_]*"; // only contains alphanumeric and _, not starting with digit


        // references


        // -------------------- BASE METHODS --------------------



        // -------------------- CUSTOM METHODS --------------------


        // commands
        public void Set(string key, object value)
        {
            if (key.Length <= keyMaxLength && key.IsRegexMatch(keyRegexPattern))
                attributes[key] = value;
            else
                Debug.LogWarning($"Invalid key: {key}");
        }
        public object Get(string key)
        {
            if (attributes.TryGetValue(key, out object value))
                return value;
            return null;
        }
        public void Remove(string key)
        {
            attributes.Remove(key);
        }
        public void Clear()
        {
            attributes.Clear();
        }

        public void OnBeforeSerialize()
        {
            stringdata = attributes.Select(pair => $"{pair.Key}: {Serialize(pair.Value)}").ToList();
            //attributes = new();
        }

        public void OnAfterDeserialize()
        {
            attributes = new();
            if (stringdata != null)
            {
                foreach (var data in stringdata)
                {
                    string[] tokens = data.Split(':');
                    string key = tokens[0];
                    object value = Deserialize(tokens[1].TrimStart());
                    attributes.Add(key, value);
                }
            }
            stringdata = null;
        }

        static string Serialize(object obj)
        {
            if (obj == null)
                return "<null>";

            if (obj is bool)
                return $"{obj} (bool)";
            else if (obj is int)
                return $"{obj} (int)";
            else if (obj is float)
                return $"{obj} (float)";
            else if (obj is string)
                return $"\"{obj}\" (string)";
            else if (obj is Vector2)
                return $"{obj} (Vector2)";
            else if (obj is Vector3)
                return $"{obj} (Vector3)";
            else if (obj is Vector4)
                return $"{obj} (Vector4)";
            else if (obj is Quaternion)
                return $"{obj} (Quaternion)";
            else if (obj is Color)
                return $"{obj.ToString().TrimStart("RGBA")} (Color)";
            else if (obj is GameObject)
                return $"{obj} (GameObject)";
            else if (obj is Component)
            {
                string derivedType = obj.GetType().ToString().SplitBy('.').Last();
                return $"{obj} ({derivedType})";
            }
            Debug.Assert(false, "Unreachable code");
            return null;
        }

        static float[] ParseFloatArray(string str, int expected)
        {
            str = str.TrimStart('(').TrimEnd(')');
            string[] components = str.Split(',');

            if (components.Length != expected)
            {
                Debug.LogError($"Expected {expected} components, but found {components.Length}");
                return null;
            }

            float[] floats = new float[expected];

            for (int i = 0; i < expected; i++)
            {
                if (!float.TryParse(components[i], out floats[i]))
                {
                    Debug.LogError($"Failed to parse component {i}: {components[i]}");
                    return null;
                }
            }

            return floats;
        }

        static object Deserialize(string str)
        {
            if (string.IsNullOrEmpty(str) || str == "<null>")
                return null;

            bool ok = Utility.IsRegexMatch(str, @"(.*) \((.*)\)", out string[] tokens);
            Debug.Assert(ok, "Invalid deserialization pattern");
            string value = tokens[1];
            string type = tokens[2];

            if (type == "bool")
                return bool.Parse(value);
            else if (type == "int")
                return int.Parse(value);
            else if (type == "float")
                return float.Parse(value);
            else if (type == "string")
                return value.TrimStart('"').TrimEnd('"');
            else if (type == "Vector2")
            {
                float[] f = ParseFloatArray(value, 2);
                return new Vector2(f[0], f[1]);
            }
            else if (type == "Vector3")
            {
                float[] f = ParseFloatArray(value, 3);
                return new Vector3(f[0], f[1], f[2]);
            }
            else if (type == "Vector4")
            {
                float[] f = ParseFloatArray(value, 4);
                return new Vector4(f[0], f[1], f[2], f[3]);
            }
            else if (type == "Quaternion")
            {
                float[] f = ParseFloatArray(value, 4);
                return new Quaternion(f[0], f[1], f[2], f[3]);
            }
            else if (type == "Color")
            {
                float[] f = ParseFloatArray(value, 4);
                return new Color(f[0], f[1], f[2], f[3]);
            }
            else if (type == "GameObject")
                return null; // TODO
            else if (type == "Component") // TODO: the derived type could be something else
                return null; // TODO

            //Debug.Assert(false, $"Unreachable code: Deserialize({str})");
            return null;
        }

        static Type ETypeToType(EType type)
        {
            switch (type)
            {
            case EType.Bool: return typeof(bool);
            case EType.Int: return typeof(int);
            case EType.Float: return typeof(float);
            case EType.String: return typeof(string);
            case EType.Vector2: return typeof(Vector2);
            case EType.Vector3: return typeof(Vector3);
            case EType.Vector4: return typeof(Vector4);
            case EType.Quaternion: return typeof(Quaternion);
            case EType.Color: return typeof(Color);
            case EType.GameObject: return typeof(GameObject);
            case EType.Component: return typeof(Component);
            }
            Debug.Assert(false, "Unreachable code");
            return null;
        }
        public static object ETypeToDefault(EType type)
        {
            switch (type)
            {
            case EType.Bool: return false;
            case EType.Int: return 0;
            case EType.Float: return 0f;
            case EType.String: return "";
            case EType.Vector2: return Vector2.zero;
            case EType.Vector3: return Vector3.zero;
            case EType.Vector4: return Vector4.zero;
            case EType.Quaternion: return Quaternion.identity;
            case EType.Color: return Color.white;
            case EType.GameObject: return null;
            case EType.Component: return null;
            }
            Debug.Assert(false, "Unreachable code");
            return null;
        }

#if SEDITOR
        [EditorButton]
        public void EDITOR_Test()
        {
            go.SetAttribute("myint", 3);
            int myint = go.GetAttribute<int>("myint");
            Debug.Assert(myint == 3);

            go.RemoveAttribute("myint");
            myint = go.GetAttribute<int>("myint");
            Debug.Assert(myint == 0);

            go.SetAttribute("myvec", new Vector3(1, 2, 3));
            Vector3 myvec = go.GetAttribute<Vector3>("myvec");
            Debug.Assert(myvec == new Vector3(1, 2, 3));

            Vector3 myvec_no = go.GetAttribute<Vector3>("myvec_notfound");
            Debug.Assert(myvec_no == Vector3.zero);

            go.SetAttribute("mystr", $"test string {4}");
            string mystr = go.GetAttribute<string>("mystr");
            Debug.Assert(mystr == "test string 4");

            go.SetAttribute("mystr", (string)null);
            mystr = go.GetAttribute<string>("mystr");
            Debug.Assert(mystr == null);

            go.SetAttribute("mytr", go.transform);
            Transform mytr = go.GetAttribute<Transform>("mytr");
            Debug.Assert(mytr == go.transform);

            Debug.Log("all tests passed");
        }
        public void EDITOR_Clear()
        {
            go.ClearAttributes();
        }
        public void EDITOR_Populate()
        {
            go.SetAttribute("mybool", true);
            go.SetAttribute("myint", 3);
            go.SetAttribute("myfloat", 3.14f);
            go.SetAttribute("mystr", "hi there");
            go.SetAttribute("myvec2", new Vector2(1f, 2f));
            go.SetAttribute("myvec3", new Vector3(1f, 2f, 3f));
            go.SetAttribute("myvec4", new Vector4(1f, 2f, 3f, 4f));
            go.SetAttribute("myquat", Quaternion.Euler(90f, 0f, 0f));
            //go.SetAttribute("mycframe", null);
            go.SetAttribute("mycol", Color.blue);
            go.SetAttribute("mygo", go);
            go.SetAttribute("mytr", go.transform);
            go.SetAttribute("mycomp", go.GetComponent<AttributesComponent>());
            go.SetAttribute("mymat", (Material)null);
        }
#endif

        // queries
        GameObject go => gameObject;
        public Dictionary<string, object> Attributes => attributes;

        // other

    }

#if SEDITOR
    [CustomEditor(typeof(AttributesComponent))]
    public class AttributesComponentEditor : Editor
    {
        private AttributesComponent component;

        string key;
        EType type;

        private void OnEnable()
        {
            component = (AttributesComponent)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var attributes = component.Attributes;
            //bool dirty = false;
            foreach (string key in attributes.Keys.ToList())
            {
                //EditorGUILayout.BeginHorizontal();
                var value = attributes[key];
                if (value == null)
                    continue;

                //attributes[key] = EditorGUILayout.ObjectField(key, (UnityEngine.Object)value, typeof(UnityEngine.Object), true);
                if (value is bool)
                    attributes[key] = EditorGUILayout.Toggle(key, (bool)value);
                else if (value is int)
                    attributes[key] = EditorGUILayout.IntField(key, (int)value);
                else if (value is float)
                    attributes[key] = EditorGUILayout.FloatField(key, (float)value);
                else if (value is string)
                    attributes[key] = EditorGUILayout.TextField(key, (string)value);
                else if (value is Vector2)
                    attributes[key] = EditorGUILayout.Vector2Field(key, (Vector2)value);
                else if (value is Vector3)
                    attributes[key] = EditorGUILayout.Vector3Field(key, (Vector3)value);
                else if (value is Vector4)
                    attributes[key] = EditorGUILayout.Vector4Field(key, (Vector4)value);
                else if (value is Quaternion)
                    attributes[key] = Quaternion.Euler(EditorGUILayout.Vector3Field(key, ((Quaternion)value).eulerAngles)); // Euler view -> has some problems
                //else if (value.GetType() == typeof(CFrame))
                //    attributes[key] = EditorGUILayout.TextField(key, (CFrame)value);
                else if (value is Color)
                    attributes[key] = EditorGUILayout.ColorField(key, (Color)value);
                else if (value is GameObject)
                    attributes[key] = EditorGUILayout.ObjectField(key, (GameObject)value, value.GetType(), true);
                else if (value is Component)
                    attributes[key] = EditorGUILayout.ObjectField(key, (Component)value, value.GetType(), true);
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(key, value.ToString());
                    GUI.enabled = true;
                }
                //if (GUILayout.Button("-"))
                //    Debug.Log("-");
                //EditorGUILayout.EndHorizontal();

                EditorUtility.SetDirty(component);
            }
#if false
            // TODO: remove this
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test"))
                component.EDITOR_Test();
            if (GUILayout.Button("Populate"))
                component.EDITOR_Populate();
            if (GUILayout.Button("Clear"))
                component.EDITOR_Clear();
            EditorGUILayout.EndHorizontal();

#endif
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 30;
            key = EditorGUILayout.TextField("key", key);
            type = (EType)EditorGUILayout.EnumPopup("type", type);

            if (GUILayout.Button("Add"))
                component.Set(key, AttributesComponent.ETypeToDefault(type));
            if (GUILayout.Button("Remove"))
                component.Remove(key);
            EditorGUILayout.EndHorizontal();

        }
    }
#endif

    public static class AttributesExtensions
    {
        public static T GetAttribute<T>(this GameObject go, string name)
        {
            AttributesComponent att = go.GetComponent<AttributesComponent>();
            if (att == null)
                return default;
            object value = att.Get(name);
            if (value == null)
                return default;
            return (T)value;
        }
        public static void SetAttribute<T>(this GameObject go, string name, T value)
        {
            AttributesComponent att = go.GetComponent<AttributesComponent>() ?? go.AddComponent<AttributesComponent>();
            //if (value == null)
            //    att.Remove(name);
            //else
            att.Set(name, value);
        }
        public static void RemoveAttribute(this GameObject go, string name)
        {
            AttributesComponent att = go.GetComponent<AttributesComponent>() ?? go.AddComponent<AttributesComponent>();
            att.Remove(name);
        }
        public static void ClearAttributes(this GameObject go)
        {
            AttributesComponent att = go.GetComponent<AttributesComponent>() ?? go.AddComponent<AttributesComponent>();
            att.Clear();
        }
    }

}