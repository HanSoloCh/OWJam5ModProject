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
        float HARMONIC_THRESHOLD = 0.7f;

        [SerializeField] string[] signalNames;
        [SerializeField] Light[] detectorLights;
        [SerializeField] MeshRenderer[] detectorLightRenderers;
        [SerializeField] GameObject computerParent;
        [SerializeField] OWTriggerVolume playerDetectorTrigger;

        AudioSignal[] harmonicSignals;
        bool playerInside;

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

            // Initialize detector
            playerDetectorTrigger.OnEntry += PlayerDetector_OnEntry;
            playerDetectorTrigger.OnExit += PlayerDetector_OnExit;
        }

        void Update()
        {
            if (playerInside)
            {
                if (CheckConvergence())
                    OWJam5ModProject.Instance.ModHelper.Console.WriteLine("Converged!!!");
            }
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
            foreach (AudioSignal signal in harmonicSignals)
            {
                if (signal.GetSignalStrength() < HARMONIC_THRESHOLD)
                    return false;
            }

            return true;
        }
    }
}