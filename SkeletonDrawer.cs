// (c) Simone Guggiari 2024

using System.Collections.Generic;
using UnityEngine;

////////// PURPOSE: Draws a skeleton's bones //////////

namespace sxg
{
    public class SkeletonDrawer : MonoBehaviour
    {

        // PUBLIC
        public Color color = Color.white;
        public float boneWidth = 0.1f;
        public Vector3 boneEuler;
        public float defaultBoneLength = 0.1f;

        public bool drawNames;
        public bool drawAxes;
        public bool drawLeaves;

        // PRIVATE
        List<TBone> skeleton;

        class TBone
        {
            public Transform t;
            public float length;
            public TBone parent;
        }


        // -------------------- BASE METHODS --------------------

        // -------------------- CUSTOM METHODS --------------------

        // COMMANDS
        void MakeSkeleton()
        {
            skeleton = new();
            foreach (var t in transform.GetDescendants(includeSelf: true))
            {
                float length = CalcBoneLength(t);
                TBone parent = skeleton.Find(b => b.t == t.parent);
                skeleton.Add(new() { t = t, length = length, parent = parent });
            }
        }

        void DrawDottedLine(Vector3 from, Vector3 to)
        {
            // TODO: move into gizmos2
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawDottedLine(from, to, 3f);
        }

        // QUERIES
        Vector3 Dir(Transform bone) => (bone.rotation * Quaternion.Euler(boneEuler)) * Vector3.forward;
        Vector3 Tail(Transform bone, float length) => bone.position + Dir(bone) * length;

        float CalcBoneLength(Transform t)
        {
            if (t.childCount == 0)
                return defaultBoneLength;
            if (t.childCount == 1)
                return Vector3.Distance(t.position, t.GetChild(0).position);
            else
                return Vector3.Distance(t.position, MostLikelyDirectChildBone(t).position);
        }

        Transform MostLikelyDirectChildBone(Transform t)
        {
            Debug.Assert(t.childCount > 0);
            Vector3 boneDir = Dir(t);
            return t.GetChildren().FindBest(ct =>
            {
                return Vector3.Angle(boneDir, ct.position - t.position);
            });
        }


        // OTHER
        private void OnDrawGizmos()
        {
            if (skeleton.IsNullOrEmpty())
                MakeSkeleton();
            Gizmos.color = color;
            Quaternion rot = Quaternion.Euler(boneEuler);
            foreach (var bone in skeleton)
            {
                Transform t = bone.t;
                float length = bone.length;
                if (!drawLeaves && t.childCount == 0)
                    continue;
                float width = length * boneWidth;
                Gizmos2.DrawPyramid(t.position, t.rotation * rot, length, width);
                if (drawNames)
                    Gizmos2.DrawLabel(t.name, Vector3.Lerp(t.position, Tail(t, length), 0.5f));
                if (drawAxes)
                    Gizmos2.DrawTransform(t, width/2f, color.a);
                if (bone.parent != null && Vector3.Distance(t.position, Tail(bone.parent.t, bone.parent.length)) > 1e-1f)
                    DrawDottedLine(t.position, t.parent.position);
            }
        }
    }
}