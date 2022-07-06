// (c) Simone Guggiari 2020-2022

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
    
}
