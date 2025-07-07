using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class IceSphere : MonoBehaviour
    {
        private float growSpeed = 5;
        private Transform waterTF = null;
        public bool isFreezing = false;

        /**
         * Find the water sphere
         */
        private void Awake()
        {
            waterTF = transform.parent.Find("Water");
        }

        /**
         * If the big planet is in front of this, grow the ice. Otherwise shrink it
         */
        private void FixedUpdate()
        {
            //Do a raycast towards the sun, see if it hits the other planet
            isFreezing = false;
            Ray ray = new Ray(transform.position, 
                OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star").transform.position - transform.position);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach(RaycastHit hit in hits)
            {
                OWJam5ModProject.DebugLog(hit.collider.name);
                if (hit.collider.name.Equals("ice_raycast_detector"))
                {
                    isFreezing = true;
                    OWJam5ModProject.DebugLog("Ice raycast hit!");
                }
            }

            //If we're growing the ice, grow to a bit past the water
            if (isFreezing) 
            { 
                float scale = Mathf.Clamp(transform.localScale.x + growSpeed * Time.deltaTime, 
                    OWJam5ModProject.WATER_DRAINED_HEIGHT, waterTF.localScale.x + 3);
                transform.localScale = new Vector3(scale, scale, scale);
            }

            //Otherwise, shrink down to the lowest water level
            else
            {

                float scale = Mathf.Clamp(transform.localScale.x - growSpeed * Time.deltaTime,
                    OWJam5ModProject.WATER_DRAINED_HEIGHT, waterTF.localScale.x + 3);
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
