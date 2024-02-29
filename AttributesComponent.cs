// (c) Simone Guggiari 2024

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

////////// PURPOSE:  //////////

namespace sxg.rym2
{
    public class AttributesComponent : MonoBehaviour
    {
        // -------------------- VARIABLES --------------------

        // public
        // bool, int, float, string, Vector2/3/4, Quaternion, CFrame, Color
        // (RaycastHit), Bounds, Rect, Ray
        // GameObject, Transform, Component, (Mesh)

        // ROBLOX supports: string bool number UDim/2 BrickColor Color3 Vector2/3 CFrame NumberSequence ColorSequence NumberRange Font

        // TODO: serialize
        public Dictionary<string, object> attributes = new(); // TODO: make private
        
        
        // private
        
        
        // references
        
        
        // -------------------- BASE METHODS --------------------
        
        
        
        // -------------------- CUSTOM METHODS --------------------
        
        
        // commands
        public void Set(string key, object value)
        {
            Debug.Assert(key.Length <= 100 && key.IsRegexMatch("[A-Za-z_][A-Za-z0-9_]*", out _)); // only contains [A-Za-z0-9_] not starting with digit
            attributes[key] = value;
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
            Debug.Assert(myvec == new Vector3(1,2,3));

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


        // queries
        GameObject go => gameObject;



        // other

    }

    [CustomEditor(typeof(AttributesComponent))]
    public class AttributesComponentEditor : Editor
    {
        private AttributesComponent component;

        string key;
        public enum Type { Int, Float, Bool }
        Type type;

        private void OnEnable()
        {
            component = (AttributesComponent)target;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            var attributes = component.attributes;
            foreach (string key in attributes.Keys.ToList())
            {
                //EditorGUILayout.BeginHorizontal();
                var value = attributes[key];
                if (value == null)
                    continue;
                    //attributes[key] = EditorGUILayout.ObjectField(key, (UnityEngine.Object)value, typeof(UnityEngine.Object), true);
                if (value.GetType() == typeof(bool))
                    attributes[key] = EditorGUILayout.Toggle(key, (bool)value);
                else if (value.GetType() == typeof(int))
                    attributes[key] = EditorGUILayout.IntField(key, (int)value);
                else if (value.GetType() == typeof(float))
                    attributes[key] = EditorGUILayout.FloatField(key, (float)value);
                else if (value.GetType() == typeof(string))
                    attributes[key] = EditorGUILayout.TextField(key, (string)value);
                else if (value.GetType() == typeof(Vector2))
                    attributes[key] = EditorGUILayout.Vector2Field(key, (Vector2)value);
                else if (value.GetType() == typeof(Vector3))
                    attributes[key] = EditorGUILayout.Vector3Field(key, (Vector3)value);
                else if (value.GetType() == typeof(Vector4))
                    attributes[key] = EditorGUILayout.Vector4Field(key, (Vector4)value);
                else if (value.GetType() == typeof(Quaternion))
                    attributes[key] = Quaternion.Euler(EditorGUILayout.Vector3Field(key, ((Quaternion)value).eulerAngles)); // Euler view -> has some problems
                //else if (value.GetType() == typeof(CFrame))
                //    attributes[key] = EditorGUILayout.TextField(key, (CFrame)value);
                else if (value.GetType() == typeof(Color))
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
            }
            // TODO: remove this
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test"))
                component.EDITOR_Test();
            if (GUILayout.Button("Populate"))
                component.EDITOR_Populate();
            if (GUILayout.Button("Clear"))
                component.EDITOR_Clear();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 30;
            key = EditorGUILayout.TextField("key", key);
            type = (Type)EditorGUILayout.EnumPopup("type", type);

            if (GUILayout.Button("Add"))
                component.Set(key, 3); // TODO
            if (GUILayout.Button("Remove"))
                component.Remove(key);
            EditorGUILayout.EndHorizontal();

        }
    }

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