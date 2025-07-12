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
    private Vector3 _localLockPos;
    private FakeOrbSlot[] _slots;

    private void Start()
    {
        _orb = GetComponent<NomaiInterfaceOrb>();
        _localLockPos = _orb._orbBody.GetOrigParent().InverseTransformPoint(_orb.transform.position);
        _slots = transform.parent.GetComponentsInChildren<FakeOrbSlot>();
    }

    [HarmonyPrefix, HarmonyPatch(typeof(NomaiInterfaceOrb), nameof(NomaiInterfaceOrb.FixedUpdate))]
    private static void NomaiInterfaceOrb_FixedUpdate(NomaiInterfaceOrb __instance)
    {
        var @this = __instance.GetComponent<FakeOrbSlot>();
        if (@this) @this.PreOrbFixedUpdate();
    }

    private void PreOrbFixedUpdate()
    {
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
                float exclusionDist = 1.3f;
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