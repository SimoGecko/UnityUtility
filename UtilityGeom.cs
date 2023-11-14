// (c) Simone Guggiari 2020-2023

using System.Collections.Generic;
using UnityEngine;

////////// PURPOSE: Various Geometric objects and functions //////////

namespace sxg
{
    [System.Serializable]
    public struct Triangle
    {
        public Vector2 a, b, c;

        public static Triangle zero = new(Vector2.zero, Vector2.zero, Vector2.zero);
        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public float Area  { get { return Utility.GetTriangleArea  (a, b, c); } }
        public Vector2 Com { get { return Utility.GetTriangleCenter(a, b, c); } }
    }

    [System.Serializable]
    public struct Segment
    {
        public Vector2 a, b;

        public static Segment zero = new(Vector2.zero, Vector2.zero);
        public Segment(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }

        public float Length                            { get { return Vector2.Distance(a, b); } }
        public bool Intersects(Segment other)          { return Utility.SegmentsIntersect(a, b, other.a, other.b); }
        public Vector2 FindIntersection(Segment other) { return Utility.FindSegmentsIntersectionExpected(a, b, other.a, other.b); }

        public float DistanceToPoint(Vector2 p)        { return Mathf.Sqrt(DistanceToPointSquared(p)); }
        public float DistanceToPointSquared(Vector2 p) { return Utility.FindSegmentDistanceToPointSquared(a, b, p); }
    }

    [System.Serializable]
    public struct Circle
    {
        public Vector2 center;
        public float radius;

        public static Circle zero = new(Vector2.zero, 0f);
        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool Contains(Vector2 point)
        {
            return Vector2.SqrMagnitude(point - center) <= radius * radius;
        }

        public float Area                  { get { return radius * radius * Mathf.PI; } }
        public float Perimeter             { get { return radius * 2f * Mathf.PI; } }
        public Vector2 GetPoint(Vector2 v) { return center + v * radius; }

        public static Circle Lerp(Circle a, Circle b, float t)
        {
            return new Circle(Vector2.Lerp(a.center, b.center, t), Mathf.Lerp(a.radius, b.radius, t));
        }
    }

    [System.Serializable]
    public struct Frustum
    {
        private float fov, aspect, near, far;
        private Matrix4x4 projectionMatrix, localToWorldMatrix;
        private Plane[] planes;

        public Frustum(float fov, float aspect, float near, float far, Transform transform)
        {
            this.fov = fov;
            this.aspect = aspect;
            this.near = near;
            this.far = far;

            projectionMatrix = Matrix4x4.Perspective(fov, aspect, near, far);
            localToWorldMatrix = Matrix4x4.identity;
            planes = null;
            Update(transform);
        }

        public void Update(Transform transform)
        {
            localToWorldMatrix = transform.localToWorldMatrix;

            Matrix4x4 camMatrix = localToWorldMatrix.inverse;
            // from https://docs.unity3d.com/ScriptReference/Camera-worldToCameraMatrix.html:
            // Note that camera space matches OpenGL convention: camera's forward is the negative Z axis.
            // This is different from Unity's convention, where forward is the positive Z axis.
            camMatrix.SetRow(2, -camMatrix.GetRow(2));
            Matrix4x4 worldToProjectionMatrix = projectionMatrix * camMatrix;
            planes = GeometryUtility.CalculateFrustumPlanes(worldToProjectionMatrix);
        }

        public bool Contains(Vector3 point)
        {
            foreach (Plane p in planes)
            {
                if (!p.GetSide(point))
                    return false;
            }
            return true;
        }

        public Plane[] Planes => planes;
        public float Fov => fov;
        public float Aspect => aspect;
        public float Near => near;
        public float Far => far;
        public Matrix4x4 Matrix => localToWorldMatrix;
    }

    [System.Serializable]
    public struct Capsule // currently assumed to be about the Y index
    {
        public Vector3 center;
        public float radius;
        public float height;
        Quaternion rot;

        public Capsule(Vector3 center, float radius, float height)
        {
            this.center = center;
            this.radius = radius;
            this.height = height;
            rot = Quaternion.identity;
        }
        public Capsule(CapsuleCollider collider)
        {
            this.center = collider.center;
            this.radius = collider.radius;
            this.height = collider.height;
            rot = Quaternion.identity;
        }
        public Capsule(Capsule other)
        {
            this.center = other.center;
            this.radius = other.radius;
            this.height = other.height;
            rot = Quaternion.identity;
        }

        public float HalfBodyHeight => Mathf.Max(height / 2f - radius, 0f);
        public Vector3 Point1 => center + Axis * HalfBodyHeight;
        public Vector3 Point2 => center - Axis * HalfBodyHeight;
        public Vector3 Axis => rot * Vector3.up;
        public Quaternion rotation => rot;

        public Vector3 Top    => center + Axis * (HalfBodyHeight + radius);
        public Vector3 Bottom => center - Axis * (HalfBodyHeight + radius);

        public void Transform(Transform t)
        {
            center = t.TransformPoint(center);
            rot = t.rotation;
        }
        public void Translate(Vector3 delta)
        {
            center += delta;
        }
        public void Expand(float delta)
        {
            radius += delta;
            height += delta * 2f;
        }
    }

}
