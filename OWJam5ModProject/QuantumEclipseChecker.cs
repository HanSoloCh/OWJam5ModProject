using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class QuantumEclipseChecker : MonoBehaviour
    {
        [SerializeField] private Transform[] rayOrigins = null;
        [SerializeField] private Light light = null;

        private float lightIntensity = 0;

        /**
         * Grab the initial info on the light
         */
        private void Start()
        {
            lightIntensity = light.intensity;
        }

        /**
         * Do a bunch of raycasts to see if the sun is fully gone
         */
        private void FixedUpdate()
        {
            int hitCount = 0;

            //Do one cast for each origin point
            foreach (Transform origin in rayOrigins)
            {
                //Do the actual raycast
                float distToStar = (origin.position - OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star").transform.position).magnitude;
                Ray ray = new Ray(origin.position, origin.up);
                RaycastHit[] hits = Physics.RaycastAll(ray, distToStar);

                //See if any hit is correct
                foreach (RaycastHit hit in hits)
                {
                    //OWJam5ModProject.DebugLog(hit.collider.name);
                    if (hit.collider.name.Equals("ice_raycast_detector"))
                    {
                        hitCount++;
                    }
                }
            }

            //If it's the right number, turn off the light
            if (hitCount >= rayOrigins.Length)
                light.intensity = 0;

            //Otherwise, turn it on
            else
                light.intensity = lightIntensity;
        }
    }
}
