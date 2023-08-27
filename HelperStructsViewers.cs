// (c) Simone Guggiari 2022

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

////////// PURPOSE:  //////////

namespace sxg
{

#if SEDITOR
    [CustomPropertyDrawer(typeof(PID))]
    public class PidDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Find the SerializedProperties by name
            var p = property.FindPropertyRelative("proportionalGain");
            var i = property.FindPropertyRelative("integralGain");
            var d = property.FindPropertyRelative("derivativeGain");

            EditorGUI.BeginProperty(rect, label, property);
            {
                Rect RectPart(Rect r, float min, float max) => new Rect(rect.GetPoint(min, 0f), new Vector2(rect.width * (max - min), rect.height));
                EditorGUIUtility.labelWidth = 30;
                EditorGUI.LabelField(RectPart(rect, 0f, 0.43f), label);
                p.floatValue = EditorGUI.FloatField(RectPart(rect, 0.43f, 0.62f), "P", p.floatValue);
                i.floatValue = EditorGUI.FloatField(RectPart(rect, 0.62f, 0.81f), "I", i.floatValue);
                d.floatValue = EditorGUI.FloatField(RectPart(rect, 0.81f, 1.00f), "D", d.floatValue);
            }
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(Range))]
    public class RangeDrawer : PropertyDrawer
    {
        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{
        //    return EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2);
        //}

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Find the SerializedProperties by name
            var min = property.FindPropertyRelative("min"); // nameof(min)
            var max = property.FindPropertyRelative("max"); 

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(rect, label, property);
            {
                // Makes the fields disabled / grayed out
                //EditorGUI.BeginDisabledGroup(true);
                //{
                //    // In your case the best option would be a Vector3Field which handles the correct drawing
                //    EditorGUI.Vector3IntField(position, label, new Vector3Int(x.intValue, y.intValue, z.intValue));
                //}
                //EditorGUI.EndDisabledGroup();
                if (!EditorGUIUtility.wideMode)
                {
                    EditorGUIUtility.wideMode = true;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
                }
                Rect RectPart(Rect r, float min, float max) => new Rect(rect.GetPoint(min, 0f), new Vector2(rect.width * (max - min), rect.height));
                EditorGUIUtility.labelWidth = 30;
                EditorGUI.LabelField(RectPart(rect, 0f, 0.43f), label);
                min.floatValue = EditorGUI.FloatField(RectPart(rect, 0.43f, 0.62f), "Min", min.floatValue);
                max.floatValue = EditorGUI.FloatField(RectPart(rect, 0.62f, 0.81f), "Max", max.floatValue);
            }
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(Smooth<float>))]
    public class SmoothFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Find the SerializedProperties by name
            var target = property.FindPropertyRelative("target");
            var value  = property.FindPropertyRelative("value");
            var time   = property.FindPropertyRelative("time");

            EditorGUI.BeginProperty(rect, label, property);
            {
                Rect RectPart(Rect r, float min, float max) => new Rect(rect.GetPoint(min, 0f), new Vector2(rect.width * (max - min), rect.height));
                EditorGUIUtility.labelWidth = 40;
                EditorGUI.LabelField(RectPart(rect, 0f, 0.43f), label);
                time.floatValue = EditorGUI.FloatField(RectPart(rect, 0.43f, 0.62f), "Time", time.floatValue);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.FloatField(RectPart(rect, 0.62f, 0.81f), "Target", target.floatValue);
                EditorGUI.FloatField(RectPart(rect, 0.81f, 1.00f), "Value", value.floatValue);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndProperty();
        }
    }
#endif

}