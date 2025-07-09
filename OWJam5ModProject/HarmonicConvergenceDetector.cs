using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class HarmonicConvergenceDetector : MonoBehaviour
    {
        string HARMONIC_FREQUENCY = "Walker_Jam5_Harmonic";
        float HARMONIC_THRESHOLD = 0.7f;

        List<AudioSignal> harmonicSignals;

        void Start()
        {
            List<AudioSignal> audioSignals = Locator.GetAudioSignals();
            harmonicSignals = new List<AudioSignal>();
            foreach (AudioSignal signal in audioSignals)
            {
                if (signal.GetFrequency().ToString() == HARMONIC_FREQUENCY)
                    harmonicSignals.Add(signal);
            }
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