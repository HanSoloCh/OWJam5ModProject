using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OWJam5ModProject;

[RequireComponent(typeof(NomaiWarpTransmitter))]
public class NomaiWarpTransmitterSwapper : MonoBehaviour
{
    private static List<NomaiWarpReceiver> _receivers;

    public static void Apply()
    {
        // kinda evil
        // doesnt get station if we put one there
        var transmitters = FindObjectsOfType<NomaiWarpTransmitter>()
            .Where(x => x.GetAttachedOWRigidbody().name.StartsWith("Walker_Jam5_Planet"))
            .ToList();
        _receivers = FindObjectsOfType<NomaiWarpReceiver>()
            .Where(x => x.GetAttachedOWRigidbody().name.StartsWith("Walker_Jam5_Planet"))
            .ToList();

        foreach (var transmitter in transmitters)
        {
            transmitter.gameObject.AddComponent<NomaiWarpTransmitterSwapper>();
        }
    }


    private NomaiWarpTransmitter _transmitter;

    private void Start()
    {
        _transmitter = GetComponent<NomaiWarpTransmitter>();
    }

    public void FixedUpdate()
    {
        // just run transmitter finding code on all the targets until we found one that matches :P
        foreach (var receiver in _receivers)
        {
            _transmitter._targetReceiver = receiver; // try this receiver

            if (_transmitter._objectsOnPlatform.Count > 0 && !_transmitter.IsBlackHoleOpen() &&
                _transmitter._targetReceiver != null && _transmitter._targetReceiver.gameObject.activeInHierarchy)
            {
                float viewAngleToTarget = _transmitter.GetViewAngleToTarget();
                float num = 0.5f * ((TimeLoop.GetSecondsRemaining() < 30f) ? 5f : _transmitter._alignmentWindow);
                if (viewAngleToTarget <= num)
                {
                    return; // we found the one we will use
                }
            }
        }
    }
}