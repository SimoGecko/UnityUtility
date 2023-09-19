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
            DrawWireCircle(center, radius, Vector3.forward, drawAxes);
        }
        public static void DrawWireCircleXZ (Vector3 center, float radius, bool drawAxes = false)
        {
            DrawWireCircle(center, radius, Vector3.up, drawAxes);
        }
        public static void DrawWireCircleCamFacing(Vector3 center, float radius, bool drawAxes = false)
        {
            DrawWireCircle(center, radius, Utility.EditorCameraPos() - center, drawAxes);
        }
        public static void DrawWireCircle   (Vector3 center, float radius, Vector3 normal, bool drawAxes = false)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawWireDisc(center, normal, radius);
            if (drawAxes)
            {
                Vector3 axis1 = Vector3.zero;
                Vector3 axis2 = Vector3.zero;
                Vector3.OrthoNormalize(ref normal, ref axis1, ref axis2);
                Gizmos.DrawLine(center - axis1 * radius, center + axis1 * radius);
                Gizmos.DrawLine(center - axis2 * radius, center + axis2 * radius);
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

        public static void DrawWireCone     (Vector3 from, Vector3 to, float radius)
        {
            DrawWireCircle(from, radius, to - from);
            (to - from).GetTangents(out Vector3 t0, out Vector3 t1);
            Gizmos.DrawLine(from + t0 * radius, to);
            Gizmos.DrawLine(from - t0 * radius, to);
            Gizmos.DrawLine(from + t1 * radius, to);
            Gizmos.DrawLine(from - t1 * radius, to);
        }

        public static void DrawWireEllipse  (Vector3 center, float A, float B)
        {
            DrawWireEllipse(center, A, B, Quaternion.identity);
        }
        public static void DrawWireEllipse(Vector3 center, float A, float B, Quaternion orientation)
        {
            //Gizmos.matrix = Matrix4x4.TRS(center, orientation, new Vector3(A, B, 1));
            //Gizmos.DrawWireSphere(Vector3.zero, 1f);
            //Gizmos.matrix = Matrix4x4.identity;
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.matrix = Matrix4x4.TRS(center, orientation, new Vector3(A, B, 1));
            DrawWireCircle(Vector3.zero, 1f);
            UnityEditor.Handles.matrix = Matrix4x4.identity;
#endif
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
#if (SEDITOR && UNITY_EDITOR)
            GameObject sceneCamObj = GameObject.Find("SceneCamera");
            Vector2 resolution = sceneCamObj?.GetComponent<Camera>().pixelRect.size ?? new(1920, 1080);
            float sceneSize = UnityEditor.SceneView.lastActiveSceneView.size;
            float ppu = resolution.magnitude / sceneSize;
            return ppu;
#else
            return 1f;
#endif
        }

        public static void DrawPath         (Vector3[] points, bool closed = false)
        {
            if (points == null)
                return;
            for (int i = 0; i < points.Length-1; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
            if (closed && points.Length >= 2)
                Gizmos.DrawLine(points[^1], points[0]);
        }

        public static void DrawTransform(this Transform transform, float scale = 0.1f, float alpha = 0.93f)
        {
            DrawTransform(transform.position, transform.rotation, scale, alpha);
        }
        public static void DrawTransform(Vector3 position, Quaternion rotation, float scale = 0.1f, float alpha = 0.93f)
        {
            Gizmos.color = new Color32(219, 62, 29, 237); // unity red
            Gizmos2.DrawLine(position, position + (rotation * Vector3.right) * scale, 2f);
            Gizmos.color = new Color32(154, 243, 72, 237); // unity green
            Gizmos2.DrawLine(position, position + (rotation * Vector3.up) * scale, 2f);
            Gizmos.color = new Color32(58, 122, 248, 237); // unity blue
            Gizmos2.DrawLine(position, position + (rotation * Vector3.forward) * scale, 2f);
        }

        public static void DrawLine(Vector3 from, Vector3 to, float thickness = 3f)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawLine(from, to, thickness);
#endif
        }

        public static void DrawPath(int steps, Color color, System.Func<float, Vector3> func)
        {
            Gizmos.color = color;
            float dt = 1f / steps;
            Vector3 p = func(0f);
            for (int i = 1; i <= steps; i++)
            {
                float t = i * dt;
                Vector3 np = func(t);
                Gizmos.DrawLine(p, np);
                p = np;
            }
        }

        public static void DrawFrustum(Frustum frustum)
        {
            Gizmos.matrix = frustum.Matrix;
            Gizmos.DrawFrustum(Vector3.zero, frustum.Fov, frustum.Far, frustum.Near, frustum.Aspect);
            Gizmos.matrix = Matrix4x4.identity;

            //foreach (Plane p in frustum.Planes)
            //    DrawPlane(p);
        }

        public static void DrawPlane(Plane plane, float size = 10f)
        {
            Vector3 o = plane.GetOrigin();
            DrawPlaneHelper(plane, o, size);
        }

        public static void DrawPlane(Plane plane, Vector3 center, float size = 10f)
        {
            Vector3 o = plane.ClosestPointOnPlane(center);
            DrawPlaneHelper(plane, o, size);
        }

        private static void DrawPlaneHelper(Plane plane, Vector3 o, float size)
        {
            Vector3 n = plane.normal;
            Gizmos.DrawLine(o, o + n);

            Gizmos.matrix = Matrix4x4.LookAt(o, o + n, Vector3.up);
            Gizmos.DrawCube(Vector3.zero, new Vector3(1f, 1f, 0f) * size);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
