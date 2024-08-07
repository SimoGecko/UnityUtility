﻿// (c) Simone Guggiari 2020-2024

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if SNETCODE
using Unity.Netcode;
#endif

////////// DESCRIPTION //////////

//[ContextMenu("")]
//[MenuItem("Tools/")]


namespace sxg
{
#if SEDITOR

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

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class RangeExAttribute : PropertyAttribute
    {
        public readonly float min, max, step;

        public RangeExAttribute(float min, float max, float step)
        {
            this.min = min;
            this.max = max;
            this.step = step;
        }
    }

    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float min, max;

        public MinMaxSliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    // from https://forum.unity.com/threads/range-attribute.451848/
    [CustomPropertyDrawer(typeof(RangeExAttribute))]
    public sealed class RangeExDrawer : PropertyDrawer
    {
        private float value;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rangeAttribute = (RangeExAttribute)base.attribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                value = EditorGUI.Slider(position, label, property.floatValue, rangeAttribute.min, rangeAttribute.max);
                value = Mathf.Round(value / rangeAttribute.step) * rangeAttribute.step;
                property.floatValue = value;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Range with float.");
            }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class Foldout : PropertyAttribute
    {
        public Foldout(string label = "", int size = 1)
        {
            this.Label = label;
            this.Size = size;
        }
        public string Label { get; private set; }
        public int Size { get; private set; }
    }


    // PURPOSE: Used together with buttons to place them side by side
    // USAGE:
    // [LayoutBeginHorizontal] void MyFunc();
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class LayoutBeginHorizontal : PropertyAttribute
    {
    }
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class LayoutEndHorizontal : PropertyAttribute
    {
    }

