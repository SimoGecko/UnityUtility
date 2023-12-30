
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace sxg
{
#if SEDITOR
    public class VariablePlotterWindow : EditorWindow
    {
        public GameObject targetObject;
        readonly float recordTime = 20f;
        readonly float plotHeight = 100f;
        readonly float dt = 1f / 30f;
        Vector3 offset = new(-20f, 1f);

        Dictionary<PlotData, List<float>> table = new();
        PlotData[] plots;
        Color unityGray = (Color)(new Color32(100, 100, 100, 255));

        [MenuItem("Window/Variable Plotter")]
        public static void ShowWindow()
        {
            GetWindow<VariablePlotterWindow>("Variable Plotter");
        }

        public void SetTargetObject(GameObject newTargetObject)
        {
            if (newTargetObject != targetObject || plots == null)
            {
                targetObject = newTargetObject;
                plots = GetPlots().ToArray();
                table = new();
                foreach (PlotData data in plots)
                    table.Add(data, new());
            }
        }

        private void OnGUI()
        {
            GameObject newTargetObject = EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true) as GameObject;
            SetTargetObject(newTargetObject);
            //if (newTargetObject == null && Application.isPlaying)
            //    SetTargetObject(FindObjectOfType<sxg.rfps.MasterController>()?.gameObject); // HACK

            Rect screenRect = new();

            if (plots != null)
            {
                int maxSamples = Mathf.RoundToInt(recordTime / dt);
                int plotCount = 0;
                Range valueRange = new();
                foreach (PlotData plot in plots)
                {
                    List<float> values = table[plot];
                    values.Add(plot.Value);
                    while (values.Count > maxSamples)
                        values.RemoveAt(0);

                    if (!plot.UseSameGraph)
                    {
                        EditorGUILayout.GetControlRect(false, 5f); // some space
                        screenRect = EditorGUILayout.GetControlRect(false, plotHeight);
                        Handles.color = unityGray;
                        Handles.DrawSolidRectangleWithOutline(screenRect, unityGray, unityGray);
                        plotCount = 0;

                        // compute range
                        valueRange = plot.Range();
                        if (plot.AutoRange)
                        {
                            float min = values.Min();
                            float max = values.Max();
                            if (min != max)
                                valueRange = new Range(min, max);
                        }
                    }
                    else
                    {
                        ++plotCount;
                    }

                    Debug.Assert(table.ContainsKey(plot));

                    List<Vector3> graphPoints = new();
                    for (int i = 0; i < values.Count; i++)
                    {
                        float time = (float)i / maxSamples; // [0..1]
                        float value = Utility.InverseLerpUnclamped(valueRange.Min, valueRange.Max, values[i]); // [0..1] unclamped
                        Vector3 p = screenRect.GetPoint(new Vector2(time, 1f - value));
                        graphPoints.Add(p);
                    }

                    Handles.color = plot.Color;
                    Handles.DrawAAPolyLine(2f, graphPoints.ToArray());

                    GUIStyle style = new();
                    style.normal.textColor = Handles.color;
                    Handles.Label(graphPoints[^1] + offset, values[^1].ToString("0.00"), style);

                    float line = EditorGUIUtility.singleLineHeight;
                    Handles.Label(screenRect.GetPoint(0f, 1f) + new Vector2(0f, -line * (plotCount + 1)), plot.Label, style);
                }
            }
        }

        float nextDrawTime;
        private void Awake()
        {
            nextDrawTime = 0f;
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                nextDrawTime = Mathf.Clamp(nextDrawTime, 0f, dt);
                nextDrawTime -= Time.deltaTime;
                if (nextDrawTime <= 0f)
                {
                    nextDrawTime = dt;
                    Repaint();
                }
            }
        }

        List<PlotData> GetPlots()
        {
            List<PlotData> ans = new();
            if (targetObject != null)
            {
                // Get all components on the target object.
                MonoBehaviour[] components = targetObject.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour component in components)
                {
                    // Get all fields in the component.
                    FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                    foreach (FieldInfo field in fields)
                    {
                        PlotAttribute plotAttribute = (PlotAttribute)System.Attribute.GetCustomAttribute(field, typeof(PlotAttribute));
                        if (plotAttribute != null)
                            ans.Add(new(plotAttribute, field, component));
                    }
                }
            }
            return ans;
        }

        private struct PlotData
        {
            private PlotAttribute att;
            private FieldInfo field;
            private MonoBehaviour component;

            public PlotData(PlotAttribute att, FieldInfo field, MonoBehaviour component)
            {
                this.att = att;
                this.field = field;
                this.component = component;
            }

            public float Value => (float)field.GetValue(component);
            public Range Range()
            {
                float margin = (att.maxValue - att.minValue) * 0.05f;
                return new Range(att.minValue - margin, att.maxValue + margin);
            }
            public Color Color => att.color;
            public bool UseSameGraph => att.sameGraph;
            public string Label => att.label;
            public bool AutoRange => att.autoRange;
        }

    }

    public class PlotAttribute : PropertyAttribute
    {
        public bool autoRange;
        public float minValue;
        public float maxValue;
        public Color color;
        public bool sameGraph;
        public string label;

        public PlotAttribute(string color = "red", string label = "", bool sameGraph = false)
            : this(-1f, 1f, color, label, sameGraph)
        {
            autoRange = true;
        }
        public PlotAttribute(float minValue, float maxValue, string color = "red", string label = "", bool sameGraph = false)
        {
            autoRange = false;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.color = ColorNameToColor(color);
            this.sameGraph = sameGraph;
            this.label = label;
        }

        private static Color ColorNameToColor(string name)
        {
            switch (name)
            {
            case "red": return Color.red;
            case "green": return Color.green;
            case "blue": return Color.blue;
            case "magenta": return Color.magenta;
            case "yellow": return Color.yellow;
            case "cyan": return Color.cyan;
            case "white": return Color.white;
            case "black": return Color.black;
            }
            return Color.white;
        }
    }
#else
    public class PlotAttribute : PropertyAttribute
    {

        public PlotAttribute(string color = "", bool sameGraph = false, string label = null)
        {
        }
        public PlotAttribute(float minValue, float maxValue, string color, bool sameGraph = false, string label = null)
        {
        }
    }
#endif
}
