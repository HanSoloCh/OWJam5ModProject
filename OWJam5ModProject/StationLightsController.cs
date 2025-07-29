using System;
using System.Collections;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class StationLightsController : MonoBehaviour
    {
        string EMISSION_COLOR_PROPERTY = "_EmissionColor";

        [SerializeField] OWTriggerVolume playerDetectorTrigger = null;
        [SerializeField] MeshRenderer[] lightEmissionRenderers;
        [SerializeField] int[] lightEmissionMaterialIndices;
        [SerializeField] float fadeDuration;

        Light[] lights;
        float[] lightsInitialIntensities;
        Color[] lightEmissionInitialColors;

        bool lightsOn;

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

            lightEmissionInitialColors = new Color[lightEmissionRenderers.Length];
            for (int i=0; i<lightEmissionRenderers.Length; i++)
            {
                lightEmissionInitialColors[i] = lightEmissionRenderers[i].materials[lightEmissionMaterialIndices[i]].GetColor(EMISSION_COLOR_PROPERTY);
                lightEmissionRenderers[i].materials[lightEmissionMaterialIndices[i]].SetColor(EMISSION_COLOR_PROPERTY, Color.black);
            }
        }

        private void PlayerDetectorTrigger_OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                if (!lightsOn)
                {
                    StartCoroutine(FadeLights());
                    lightsOn = true;
                }
            }
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

                for (int i=0; i<lightEmissionRenderers.Length; i++)
                {
                    lightEmissionRenderers[i].materials[lightEmissionMaterialIndices[i]].SetColor(EMISSION_COLOR_PROPERTY, Color.Lerp(Color.black, lightEmissionInitialColors[i], t));
                }

                t += Time.deltaTime / fadeDuration;
                yield return new WaitForEndOfFrame();
            }

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = lightsInitialIntensities[i];
            }

            for (int i = 0; i < lightEmissionRenderers.Length; i++)
            {
                lightEmissionRenderers[i].materials[lightEmissionMaterialIndices[i]].SetColor(EMISSION_COLOR_PROPERTY, lightEmissionInitialColors[i]);
            }
        }
    }
}
