using System.Collections.Generic;
using System.Linq;
using NewHorizons.Components;
using OWML.Utils;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// allows transmitter to have multiple receivers of matching frequency
/// </summary>
[RequireComponent(typeof(NomaiWarpTransmitter))]
public class NomaiWarpTransmitterSwapper : MonoBehaviour
{
    public static void Apply()
    {
        // find the one transmitter
        var transmitter = FindObjectsOfType<NomaiWarpTransmitter>()
            .Single(x => x._frequency == EnumUtils.Parse<NomaiWarpPlatform.Frequency>("WalkerJam5"));
        transmitter.gameObject.AddComponent<NomaiWarpTransmitterSwapper>();
    }

    private List<NomaiWarpReceiver> _receivers;
    private NomaiWarpTransmitterCooldown _cooldown;
    private NomaiWarpTransmitter _transmitter;

    private void Start()
    {
        _transmitter = GetComponent<NomaiWarpTransmitter>();
        _cooldown = GetComponent<NomaiWarpTransmitterCooldown>();
        // kinda evil
        // doesnt get station if we put one there
        _receivers = FindObjectsOfType<NomaiWarpReceiver>()
            .Where(x => x.GetAttachedOWRigidbody().name.StartsWith("Walker_Jam5_Planet"))
            .Where(x => x._frequency == _transmitter._frequency)
            .ToList();
    }

    public void FixedUpdate()
    {
        FinalRequirementManager.padLinedUp = false;
        if (_cooldown.GetValue<bool>("_cooldownActive")) return; // dont touch anything if its cooling down
        
        // just run transmitter finding code on all the targets until we found one that matches :P
        foreach (var receiver in _receivers)
        {
            _transmitter._targetReceiver = receiver; // try this receiver
            
            float viewAngleToTarget = _transmitter.GetViewAngleToTarget();
            float num = 0.5f * ((TimeLoop.GetSecondsRemaining() < 30f) ? 5f : _transmitter._alignmentWindow);
            if (viewAngleToTarget <= num)
            {
                FinalRequirementManager.padLinedUp = true;
                if (_transmitter._objectsOnPlatform.Count > 0 && !_transmitter.IsBlackHoleOpen() &&
                _transmitter._targetReceiver != null && _transmitter._targetReceiver.gameObject.activeInHierarchy)
                {
                    return; // we found the one we will use
                }

            }
        }
        
        // none matched. just leave it at the last one tried, as that will never align (we just checked that)
    }
}