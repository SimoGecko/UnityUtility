// (c) Simone Guggiari 2022-2023

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

////////// PURPOSE:  //////////

namespace sxg
{

    public static class RectHelper
    {
        public static Rect Part(this Rect r, float min, float max)
        {
            return new(r.GetPoint(min, 0f), new Vector2(r.width * (max - min), r.height));
        }
    }


#if SEDITOR
    [CustomPropertyDrawer(typeof(PID))]
    public class PidDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Find the SerializedProperties by name
            var p = property.FindPropertyRelative("Kp");
            var d = property.FindPropertyRelative("Kd");
            var i = property.FindPropertyRelative("Ki");
            var angle = property.FindPropertyRelative("isAngle");

            EditorGUI.BeginProperty(rect, label, property);
            {
                EditorGUIUtility.labelWidth = 30;
                EditorGUI.LabelField(rect.Part(0f, 0.43f), label);
                p.floatValue = EditorGUI.FloatField(rect.Part(0.43f, 0.58f), "P", p.floatValue);
                d.floatValue = EditorGUI.FloatField(rect.Part(0.58f, 0.73f), "D", d.floatValue); // NOTE: the order PDI
                i.floatValue = EditorGUI.FloatField(rect.Part(0.73f, 0.88f), "I", i.floatValue);
                angle.boolValue = EditorGUI.Toggle(rect.Part(0.88f, 1.00f), "angle", angle.boolValue);
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
                EditorGUIUtility.labelWidth = 30;
                EditorGUI.LabelField(rect.Part(0f, 0.43f), label);
                min.floatValue = EditorGUI.FloatField(rect.Part(0.43f, 0.62f), "Min", min.floatValue);
                max.floatValue = EditorGUI.FloatField(rect.Part(0.62f, 0.81f), "Max", max.floatValue);
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
                EditorGUIUtility.labelWidth = 40;
                EditorGUI.LabelField(rect.Part(0f, 0.43f), label);
                time.floatValue = EditorGUI.FloatField(rect.Part(0.43f, 0.62f), "Time", time.floatValue);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.FloatField(rect.Part(0.62f, 0.81f), "Target", target.floatValue);
                EditorGUI.FloatField(rect.Part(0.81f, 1.00f), "Value", value.floatValue);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(Throttler))]
    public class ThrottlerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var timeBetween = property.FindPropertyRelative("timeBetween");

            EditorGUI.BeginProperty(rect, label, property);
            {
                EditorGUIUtility.labelWidth = 80;
                EditorGUI.LabelField(rect.Part(0f, 0.4f), label);
                timeBetween.floatValue = EditorGUI.FloatField(rect.Part(0.4f, 1.0f), "timeBetween", timeBetween.floatValue);
            }
            EditorGUI.EndProperty();
        }
    }
#endif

}