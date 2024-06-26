﻿// (c) Simone Guggiari 2020-2024

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

        public float Area => Utility.GetTriangleArea(a, b, c);
        public Vector2 Com => Utility.GetTriangleCenter(a, b, c);
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

        public float Length                            => Vector2.Distance(a, b);
        public bool Intersects(Segment other)          => Utility.SegmentsIntersect(a, b, other.a, other.b);
        public Vector2 FindIntersection(Segment other) => Utility.FindSegmentsIntersectionExpected(a, b, other.a, other.b);

        public float DistanceToPoint(Vector2 p)        => Mathf.Sqrt(DistanceToPointSquared(p));
        public float DistanceToPointSquared(Vector2 p) => Utility.FindSegmentDistanceToPointSquared(a, b, p);
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

        public float Area                  => radius * radius * Mathf.PI;
        public float Perimeter             => radius * 2f * Mathf.PI;
        public Vector2 GetPoint(Vector2 v) => center + v * radius;

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
        public float height; // top to bottom
        //Quaternion rot;

        public Capsule(Vector3 center, float radius, float height)
            : this(center, radius, height, Quaternion.identity) { }
        public Capsule(Vector3 center, float radius, float height, Quaternion rot)
        {
            this.center = center;
            this.radius = radius;
            this.height = height;
            //this.rot = rot;
        }
        public Capsule(CapsuleCollider collider)
        {
            center = collider.center;
            radius = collider.radius;
            height = collider.height;
            //rot = Quaternion.identity;
        }
        public Capsule(Capsule other)
        {
            center = other.center;
            radius = other.radius;
            height = other.height;
            //rot = other.rot;
        }

        public Quaternion rotation => Quaternion.identity;
        public Vector3 Axis => rotation * Vector3.up;
        public Vector3 Top    => center + Axis * height / 2f;
        public Vector3 Bottom => center - Axis * height / 2f;

        public float HalfBodyHeight => Mathf.Max(height / 2f - radius, 0f);
        public Vector3 Point1 => center + Axis * HalfBodyHeight;
        public Vector3 Point2 => center - Axis * HalfBodyHeight;


        // They should be immutable
        public Capsule Transform(Transform t)
        {
            return new(t.TransformPoint(center), radius, height, t.rotation * rotation);
        }
        public Capsule Translate(Vector3 delta)
        {
            return new(center + delta, radius, height);
        }
        public Capsule Expand(float delta)
        {
            return new(center, radius + delta, height + delta * 2f);
        }
    }

}
