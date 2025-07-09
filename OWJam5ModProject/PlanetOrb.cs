using System;
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
            planetRB.transform.forward = sun.transform.up * -1;
            planetRB.transform.up = (planetRB.transform.position - sun.transform.position).normalized;
        }
    }
}
