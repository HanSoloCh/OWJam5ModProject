using System;
using System.Collections;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class StationLightsController : MonoBehaviour
    {
        [SerializeField] OWTriggerVolume playerDetectorTrigger = null;
        [SerializeField] float fadeDuration;

        Light[] lights;
        float[] lightsInitialIntensities;

        void Start()
        {
            playerDetectorTrigger.OnEntry += PlayerDetectorTrigger_OnEntry;

            lights = GetComponentsInChildren<Light>();

            lightsInitialIntensities = new float[lights.Length];
            for (int i=0; i<lights.Length; i++)
            {
                lightsInitialIntensities[i] = lights[i].intensity;
                lights[i].intensity = 0;
            }
        }

        private void PlayerDetectorTrigger_OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
                StartCoroutine(FadeLights());
        }

        IEnumerator FadeLights()
        {
            float t = 0;
            while (t < 1)
            {
                for (int i=0; i<lights.Length; i++)
                {
                    lights[i].intensity = Mathf.Lerp(0, lightsInitialIntensities[i], t);
                }

                t += Time.deltaTime / fadeDuration;
                yield return new WaitForEndOfFrame();
            }

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = lightsInitialIntensities[i];
            }
        }
    }
}
