using NewHorizons;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// keep the orb in place when not being dragged, acting like a slot
/// </summary>
[UsedInUnityProject]
[RequireComponent(typeof(NomaiInterfaceOrb))]
public class FakeOrbSlot : MonoBehaviour
{
    private NomaiInterfaceOrb _orb;
    private bool _wasBeingDragged;
    private Vector3 _localLockPos;

    private void Awake()
    {
        _orb = GetComponent<NomaiInterfaceOrb>();
        _localLockPos = _orb._orbBody.GetOrigParent().InverseTransformPoint(_orb.transform.position);
    }

    private void FixedUpdate()
    {
        var isBeingDragged = _orb._isBeingDragged;

        if (_wasBeingDragged && !isBeingDragged)
        {
            _localLockPos = _orb._localTargetPos;
        }

        if (!isBeingDragged)
        {
            _orb.MoveTowardPosition(_orb._orbBody.GetOrigParent().TransformPoint(_localLockPos));
        }
        
        _wasBeingDragged = isBeingDragged;
    }
}