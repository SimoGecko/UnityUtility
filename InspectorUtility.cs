// (c) Simone Guggiari 2020-2024

using System.Collections.Generic;

using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor;
using System.Drawing;
using System.Reflection.Emit;
#if SNETCODE
using Unity.Netcode;
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

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class RangeExAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;
        public readonly float step;

        public RangeExAttribute(float min, float max, float step)
        {
            this.min = min;
            this.max = max;
            this.step = step;
        }
    }

    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public float min;
        public float max;

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
        public Foldout(string label ="", int size = 1)
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
                foreach(var member in members)
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

    static class ButtonDrawerHelper
    {
        public static void DrawButtonsForType(Type type, MonoBehaviour mono)
        {
            //var methods = typeof(MonoBehaviour) // base
            var methods = type // derived
                .GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(o => Attribute.IsDefined(o, typeof(EditorButtonAttribute)));

            int buttonsLeft = -1;
            foreach (var memberInfo in methods)
            {
                EditorButtonAttribute attribute = memberInfo.GetCustomAttribute<EditorButtonAttribute>();
                string label = string.IsNullOrEmpty(attribute.Label) ? Prettify(memberInfo.Name) : attribute.Label;

                if (memberInfo.GetCustomAttribute<LayoutBeginHorizontal>() != null)
                {
                    GUILayout.BeginHorizontal();
                }

                if (attribute.Size != 0)
                {
                    GUILayout.BeginHorizontal();
                    buttonsLeft = attribute.Size;
                }
                if (GUILayout.Button(new GUIContent(label, attribute.Tooltip)))
                {
                    var method = memberInfo as MethodInfo;
                    method.Invoke(mono, null);
                }
                --buttonsLeft;

                if (memberInfo.GetCustomAttribute<LayoutEndHorizontal>() != null || buttonsLeft == 0)
                {
                    GUILayout.EndHorizontal();
                }
            }
        }

        public static string Prettify(string methodName)
        {
            // Check that is starts with EDITOR_
            if (methodName.StartsWith("EDITOR_"))
            {
                methodName = methodName[7..];
            }
            else
            {
                //Debug.LogWarning($"Editor method {methodName} should start with EDITOR_");
            }
            return Utility.AddSpacesBeforeCapitalLetters(methodName);
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