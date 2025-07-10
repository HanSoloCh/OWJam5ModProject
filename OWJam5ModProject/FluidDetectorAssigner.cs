using System;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class FluidDetectorAssigner : MonoBehaviour
    {
        void Start()
        {
            ConstantFluidDetector fluidDetector = GetComponent<ConstantFluidDetector>();
            FluidVolume fluidVolume = transform.root.Find("Sector/Water").GetComponentInChildren<FluidVolume>();
            fluidDetector._onlyDetectableFluid = fluidVolume;
        }
    }
}
