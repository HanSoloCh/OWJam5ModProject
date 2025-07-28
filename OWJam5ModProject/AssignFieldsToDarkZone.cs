using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class AssignFieldsToDarkZone : MonoBehaviour
    {
        public DarkZone dz;

        public void Start()
        {
            if (!dz._ambientLight)
            {
                Transform ambientLightTransform = transform.root.Find("Sector/AmbientLight");
                if (ambientLightTransform != null)
                    dz._ambientLight = ambientLightTransform.GetComponent<Light>();
            }

            if (!dz._planetaryFog) 
            {
                Transform planetaryFogTransform = transform.root.Find("Sector/FogSphere");
                if (planetaryFogTransform != null)
                    dz._planetaryFog = planetaryFogTransform.GetComponent<PlanetaryFogController>();
            }
            
            dz.Awake(); //gotta call this again so it gets the right values instead of getting them before NH makes them
        }
    }
}
