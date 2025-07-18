using HarmonyLib;
using NewHorizons;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// keep the orb in place when not being dragged, acting like a slot
/// </summary>
[RequireComponent(typeof(NomaiInterfaceOrb))]
[HarmonyPatch]
public class FakeOrbSlot : MonoBehaviour
{
    private NomaiInterfaceOrb _orb;
    private bool _wasBeingDragged;
    internal Vector3 _localLockPos;
    private FakeOrbSlot[] _slots;

    private void Start()
    {
        _orb = GetComponent<NomaiInterfaceOrb>();
        _localLockPos = _orb._orbBody.GetOrigParent().InverseTransformPoint(_orb.transform.position);
        _slots = _orb._orbBody.GetOrigParent().GetComponentsInChildren<FakeOrbSlot>();
        
        _orb._freezeLocalRotation = true; // so the decals dont rotate
    }

    // have to run our code before orb sets its velocity
    [HarmonyPrefix, HarmonyPatch(typeof(NomaiInterfaceOrb), nameof(NomaiInterfaceOrb.FixedUpdate))]
    private static void NomaiInterfaceOrb_FixedUpdate(NomaiInterfaceOrb __instance)
    {
        if (__instance.TryGetComponent<FakeOrbSlot>(out var @this))
            @this.PreOrbFixedUpdate(); 
    }

    private void PreOrbFixedUpdate()
    {
        if (!_orb) return; // start hasnt run yet
        
        var isBeingDragged = _orb._isBeingDragged;

        if (_wasBeingDragged && !isBeingDragged)
        {
            // _localLockPos = _orb._localTargetPos;
            _localLockPos = _orb._orbBody.GetOrigParent().InverseTransformPoint(_orb.transform.position);
        }

        if (!isBeingDragged)
        {
            _orb.MoveTowardPosition(_orb._orbBody.GetOrigParent().TransformPoint(_localLockPos));
        }

        _wasBeingDragged = isBeingDragged;

        //Prevent orbs from getting too close
        foreach(FakeOrbSlot slot in _slots)
        {
            if(slot != this)
            {
                Vector3 hereToThere = slot.transform.position - this.transform.position;
                float exclusionDist = 1.4f;
                if(hereToThere.magnitude < exclusionDist)
                {
                    Vector3 toMove = (hereToThere.normalized * exclusionDist) - hereToThere;
                    this.transform.position += toMove * -0.5f;
                    slot.transform.position += toMove * 0.5f;
                }
            }
        }

        // OWJam5ModProject.DebugLog($"orb {_orb} vel = {_orb._orbBody.GetVelocity()}");
    }
}