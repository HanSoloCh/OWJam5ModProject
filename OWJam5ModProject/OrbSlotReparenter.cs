using System.Linq;
using NewHorizons;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// unparent from the orb when not dragging, and parent to the orb when dragging
/// </summary>
[UsedInUnityProject]
[RequireComponent(typeof(NomaiInterfaceOrb))]
public class OrbSlotReparenter : MonoBehaviour
{
    private NomaiInterfaceOrb _orb;
    private NomaiInterfaceSlot _slot;

    private void Start()
    {
        _orb = GetComponent<NomaiInterfaceOrb>();
        _slot = _orb._slots.Single();

        _slot.OnSlotActivated += OnSlotActivated;
        _slot.OnSlotDeactivated += OnSlotDeactivated;
        
        // start deactivated
        OnSlotDeactivated(null);
    }

    private void OnDestroy()
    {
        _slot.OnSlotActivated -= OnSlotActivated;
        _slot.OnSlotDeactivated -= OnSlotDeactivated;
    }

    private void OnSlotActivated(NomaiInterfaceSlot slot)
    {
        _slot.transform.parent = _orb._orbBody.GetOrigParent();
    }

    private void OnSlotDeactivated(NomaiInterfaceSlot slot)
    {
        _slot.transform.parent = _orb.transform;
    }
}