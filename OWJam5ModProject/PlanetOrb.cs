using System;
using System.Linq;
using NewHorizons.Utility;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class PlanetOrb : MonoBehaviour
    {
        const string SUN_NAME = "Walker_Jam5_Star";

        [SerializeField] string planetName = null;
        [SerializeField] Transform center = null;
        [SerializeField] float scaleFactor = 0;
        [SerializeField] float freezeRadius = 100;
        [SerializeField] bool isGrandOrb = false;

        GameObject sun;
        OWRigidbody planetRB;
        NomaiInterfaceOrb orb;
        GameObject player;
        
        static ScreenPrompt cancelDragPrompt;
        static bool tutorialPrompt = true; // not a persistent condition because people might forget between sessions

        bool wasBeingDragged;

        void Start()
        {
            if (!isGrandOrb)
            {
                planetRB = OWJam5ModProject.Instance.NewHorizons.GetPlanet(planetName).GetComponent<OWRigidbody>();
                sun = OWJam5ModProject.Instance.NewHorizons.GetPlanet(SUN_NAME);
            }
            orb = GetComponent<NomaiInterfaceOrb>();
            player = Locator.GetPlayerBody().gameObject;

            // one prompt for all orbs
            if (cancelDragPrompt == null)
            {
                var prompt = OWJam5ModProject.Instance.NewHorizons.GetTranslationForUI("ORB_CANCEL_DRAG_PROMPT");
                cancelDragPrompt = new ScreenPrompt(InputLibrary.cancel, prompt + "   <CMD>");
                Locator.GetPromptManager().AddScreenPrompt(cancelDragPrompt, tutorialPrompt ? PromptPosition.Center : PromptPosition.UpperRight);
            }

            if (!isGrandOrb) UpdateLocation(); // do initial
        }

        private void OnDestroy()
        {
            if (cancelDragPrompt != null)
            {
                Locator.GetPromptManager().RemoveScreenPrompt(cancelDragPrompt);
                cancelDragPrompt = null;
            }
        }

        void FixedUpdate()
        {
            // radius check
            if (isGrandOrb)
                return;
            if ((player.transform.position - center.transform.position).sqrMagnitude > freezeRadius * freezeRadius)
                return;

            UpdateLocation();
        }

        private void Update()
        {
            // Update release prompt visibility
            if (wasBeingDragged && !orb._isBeingDragged)
            {
                cancelDragPrompt.SetVisibility(false);
            } 
            else if (!wasBeingDragged && orb._isBeingDragged)
            {
                cancelDragPrompt.SetVisibility(true);
            }
            wasBeingDragged = orb._isBeingDragged;
            
            // Check release button pressed
            if (orb._isBeingDragged && OWInput.IsNewlyPressed(InputLibrary.cancel))
            {
                orb._orbAudio.PlaySlotActivatedClip();
                orb.CancelDrag();

                orb._enterSlotTime = Time.time;

                if (tutorialPrompt)
                {
                    tutorialPrompt = false;
                    Locator.GetPromptManager().RemoveScreenPrompt(cancelDragPrompt);
                    Locator.GetPromptManager().AddScreenPrompt(cancelDragPrompt, PromptPosition.UpperRight);
                }
            }
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

            //Magnetic north: +global y to start
            //Towards sun: local -z
            planetRB.transform.up = -sun.transform.up;
            Vector3 toSun = sun.transform.position - planetRB.transform.position;
            float angle = Vector3.SignedAngle(-planetRB.transform.forward, toSun, planetRB.transform.up);
            planetRB.transform.Rotate(planetRB.transform.up, angle, Space.World);

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
