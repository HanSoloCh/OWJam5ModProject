using System.Linq;
using NewHorizons.Components.SizeControllers;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// turn one water off when ur in the other water because of the weird visual issue
/// </summary>
public class TurnWaterOff : MonoBehaviour
{
    private Renderer[] otherFogs;

    public static void Apply()
    {
        var waters = FindObjectsOfType<WaterSizeController>().Select(x => x.gameObject).ToArray();
        foreach (var water in waters)
        {
            var turnWaterOff = water.AddComponent<TurnWaterOff>();
            turnWaterOff.otherFogs = waters.Except([water])
                .Select(x => x.transform.Find("OceanFog").GetComponent<Renderer>())
                .ToArray();
        }
    }

    private void Start()
    {
        var volume = transform.Find("WaterVolume").GetComponent<OWTriggerVolume>();
        volume.OnEntry += obj =>
        {
            if (obj.CompareTag("PlayerCameraDetector"))
                foreach (var otherFog in otherFogs)
                    otherFog.enabled = false;
        };
        volume.OnExit += obj =>
        {
            if (obj.CompareTag("PlayerCameraDetector"))
                foreach (var otherFog in otherFogs)
                    otherFog.enabled = true;
        };
    }
}