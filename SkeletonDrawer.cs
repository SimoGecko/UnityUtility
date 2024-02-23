// (c) Simone Guggiari 2024

using System.Collections.Generic;
using UnityEngine;

////////// PURPOSE: Draws a skeleton's bones //////////

namespace sxg
{
    public class SkeletonDrawer : MonoBehaviour
    {

        // public
        public Color color = Color.white;
        public float boneWidth = 0.1f;
        public Vector3 boneEuler;
        public float defaultBoneLength = 0.1f;

        // private


        // other
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Quaternion rot = Quaternion.Euler(boneEuler);
            transform.ForeachDescendant(t =>
            {
                float length = t.childCount >= 1 ? Vector3.Distance(t.position, t.GetChild(0).position) : defaultBoneLength;
                float width = length * boneWidth;
                Gizmos2.DrawPyramid(t.position, t.rotation * rot, length, width);
            }, true);
        }

    }
}