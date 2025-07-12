using NewHorizons.Components.SizeControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public static class FinalRequirementManager
    {
        /**
         * Five requirements:
         * 
         * 1. Large planet has a max-size ice sphere
         * 2. Water planet has minimum water
         * 3. At least one receiver is lined up
         * 4. Two different planets need to have sand
         * 5. Any planets in the same orbit are on opposite sides of the sun
         */

        public static bool inJamSystem = false;

        //Some handy constants
        private const float CLOSE_ORBIT_DIST = 1200f;
        private const float FAR_ORBIT_DIST = 2000f;
        private const float PLANET_PROXIMITY_ANGLE = 150f;

        //Tracking the various bodies and such (needed for #5)
        private static Transform starTF = null;
        private static Transform[] planetTFs = null; //Crystal, big, sand, water

        //Track specific components and metrics needed for the requirements
        private static Transform bigPlanetIce = null;       //#1
        private static Transform icePlanetWater = null;     //#2
        public static bool padLinedUp = false;                //#3
        private static Transform sandPlanetSand = null;     //#4
        
        /**
         * Initialize everything that we need
         */
        public static void Initialize()
        {
            padLinedUp = false;

            //Find all of the bodies of the mod
            starTF = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star").transform;
            planetTFs = new Transform[4];
            planetTFs[0] = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Planet1").transform;
            planetTFs[1] = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Planet2").transform;
            planetTFs[2] = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Planet3").transform;
            planetTFs[3] = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Planet4").transform;

            //Grab a whole slew of components
            bigPlanetIce = planetTFs[1].GetComponentInChildren<IceSphere>().transform;
            icePlanetWater = planetTFs[3].GetComponentInChildren<WaterSizeController>().transform;
            sandPlanetSand = planetTFs[2].Find("Sector/Sand");
        }

        /**
         * Checks whether or not the large planet has enough ice
         */
        public static bool CheckIceReq()
        {
            if (!inJamSystem || bigPlanetIce == null)
                return false;

            return bigPlanetIce.Find("outer_ice").localScale.x > OWJam5ModProject.WATER_FILLED_HEIGHT + 4;
        }

        /**
         * Checks whether or not the water planet has low enough water
         */
        public static bool CheckDryReq()
        {
            if(!inJamSystem || icePlanetWater == null) 
                return false;

            return icePlanetWater.localScale.x < OWJam5ModProject.WATER_DRAINED_HEIGHT + 1;
        }

        /**
         * Checks whether or not at least one warp pad is lined up
         */
        public static bool CheckWarpReq()
        {
            if (!inJamSystem)
                return false;

            return padLinedUp;
        }

        /**
         * Checks whether or not enough sand is on the two viable planets
         */
        public static bool CheckSandReq()
        {
            //Technically we only need to check one, since the sand can only be in one other place
            if(!inJamSystem || sandPlanetSand == null)
                return false;

            //Sand starts at 306
            //Ends at 260
            float sandScale = sandPlanetSand.localScale.x;
            return sandScale < 305 && sandScale > 261;

        }

        /**
         * Checks whether or not all planets in the same orbit are far enough away
         */
        public static bool CheckAngleReq()
        {
            if (!inJamSystem)
                return false;

            //Sort the bodies into different orbit levels
            //(0 is close, 1 is mid, 2 is far)
            List<Transform>[] orbits = {new List<Transform>(), new List<Transform>(), new List<Transform>()};
            foreach(Transform planet in planetTFs)
            {
                float starDist = (planet.position - starTF.position).magnitude;
                if(starDist < CLOSE_ORBIT_DIST)
                    orbits[0].Add(planet);
                else if(starDist > FAR_ORBIT_DIST)
                    orbits[1].Add(planet);
                else
                    orbits[2].Add(planet);
            }

            //Go through each orbit level and see if any planets are too close
            foreach(List<Transform> orbit in orbits)
            {
                for(int i = 0; i < orbit.Count; i++)
                {
                    for(int j = i + 1; j < orbit.Count; j++)
                    {
                        Vector3 toPlanet1 = orbit[i].position - starTF.position;
                        Vector3 toPlanet2 = orbit[j].position - starTF.position;
                        if (Vector3.Angle(toPlanet1, toPlanet2) < PLANET_PROXIMITY_ANGLE)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
