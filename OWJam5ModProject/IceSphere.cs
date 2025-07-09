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
        private float growSpeed = 2;
        private Transform waterTF = null;
        [NonSerialized] public bool isFreezing = false;

        private bool shouldShadeFreeze = true, shouldDistanceFreeze = true;
        private float iceFreezeDistance = 2000;

        /**
         * Find the water sphere
         */
        private void Start()
        {
            waterTF = transform.root.Find("Sector/Water");
        }

        /**
         * If the big planet is in front of this, grow the ice. Otherwise shrink it
         */
        private void FixedUpdate()
        {
            //Do a raycast towards the sun, see if it hits the other planet
            isFreezing = false;
            var planetToStar = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star").transform.position - transform.position;
            if (shouldShadeFreeze)
            {
                Ray ray = new Ray(transform.position, planetToStar);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                foreach (RaycastHit hit in hits)
                {
                    //OWJam5ModProject.DebugLog(hit.collider.name);
                    if (hit.collider.name.Equals("ice_raycast_detector"))
                    {
                        isFreezing = true;
                    }
                }
            }
            if (shouldDistanceFreeze)
                isFreezing &= planetToStar.magnitude > iceFreezeDistance; // also freeze if far away enough

            //If we're growing the ice, grow to a bit past the water
            if (isFreezing) 
            { 
                float scale = Mathf.Clamp(transform.localScale.x + growSpeed * Time.deltaTime,
                    waterTF.localScale.x - 10, waterTF.localScale.x + 5);
                transform.localScale = new Vector3(scale, scale, scale);
            }

            //Otherwise, shrink down to the lowest water level
            else
            {

                float scale = Mathf.Clamp(transform.localScale.x - growSpeed * Time.deltaTime,
                    waterTF.localScale.x - 10, waterTF.localScale.x + 5);
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