    // PURPOSE: Adds a button in the inspector that allows calling this function.
    // If the function has parameters those are also shown in the inspector.
    // Usually these functions only exist while in Editor mode.
    // USAGE:
    // [EditorButton("OptionalName")] void MyFunc();
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class EditorButtonAttribute : PropertyAttribute
    {
        public EditorButtonAttribute(string label = "", string tooltip = "", int size = 0)
        {
            this.Label = label;
            this.Tooltip = tooltip;
            this.Size = size;
        }
        public EditorButtonAttribute(int size)
        {
            this.Label = "";
            this.Tooltip = "";
            this.Size = size;
        }
        public string Label { get; private set; }
        public string Tooltip { get; private set; }
        public int Size { get; private set; }

        public string GetLabel(MemberInfo memberInfo)
        {
            return string.IsNullOrEmpty(Label) ? Prettify(memberInfo.Name) : Label;
        }
        private static string Prettify(string methodName)
        {
            if (methodName.StartsWith("EDITOR_"))
            {
                methodName = methodName.TrimStart("EDITOR_");
            }
            else
            {
                Debug.LogWarning($"Editor method {methodName} should start with EDITOR_");
            }
            return Utility.AddSpacesBeforeCapitalLetters(methodName);
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TabHierarchyAttribute : PropertyAttribute
    {
    }

#if SNETCODE
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
#endif

    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class MyMonoBehaviourEditor : Editor
    {
        static int tabIndex = 0;

        public override void OnInspectorGUI()
        {
            var mono = target as MonoBehaviour;
            if (Attribute.GetCustomAttribute(mono.GetType(), typeof(TabHierarchyAttribute), true) == null)
            {
                // TODO: add tabs
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
                foreach (var member in members)
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

    public class MyEditorWindow : EditorWindow
    //public class MyEditorWindow<T> : EditorWindow where T : EditorWindow
    {
        /*
        [MenuItem("Window/Tools/" + nameof(T))]
        static void ShowWindow() => GetWindow<T>();
        */

        private SerializedObject serialObj;
        private Vector2 scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            serialObj = new SerializedObject(this);
        }

        private IEnumerable<string> FieldNames()
        {
            return GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(field => field.Name);
        }

        protected virtual void OnGUI()
        {
            serialObj.Update();

            EditorGUIUtility.wideMode = true;

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(this), GetType(), false);
            GUI.enabled = true;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none);

            var fieldNames = FieldNames();
            foreach (string fieldName in fieldNames)
            {
                SerializedProperty serialProp = serialObj.FindProperty(fieldName);
                EditorGUILayout.PropertyField(serialProp, true);
            }
            serialObj.ApplyModifiedProperties();

            // TODO: Draw additional method parameters as required
            try
            {
                ButtonDrawerHelper.DrawButtonsForType(this.GetType(), this);
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }

        protected Transform Target => Selection.activeGameObject.transform;
    }

    static class ButtonDrawerHelper
    {
        static Dictionary<string, object> data = new(); // TODO: this gets reset every time. Find a way to store and serialize this to preserve

        public static void DrawButtonsForType(Type type, object obj)
        {
            //var methods = typeof(MonoBehaviour) // base
            var members = type // derived
                .GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(o => Attribute.IsDefined(o, typeof(EditorButtonAttribute)));

            int buttonsLeft = -1;
            bool inHorizontalMode = false;
            foreach (var memberInfo in members)
            {
                EditorButtonAttribute attribute = memberInfo.GetCustomAttribute<EditorButtonAttribute>();

                if (memberInfo.GetCustomAttribute<LayoutBeginHorizontal>() != null)
                {
                    GUILayout.BeginHorizontal();
                    inHorizontalMode = true;
                }

                if (attribute.Size != 0)
                {
                    GUILayout.BeginHorizontal();
                    buttonsLeft = attribute.Size;
                }

                ShowAndRunMethod(obj, memberInfo, attribute, setWidth: !inHorizontalMode);
                --buttonsLeft;

                if (memberInfo.GetCustomAttribute<LayoutEndHorizontal>() != null || buttonsLeft == 0)
                {
                    GUILayout.EndHorizontal();
                    inHorizontalMode = false;
                }
            }
        }

        public static void ShowAndRunMethod(object obj, MemberInfo memberInfo, EditorButtonAttribute attribute, bool alignLeft = false, bool setWidth = true)
        {
            GUILayout.BeginHorizontal();

            string label = attribute.GetLabel(memberInfo);
            string tooltip = attribute.Tooltip;

            float buttonWidth = EditorGUIUtility.labelWidth;

            GUIStyle buttonStyle = new(GUI.skin.button);
            if (setWidth)
                buttonStyle.fixedWidth = buttonWidth;
            if (alignLeft)
                buttonStyle.alignment = TextAnchor.MiddleLeft;

            EditorGUIUtility.labelWidth = 80f;
            bool runFunc = GUILayout.Button(new GUIContent(label, tooltip), buttonStyle);

            var methodInfo = memberInfo as MethodInfo;
            List<object> args = new();
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                string key = $"{methodInfo.DeclaringType.Name}::{methodInfo.Name}.{parameter.Name}";

                data.TryGetValue(key, out object value);

                if (parameter.ParameterType == typeof(int))
                {
                    value ??= 0;
                    value = EditorGUILayout.IntField(parameter.Name, (int)(value));
                }
                else if (parameter.ParameterType == typeof(float))
                {
                    value ??= 0f;
                    value = EditorGUILayout.FloatField(parameter.Name, (float)value);
                }
                else if (parameter.ParameterType == typeof(string))
                {
                    value ??= "";
                    value = EditorGUILayout.TextField(parameter.Name, (string)value);
                }
                else if (parameter.ParameterType == typeof(Vector2))
                {
                    value ??= Vector2.zero;
                    value = EditorGUILayout.Vector2Field(parameter.Name, (Vector2)value);
                }
                else if (parameter.ParameterType == typeof(Vector3))
                {
                    value ??= Vector3.zero;
                    value = EditorGUILayout.Vector3Field(parameter.Name, (Vector3)value);
                }
                else if (parameter.ParameterType.IsEnum)
                {
                    value ??= Enum.ToObject(parameter.ParameterType, 0);
                    value = EditorGUILayout.EnumPopup(parameter.Name, (Enum)value);
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(parameter.ParameterType))
                {
                    value ??= null;
                    value = EditorGUILayout.ObjectField(parameter.Name, (UnityEngine.Object)value, parameter.ParameterType, true);
                }

                args.Add(value);
                data[key] = value;
            }
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = buttonWidth;

            if (runFunc)
            {
                methodInfo.Invoke(obj, args.ToArray());
            }
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

    // from https://github.com/augustdominik/SimpleMinMaxSlider/
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minMaxAttribute = (MinMaxSliderAttribute)attribute;
            var propertyType = property.propertyType;

            label.tooltip = minMaxAttribute.min.ToString("F2") + " to " + minMaxAttribute.max.ToString("F2");

            //PrefixLabel returns the rect of the right part of the control. It leaves out the label section. We don't have to worry about it. Nice!
            Rect controlRect = EditorGUI.PrefixLabel(position, label);
            Rect[] splittedRect = SplitRect(controlRect, 3);

            if (propertyType == SerializedPropertyType.Vector2)
            {
                EditorGUI.BeginChangeCheck();

                Vector2 vector = property.vector2Value;
                float minVal = vector.x;
                float maxVal = vector.y;

                //F2 limits the float to two decimal places (0.00).
                minVal = EditorGUI.FloatField(splittedRect[0], float.Parse(minVal.ToString("F2")));
                maxVal = EditorGUI.FloatField(splittedRect[2], float.Parse(maxVal.ToString("F2")));

                EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal,
                minMaxAttribute.min, minMaxAttribute.max);

                if (minVal < minMaxAttribute.min)
                    minVal = minMaxAttribute.min;

                if (maxVal > minMaxAttribute.max)
                    maxVal = minMaxAttribute.max;

                vector = new Vector2(minVal > maxVal ? maxVal : minVal, maxVal);

                if (EditorGUI.EndChangeCheck())
                    property.vector2Value = vector;
            }
            else if (propertyType == SerializedPropertyType.Vector2Int)
            {

                EditorGUI.BeginChangeCheck();

                Vector2Int vector = property.vector2IntValue;
                float minVal = vector.x;
                float maxVal = vector.y;

                minVal = EditorGUI.FloatField(splittedRect[0], minVal);
                maxVal = EditorGUI.FloatField(splittedRect[2], maxVal);

                EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal,
                minMaxAttribute.min, minMaxAttribute.max);

                if (minVal < minMaxAttribute.min)
                    maxVal = minMaxAttribute.min;

                if (maxVal > minMaxAttribute.max)
                    maxVal = minMaxAttribute.max;

                vector = new Vector2Int(Mathf.FloorToInt(minVal > maxVal ? maxVal : minVal), Mathf.FloorToInt(maxVal));

                if (EditorGUI.EndChangeCheck())
                    property.vector2IntValue = vector;
            }
        }

        Rect[] SplitRect(Rect rectToSplit, int n)
        {
            Rect[] rects = new Rect[n];
            for (int i = 0; i < n; i++)
                rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n, rectToSplit.height);

            int padding = (int)rects[0].width - 40;
            int space = 5;

            rects[0].width -= padding + space;
            rects[2].width -= padding + space;

            rects[1].x -= padding;
            rects[1].width += padding * 2;

            rects[2].x += padding + space;
            return rects;
        }

    }

    // Allows to draw all elements of a struct or class in a single line for nicer view inside arrays or lists.
    // Subclass this class and override properties and widths.
    /*
    [CustomPropertyDrawer(typeof(_TYPE_))]
    public class _TYPE_Drawer : ArrayInlineDrawer
    {
        override protected string[] properties => new[] { "myproperty", ... };
        override protected float[] widths => new[] { 0.5f, ... };
    }
    */
    public abstract class ArrayInlineDrawer : PropertyDrawer
    {
        protected virtual string[] properties => null;
        protected virtual float[] widths => null;
        float margin = 10f;

        // adapted from https://docs.unity3d.com/ScriptReference/PropertyDrawer.html
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float offset = 0f;
            for (int i = 0; i < properties.Length; i++)
            {
                float width = widths[i] * position.width;
                Rect propRect = new Rect(position.x + offset, position.y, width, position.height);
                offset += width + margin;
                EditorGUI.PropertyField(propRect, property.FindPropertyRelative(properties[i]), GUIContent.none);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
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
    public class EditorButtonAttribute : Attribute
    {
        public EditorButtonAttribute(string _ = "", string _2 = "")
        {
        }
    }
    public class TabHierarchyAttribute : Attribute
    {
    }
    public class RangeExAttribute : PropertyAttribute
    {
        public RangeExAttribute(float min, float max, float step)
        {
        }
    }

#endif
}