#define DEBUG_VELOCITY

using System;
using HarmonyLib;
using NewHorizons;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// keep the orb in place when not being dragged, acting like a slot
/// </summary>
[UsedInUnityProject]
[RequireComponent(typeof(NomaiInterfaceOrb))]
[HarmonyPatch]
public class FakeOrbSlot : MonoBehaviour
{
    private NomaiInterfaceOrb _orb;
    private bool _wasBeingDragged;
    private Vector3 _localLockPos;
    
    private Vector3 _debugVelocity;

    private void Start()
    {
        _orb = GetComponent<NomaiInterfaceOrb>();
        _localLockPos = _orb._orbBody.GetOrigParent().InverseTransformPoint(_orb.transform.position);
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

        _debugVelocity = _orb._orbBody.GetVelocity();
    }

    private void OnGUI()
    {
        GUILayout.Label($"orb {_orb} vel = {_debugVelocity}");
    }
}