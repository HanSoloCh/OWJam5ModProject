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
        NomaiComputer computer;
        bool playerInside;
        bool convergenceSolved;

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