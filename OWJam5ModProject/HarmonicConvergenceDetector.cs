using HarmonyLib;
using NewHorizons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class HarmonicConvergenceDetector : MonoBehaviour
    {
        string HARMONIC_FREQUENCY = "Walker_Jam5_Harmonic";
        float HARMONIC_THRESHOLD = 0.85f;
        string EMISSION_COLOR_PROPERTY = "_EmissionColor";
        float TORCH_FADE_DURATION = 0.25f;

        [SerializeField] string[] signalNames;
        [SerializeField] Light[] detectorLights;
        [SerializeField] MeshRenderer[] detectorLightRenderers;
        [SerializeField] OWAudioSource[] detectorLightPowerOnAudioSources;
        [SerializeField] OWAudioSource[] detectorLightPowerOffAudioSources;
        [SerializeField] GameObject computerParent;
        [SerializeField] OWTriggerVolume playerDetectorTrigger;

        AudioSignal[] harmonicSignals;
        NomaiComputer computer;
        bool playerInside;
        bool convergenceSolved;
        bool[] torchesActive;
        float[] torchFadeValues;
        float torchBrightness;
        Color torchColor;

        void Start()
        {
            // Get signals
            List<AudioSignal> audioSignals = Locator.GetAudioSignals();
            harmonicSignals = new AudioSignal[signalNames.Length];
            foreach (AudioSignal signal in audioSignals)
            {
                if (signal.GetFrequency().ToString() == HARMONIC_FREQUENCY)
                {
                    for (int i=0; i<signalNames.Length; i++)
                    {
                        if (signal.GetName().ToString() == signalNames[i])
                        {
                            harmonicSignals[i] = signal;
                            signal.GetComponent<AudioSignalDetectionTrigger>()._allowUnderwater = true;
                            break;
                        }
                    }
                }
            }

            // Initialize player detector trigger
            playerDetectorTrigger.OnEntry += PlayerDetector_OnEntry;
            playerDetectorTrigger.OnExit += PlayerDetector_OnExit;

            // Initialize computer
            NewHorizons.Utility.OWML.Delay.FireOnNextUpdate(InitializeComputer);

            // Initialize torches
            torchesActive = new bool[detectorLights.Length];
            
            torchFadeValues = new float[detectorLights.Length];
            for (int i = 0; i < torchFadeValues.Length; i++)
                torchFadeValues[i] = 1;

            torchBrightness = detectorLights[0].intensity;
            torchColor = detectorLightRenderers[0].material.GetColor(EMISSION_COLOR_PROPERTY);
        }

        void InitializeComputer()
        {
            computer = computerParent.GetComponentInChildren<NomaiComputer>();
            computer.ClearAllEntries(); // Computer starts deactivated
        }

        void Update()
        {
            if (playerInside && !convergenceSolved)
            {
                if (CheckConvergence())
                {
                    convergenceSolved = true;
                    computer.DisplayAllEntries();
                }
            }

            UpdateTorches();
        }

        void OnDestroy()
        {
            playerDetectorTrigger.OnEntry -= PlayerDetector_OnEntry;
            playerDetectorTrigger.OnExit -= PlayerDetector_OnExit;
        }

        private void PlayerDetector_OnEntry(GameObject hitObj)
        {
            //OWJam5ModProject.Instance.ModHelper.Console.WriteLine(hitObj.ToString() + " entered");
            //OWJam5ModProject.Instance.ModHelper.Console.WriteLine(Locator.GetPlayerDetector().ToString() + " compared");
            if (hitObj == Locator.GetPlayerDetector())
                playerInside = true;
        }

        private void PlayerDetector_OnExit(GameObject hitObj)
        {
            if (hitObj == Locator.GetPlayerDetector())
                playerInside = false;
        }

        bool CheckConvergence()
        {
            bool result = true;

            for (int i=0; i<harmonicSignals.Length; i++)
            {
                bool signalPlaying = harmonicSignals[i].GetSignalStrength() >= HARMONIC_THRESHOLD;

                if (torchesActive[i] != signalPlaying)
                {
                    if (signalPlaying)
                        detectorLightPowerOnAudioSources[i].Play();
                    else
                        detectorLightPowerOffAudioSources[i].Play();
                }

                torchesActive[i] = signalPlaying;

                if (signalPlaying == false)
                    result = false;
            }

            return result;
        }

        void UpdateTorches()
        {
            for (int i=0; i<torchesActive.Length; i++)
            {
                float targetFadeValue = torchesActive[i] ? 1 : 0;
                float newFadeValue = Mathf.MoveTowards(torchFadeValues[i], targetFadeValue, Time.deltaTime / TORCH_FADE_DURATION);

                if (newFadeValue != torchFadeValues[i])
                {
                    torchFadeValues[i] = newFadeValue;
                    detectorLightRenderers[i].material.SetColor(EMISSION_COLOR_PROPERTY, Color.Lerp(Color.black, torchColor, torchFadeValues[i]));
                    detectorLights[i].intensity = Mathf.Lerp(0, torchBrightness, torchFadeValues[i]);
                }
            }
        }
    }
}