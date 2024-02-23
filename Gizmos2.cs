// (c) Simone Guggiari 2020-2024

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

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
            UnityEditor.Handles.matrix = Gizmos.matrix;
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
            UnityEditor.Handles.matrix = Matrix4x4.identity;
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
        public static void DrawCircle(Vector3 center, float radius)
        {
            DrawCircle(center, radius, Vector3.forward);
        }
        public static void DrawCircle       (Vector3 center, float radius, Vector3 normal)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.lighting = false;
            UnityEditor.Handles.DrawSolidDisc(center, normal, radius);
#endif
        }

        public static void DrawLabel        (string content, Vector3 position, float minzoom)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView?.camera;
            float zoomFactor = sceneCamera != null ? sceneCamera.orthographicSize : 1f;
            float maxzoom = minzoom * 3f;
            float a = Mathf.Clamp01(1f-Mathf.InverseLerp(minzoom, maxzoom, zoomFactor));
            if (a <= 0)
                return;
            Color c = Gizmos.color;
            Color c2 = c;
            c2.a *= a;
            Gizmos.color = c2;
            DrawLabel(content, position);
            Gizmos.color = c;
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

        public static void DrawPath         (IEnumerable<Vector2> points, bool closed = false)
        {
            DrawPath(points.Select(p => (Vector3)p), closed);
        }
        public static void DrawPath         (IEnumerable<Vector3> points, bool closed = false)
        {
            if (points == null)
                return;
#if (SEDITOR && UNITY_EDITOR)
            Handles.DrawAAPolyLine(10f, points.ToArray());
            Handles.color = Gizmos.color;
            Handles.DrawPolyLine(points.ToArray());
            if (closed && points.Count() >= 2)
                Gizmos.DrawLine(points.Last(), points.First());
#endif
        }

        public static void DrawTransform(this Transform transform, float scale = 0.1f, float alpha = 0.93f)
        {
            DrawTransform(transform.position, transform.rotation, scale, alpha);
        }
        public static void DrawTransform(Vector3 position, Quaternion rotation, float scale = 0.1f, float alpha = 0.93f)
        {
            Color color = Gizmos.color;
            Gizmos.color = new Color32(219, 62, 29, 237); // unity red
            Gizmos2.DrawLine(position, position + (rotation * Vector3.right) * scale, 2f);
            Gizmos.color = new Color32(154, 243, 72, 237); // unity green
            Gizmos2.DrawLine(position, position + (rotation * Vector3.up) * scale, 2f);
            Gizmos.color = new Color32(58, 122, 248, 237); // unity blue
            Gizmos2.DrawLine(position, position + (rotation * Vector3.forward) * scale, 2f);
            Gizmos.color = color;
        }

        public static void DrawLine(Vector3 from, Vector3 to, float thickness = 3f)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawLine(from, to, thickness);
#else
            Gizmos.DrawLine(from, to);
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

        public static void DrawPyramid(Vector3 position, Quaternion rotation, float height, float baseRadius, int sides = 4, float thickness = 0f)
        {
            Vector3 base0 = rotation * Vector3.right;
            Vector3 base1 = rotation * Vector3.up;
            Vector3 forward = rotation * Vector3.forward;
            Vector3 from = position;
            Vector3 to = position + forward * height;

            float step = 360f / sides;
            float stepOffset = step/2;

            Vector3[] point = new Vector3[2];
            Vector2 dir = Utility.DirectionFromAngle(stepOffset) * baseRadius;
            point[0] = from + base0 * dir[0] + base1 * dir[1];

            for (int i = 1; i <= sides; ++i)
            {
                dir = Utility.DirectionFromAngle(step * i + stepOffset) * baseRadius;
                point[1] = from + base0 * dir[0] + base1 * dir[1];
                Gizmos2.DrawLine(point[0], point[1], thickness);
                Gizmos2.DrawLine(point[0], to, thickness);
                point[0] = point[1];
            }
        }

        public static void DrawDottedLine(Vector3 from, Vector3 to, float screenSpaceSize = 5f)
        {
#if (SEDITOR && UNITY_EDITOR)
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawDottedLine(from, to, screenSpaceSize);
#endif
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

        public static void DrawPoint(Vector3 point, float size)
        {
            DrawCross(point, size);
        }
        public static void DrawCross(Vector3 point, float size)
        {
            float radius = size / 2f;
            Gizmos.DrawLine(point - Vector3.right * radius, point + Vector3.right * radius);
            Gizmos.DrawLine(point - Vector3.up * radius, point + Vector3.up * radius);
            Gizmos.DrawLine(point - Vector3.forward * radius, point + Vector3.forward * radius);
        }

        public static void DrawWireCapsule(Capsule capsule)
        {
            DrawWireCapsule(capsule.center, capsule.rotation, capsule.radius, capsule.height);
        }
        public static void DrawWireCapsule(Vector3 pos, Quaternion rot, float radius, float height)
        {
#if (SEDITOR && UNITY_EDITOR)
            // from https://forum.unity.com/threads/drawing-capsule-gizmo.354634/
            Handles.color = Gizmos.color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (height - (radius * 2)) / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
#endif
        }
        public static void DrawArrow(Vector3 from, Vector3 to, float size = 0.1f, bool invert = false)
        {
            if (invert)
                Utility.Swap(ref from, ref to);
            Gizmos.DrawLine(from, to);
            Vector2 dir = (to-from).normalized *size;
            Gizmos.DrawLine(to, to - Quaternion.Euler(0, 0, 20) * dir);
            Gizmos.DrawLine(to, to - Quaternion.Euler(0, 0, -20) * dir);
        }

        public static void DrawWireArc(Vector2 center, Vector2 from, float angle, float radius)
        {
#if (SEDITOR && UNITY_EDITOR)
            Handles.color = Gizmos.color;
            Handles.DrawWireArc(center, Vector3.forward, from, angle, radius);
#endif
        }
    }
}
