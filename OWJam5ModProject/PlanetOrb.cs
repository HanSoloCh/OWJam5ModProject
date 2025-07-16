using System;
using System.Linq;
using NewHorizons.Utility;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class PlanetOrb : MonoBehaviour
    {
        const string SUN_NAME = "Walker_Jam5_Star";

        [SerializeField] string planetName;
        [SerializeField] Transform center;
        [SerializeField] float scaleFactor;
        [SerializeField] float freezeRadius = 100;

        GameObject sun;
        OWRigidbody planetRB;
        NomaiInterfaceOrb orb;
        GameObject player;
        
        void Start()
        {
            planetRB = OWJam5ModProject.Instance.NewHorizons.GetPlanet(planetName).GetComponent<OWRigidbody>();
            sun = OWJam5ModProject.Instance.NewHorizons.GetPlanet(SUN_NAME);
            orb = GetComponent<NomaiInterfaceOrb>();
            player = Locator.GetPlayerBody().gameObject;

            UpdateLocation(); // do initial
        }

        void FixedUpdate()
        {
            // radius check
            if ((player.transform.position - center.transform.position).sqrMagnitude > freezeRadius * freezeRadius)
                return;

            UpdateLocation();
        }

        private void UpdateLocation()
        {
            // qsb does the same thing for occasional sync :D
            var relShipPos = planetRB.transform.InverseTransformPoint(Locator.GetShipTransform().position);
            var relShipRot = planetRB.transform.InverseTransformRotation(Locator.GetShipTransform().rotation);

            /*
            Vector3 relativeVelocity = center.InverseTransformVector(orb._orbBody.GetVelocity());
            relativeVelocity.y = 0;
            Vector3 planetTargetVelocity = sun.transform.TransformVector(relativeVelocity * scaleFactor);
            planetRB.SetVelocity(planetTargetVelocity);
            */

            Vector3 relativePosition = center.InverseTransformPoint(orb._orbBody.GetPosition());
            relativePosition.y = 0;
            Vector3 planetTargetPosition = sun.transform.TransformPoint(relativePosition * scaleFactor);
            planetRB.SetPosition(planetTargetPosition);

            //North pole : -z
            //To sun : -y
            //Don't ask me how exactly this math works, god I hate rotating things
            planetRB.transform.forward = sun.transform.up;
            float angle = Vector3.SignedAngle(-planetRB.transform.up, 
                (sun.transform.position - planetRB.transform.position).normalized, planetRB.transform.forward);
            planetRB.transform.Rotate(planetRB.transform.forward, angle);

            // move ship if landed on planet
            var shipSectorDetector = Locator.GetShipDetector().GetComponent<SectorDetector>();
            var inPlanetSector = shipSectorDetector._sectorList.Any(x => x.GetAttachedOWRigidbody() == planetRB);
            var touchedPlanetLast = ShipContactSensor.lastContact == planetRB;
            if (inPlanetSector && touchedPlanetLast)
            {
                Locator.GetShipTransform().position = planetRB.transform.TransformPoint(relShipPos);
                Locator.GetShipTransform().rotation = planetRB.transform.TransformRotation(relShipRot);
            }
        }
    }
}
