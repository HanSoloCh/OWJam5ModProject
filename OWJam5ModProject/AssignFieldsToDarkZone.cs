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
            // dz._ambientLight = transform.root.Find("Sector/AmbientLight").GetComponent<Light>();
            dz._planetaryFog = transform.root.Find("Sector/FogSphere").GetComponent<PlanetaryFogController>();
            dz.Awake(); //gotta call this again so it gets the right values instead of getting them before NH makes them
        }
    }
}
