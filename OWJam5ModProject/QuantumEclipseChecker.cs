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
        [SerializeField] private GameObject[] disableOnEclipse = null;
        [SerializeField] private Light quantumLight = null;

        private bool objectsActive = true;
        private float lightIntensity = 0;

        /**
         * Grab the light intensity on start
         */
        private void Start()
        {
            lightIntensity = quantumLight.intensity;
        }

        /**
         * Toggles all of the disableable objects
         */
        private void ToggleObjects(bool active)
        {
            if (active == objectsActive)
                return;

            objectsActive = active;
            foreach(GameObject obj in disableOnEclipse)
                obj.SetActive(active);

            if(active)
                quantumLight.intensity = lightIntensity;
            else
                quantumLight.intensity = 0;
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
                ToggleObjects(false);

            //Otherwise, turn it on
            else
                ToggleObjects(true);
        }
    }
}
