using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class FinalDoorAnimator : MonoBehaviour
    {
        public FinalDoorController controller;
        public Transform startTransform;
        public Transform endTransform;
        public float Delay;
        public float Duration = 1;

        public void LerpTransforms()
        {
            float t = Mathf.Clamp01((controller.t - Delay) / Duration);
            transform.localPosition = Vector3.Lerp(startTransform.localPosition, endTransform.localPosition, t);
            transform.localRotation = Quaternion.Lerp(startTransform.localRotation, endTransform.localRotation, t);
            transform.localScale = Vector3.Lerp(startTransform.localScale, endTransform.localScale, t);
        }

        public void OnDrawGizmos()
        {
            if (controller != null) LerpTransforms();
        }

        public void Update()
        {
            LerpTransforms();
        }
    }
}
