// (c) Simone Guggiari 2024

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

////////// PURPOSE: Draws a skeleton's bones //////////

namespace sxg
{
    public class SkeletonDrawer : MonoBehaviour
    {

        // PUBLIC
        public Color color = new(1f, 1f, 1f, 0.5f);
        public float boneWidth = 0.1f;
        public Vector3 boneEuler = new(-90f, 0f, 0f);

        public bool drawNames = false;
        public bool drawAxes = true;
        public bool drawLeaves = true;

        // PRIVATE
        List<TBone> skeleton;

        class TBone
        {
            public Transform t;
            public float length;
            public TBone parent;
            public Quaternion rotation;

            public TBone(Transform t, TBone parent, Quaternion rotation)
            {
                this.t = t;
                this.parent = parent;
                this.rotation = rotation;
                this.length = CalcBoneLength();
                bool flip = t.GetAncestors(includeSelf: true).Any(t => t.gameObject.tag == "BoneFlip");
                if (flip)
                    this.rotation *= Quaternion.Euler(180f, 0f, 0f);
            }

            public Vector3 Dir => (t.rotation * rotation) * Vector3.forward;
            public Vector3 Tail => t.position + Dir * length;
            public Quaternion Rot => t.rotation * rotation;

            private float CalcBoneLength()
            {
                float defaultBoneLength = 0.1f; // TODO: param?

                if (t.childCount == 0)
                    return parent != null ? parent.length * 0.8f : defaultBoneLength;
                if (t.childCount == 1)
                    return Vector3.Distance(t.position, t.GetChild(0).position);
                else
                    return Vector3.Distance(t.position, MostLikelyDirectChildBone().position);
            }

            private Transform MostLikelyDirectChildBone()
            {
                Debug.Assert(t.childCount > 0);
                Vector3 boneDir = Dir;
                return t.GetChildren().FindBest(ct =>
                {
                    return Vector3.Angle(boneDir, ct.position - t.position);
                });
            }
        }


        // -------------------- BASE METHODS --------------------

        // -------------------- CUSTOM METHODS --------------------

        // COMMANDS
        [EditorButton]
        void EDITOR_MakeSkeleton() => MakeSkeleton();

        void MakeSkeleton()
        {
            skeleton = new();
            Quaternion rot = Quaternion.Euler(boneEuler);
            foreach (var t in transform.GetDescendants(includeSelf: true))
            {
                TBone parent = skeleton.Find(b => b.t == t.parent);
                skeleton.Add(new(t, parent, rot));
                //float length = CalcBoneLength(t);
            }
        }

        void DrawDottedLine(Vector3 from, Vector3 to)
        {
            // TODO: move into gizmos2
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawDottedLine(from, to, 3f);
        }

        // QUERIES


        // OTHER
        private void OnDrawGizmos()
        {
            if (skeleton.IsNullOrEmpty())
                MakeSkeleton();
            Gizmos.color = color;
            foreach (var bone in skeleton)
            {
                Transform t = bone.t;
                float length = bone.length;
                float width = length * boneWidth;
                if (drawLeaves || t.childCount > 0)
                    Gizmos2.DrawPyramid(t.position, bone.Rot, length, width);
                if (drawNames)
                    Gizmos2.DrawLabel(t.name, Vector3.Lerp(t.position, bone.Tail, 0.5f));
                if (drawAxes)
                    Gizmos2.DrawTransform(t, width/2f, color.a);
                if (bone.parent != null && Vector3.Distance(t.position, bone.parent.Tail) > 0.01f)
                    DrawDottedLine(t.position, t.parent.position);
            }
        }
    }
}