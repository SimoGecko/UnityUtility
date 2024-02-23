// (c) Simone Guggiari 2020-2024

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

////////// PURPOSE: Path class //////////

namespace sxg
{
    public class Path
    {
        public event System.Action OnPathChanged;

        private List<Vector2> points;
        private float length;
        private Rect bounds;

        // -------------------- CUSTOM METHODS --------------------

        public Path()
        {
            points = new List<Vector2>();
            length = 0f;
            bounds = Rect.zero;
        }
        public Path(List<Vector2> points)
        {
            this.points = new List<Vector2>(points);
            PointsModified();
        }
        public Path(Path other)
        {
            this.points = new List<Vector2>(other.points);
            this.length = other.length;
            this.bounds = other.bounds;
        }

        public void SetVertices(Vector2[] vertices)
        {
            points = vertices.ToList();
            PointsModified();
        }

        public void Clear()
        {
            points.Clear();
            length = 0f;
            bounds = Rect.zero;
        }
        public void Add(Vector2 point)
        {
            points.Add(point);
            if (points.Count >= 2)
            {
                length += Vector2.Distance(points.Get(-2), points.Get(-1));
            }
            if(points.Count == 1)
            {
                bounds = new Rect(point, Vector2.zero);
            }
            else
            {
                bounds = bounds.ExpandToInclude(point);
            }
        }
        public void RemoveAt(int index)
        {
            points.RemoveAt(index);
            PointsModified();
        }
        public void Reverse()
        {
            points.Reverse();
            PointsModified();
        }

        private void PointsModified()
        {
            ComputeLength();
            ComputeBounds();
            OnPathChanged?.Invoke();
        }

        public int N { get { return points.IsNullOrEmpty() ? 0 : points.Count; } }
        public int Count { get { return N; } }
        public Vector2 Get(int index) { return points.Get(index); }
        public bool IsNullOrEmpty() { return points.IsNullOrEmpty(); }
        public float Length { get { return length; } }
        public Rect Bounds { get { return bounds; } }
        public List<Vector2> Vertices { get { return points; } }
        public List<Vector2> GetRange(int first, int last)
        {
            return points.GetRange(first, last);
        }
        public bool IsCW()
        {
            float anglesum = 0f;
            for (int i = 0; i < N; i++)
            {
                Vector2 p0 = points[i];
                Vector2 p1 = points[(i + 1) % N];
                Vector2 p2 = points[(i + 2) % N];
                anglesum += Vector2.SignedAngle(p1 - p0, p2 - p1);
            }
            return anglesum < 0f;
        }

        public bool Intersects(Segment segment)
        {
            if (!bounds.Contains(segment.a) && !bounds.Contains(segment.b)) return false; // early out

            for (int i = 0; i < points.Count - 1; i++)
            {
                Segment piece = new(points[i], points[i + 1]);
                if (segment.Intersects(piece))
                {
                    return true;
                }
            }
            return false;
        }
        public bool Intersects(Segment segment, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            if (!bounds.Contains(segment.a) && !bounds.Contains(segment.b)) return false; // early out

            for (int i = 0; i < points.Count - 1; i++)
            {
                Segment piece = new(points[i], points[i + 1]);
                if (segment.Intersects(piece))
                {
                    intersection = segment.FindIntersection(piece);
                    return true;
                }
            }
            return false;
        }

        public float DistanceToPoint(Vector2 point, int numLastSegmentsIgnored = 0)
        {
            float minSqrDist = float.MaxValue;
            Debug.Assert(points.Count >= 2, "Calling distance to a path that doesn't contain segments");
            for (int i = 0; i < points.Count-1-numLastSegmentsIgnored; i++)
            {
                Segment s = new(points[i], points[i + 1]);
                minSqrDist = Mathf.Min(minSqrDist, s.DistanceToPointSquared(point));
            }
            return Mathf.Sqrt(minSqrDist);
        }

        public Vector2 this[int index] { get { return points[index]; } }

        private void ComputeLength()
        {
            length = 0f;
            if (!points.IsNullOrEmpty())
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    length += Vector2.Distance(points[i], points[i + 1]);
                }
            }
        }
        private void ComputeBounds()
        {
            bounds = new Rect(points[0], Vector2.zero);
            for (int i = 1; i < points.Count; i++)
            {
                bounds = bounds.ExpandToInclude(points[i]);
            }
        }

        public void OnDrawGizmos()
        {
            // draw bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            // draw path
            if (points != null)
            {
                // draw line
                Gizmos.color = Color.black;
                for (int i = 0; i < points.Count-1; i++)
                {
                    Gizmos.DrawLine(points[i], points[i + 1]);
                }
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos2.DrawLabel(i.ToString(), points[i]);
                    Gizmos.DrawSphere(points[i], 0.05f);
                }
            }

            // draw info


        }
    }
}
