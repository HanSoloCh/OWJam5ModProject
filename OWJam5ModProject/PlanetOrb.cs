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

        GameObject sun;
        OWRigidbody planetRB;
        OWRigidbody rigid;

        void Start()
        {
            planetRB = OWJam5ModProject.Instance.NewHorizons.GetPlanet(planetName).GetComponent<OWRigidbody>();
            sun = OWJam5ModProject.Instance.NewHorizons.GetPlanet(SUN_NAME);
            rigid = GetComponent<OWRigidbody>();

            Vector3 relativePosition = center.InverseTransformPoint(rigid.transform.position);
            Vector3 planetTargetPosition = sun.transform.TransformPoint(relativePosition * scaleFactor);
            planetRB.SetPosition(planetTargetPosition);
        }

        void FixedUpdate()
        {
            Vector3 relativeVelocity = center.InverseTransformVector(rigid.GetVelocity());
            Vector3 planetTargetVelocity = sun.transform.TransformVector(relativeVelocity * scaleFactor);

            planetRB.SetVelocity(planetTargetVelocity);
        }
    }
}
