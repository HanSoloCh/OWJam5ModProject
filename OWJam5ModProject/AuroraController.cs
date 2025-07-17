using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class AuroraController : MonoBehaviour
    {
        public MeshRenderer render;
        public Transform scaleRead;
        public float defaultLocalScale;

        public void Update()
        {
            render.material.SetFloat("_Intensity", (scaleRead.localScale.x - defaultLocalScale) / 36f);
        }
    }
}
