// (c) Simone Guggiari 2020-2022

using System.Collections.Generic;
using UnityEngine;

////////// PURPOSE: Various Gizmos utility functions //////////

namespace sxg
{
    public static class Gizmos2
    {
        public static void DrawWireCircle   (Circle circle, bool drawAxes = false)
        {
            DrawWireCircle(circle.center, circle.radius, drawAxes);
        }
        public static void DrawWireCircle   (Vector3 center, float radius, bool drawAxes = false)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawWireDisc(center, Vector3.forward, radius);
            if (drawAxes)
            {
                Gizmos.DrawLine(center - Vector3.right * radius, center + Vector3.right * radius);
                Gizmos.DrawLine(center - Vector3.up * radius, center + Vector3.up * radius);
            }
#endif
        }
        public static void DrawWireCircleXZ (Vector3 center, float radius, bool drawAxes = false)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawWireDisc(center, Vector3.up, radius);
            if (drawAxes)
            {
                Gizmos.DrawLine(center - Vector3.right * radius, center + Vector3.right * radius);
                Gizmos.DrawLine(center - Vector3.forward * radius, center + Vector3.forward * radius);
            }
#endif
        }
        public static void DrawWireCube     (Vector3 center, Vector3 size)
        {
            Gizmos.DrawWireCube(center, size);
        }
        public static void DrawWireRect     (Vector3 center, Vector2 size)
        {
            Gizmos.DrawWireCube(center, size);
        }
        public static void DrawWireRect     (Vector2 pointMin, Vector2 pointMax)
        {
            Rect rect = new(pointMin, pointMax - pointMin);
            DrawWireRect(rect);
        }
        public static void DrawWireRect     (Rect rect)
        {
            DrawWireRect(rect.center, rect.size);
        }
        public static void DrawWireSphere   (Vector3 center, float radius)
        {
            Gizmos.DrawWireSphere(center, radius);
        }

        public static void DrawCircle       (Vector3 center, float radius)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawSolidDisc(center, Vector3.forward, radius);
#endif
        }

        public static void DrawLabel        (string content, Vector3 position, int fontSize = 8)
        {
            DrawLabel(content, position, Gizmos.color, fontSize);
        }
        public static void DrawLabel        (string content, Vector3 position, Color fontColor, int fontSize = 8, TextAnchor alignment = TextAnchor.UpperLeft, bool worldSize = false)
        {
#if (SEDITOR && UNITY_EDITOR)
            GUIStyle style = new();
            if (worldSize)
            {
                fontSize = Mathf.RoundToInt(fontSize * GetEditorPpu());
                fontSize = Mathf.Clamp(fontSize, 1, 512);
                //Debug.Log("fontsize: " + fontSize);
            }
            style.normal.textColor = fontColor;
            style.fontSize = fontSize;
            style.alignment = alignment;
            UnityEditor.Handles.Label(position, content, style);
#endif
        }
        public static float GetEditorPpu()
        {
            GameObject sceneCamObj = GameObject.Find("SceneCamera");
            Vector2 resolution = sceneCamObj?.GetComponent<Camera>().pixelRect.size ?? new(1920, 1080);
            float sceneSize = UnityEditor.SceneView.lastActiveSceneView.size;
            float ppu = resolution.magnitude / sceneSize;
            return ppu;
        }

        public static void DrawPath         (params Vector3[] points)
        {
            for (int i = 0; i < points.Length-1; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
            Gizmos.DrawLine(points[^1], points[0]);
        }

    }
}
