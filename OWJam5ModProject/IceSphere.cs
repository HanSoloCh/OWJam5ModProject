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
        [SerializeField] float iceGrowAmount = 5; //configurable grow amount -jamie
        [SerializeField] float minIceHeight = 0;
        private float growSpeed = 4;
        private Transform waterTF = null;
        private Transform innerTF = null;
        private Transform outerTF = null;
        private float iceFreezeDistance = 2000;
        private float iceMeltDistance = 1200;
        private float waterInnerIceOffset = -0.5f;

        [SerializeField] Light light;
        [SerializeField] bool startsFrozen;

        private Material waterMaterial;
        private Color waterTint;

        /**
         * Find the water sphere
         */
        private void Start()
        {
            waterTF = transform.root.Find("Sector/Water");
            innerTF = transform.Find("inner_ice");
            outerTF = transform.Find("outer_ice");

            waterMaterial = waterTF.Find("OceanFog").GetComponent<MeshRenderer>().material;
            waterTint = waterMaterial.color;
            if (startsFrozen)
                waterMaterial.color = Color.black;

            //This being negative will make the ice always drainable
            iceGrowAmount = Mathf.Max(0, iceGrowAmount);
        }

        /**
         * Get whether or no it's too frozen to drain water
         */
        public bool CanDrain()
        {
            return waterTF.localScale.x > outerTF.localScale.x;
        }

        /**
         * If the big planet is in front of this, grow the ice. Otherwise shrink it
         */
        private void FixedUpdate()
        {
            //If the water seems really low, that means we're bugged and shouldn't do anything with the ice this frame
            if (waterTF.localScale.x < 10)
                return;

            //Do a raycast towards the sun, see if it hits the other planet
            bool isShadowed = false;
            bool isCold = false;
            bool isHot = false;
            var planetToStar = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star").transform.position - transform.position;
            Ray ray = new Ray(transform.position, planetToStar.normalized);
            RaycastHit[] hits = Physics.RaycastAll(ray, planetToStar.magnitude);
            foreach (RaycastHit hit in hits)
            {
                //OWJam5ModProject.DebugLog(hit.collider.name);
                if (hit.collider.name.Equals("ice_raycast_detector"))
                {
                    isShadowed = true;
                }
            }

            //See where the planet is relative to the star, and what that means for it freezing
            isCold = planetToStar.magnitude > iceFreezeDistance;
            isHot = planetToStar.magnitude < iceMeltDistance;

            //If far enough and shadowed, should grow a bit past the water
            float scale = outerTF.localScale.x;
            if (isCold && isShadowed) 
            {
                scale = scale + growSpeed * Time.deltaTime;
                if (light != null)
                    light.intensity = 0;
                waterMaterial.color = Color.black;
            }

            //If hot and unshadowed, ice should shrink
            else if(isHot && !isShadowed)
            {
                scale = scale - growSpeed * Time.deltaTime;
                if (light != null)
                    light.intensity = 1;
                waterMaterial.color = waterTint;
            }

            //Make sure that the scale is in bounds 
            scale = Mathf.Clamp(scale, waterTF.localScale.x - 36, waterTF.localScale.x + iceGrowAmount);
            scale = Mathf.Max(scale, minIceHeight);
            outerTF.localScale = new Vector3(scale, scale, scale);
            float innerScale = Mathf.Min(waterTF.localScale.x + waterInnerIceOffset, scale);
            innerTF.localScale = new Vector3(innerScale, innerScale, innerScale);
        }
    }
}
