using System.Linq;
using NewHorizons.Components.Orbital;
using UnityEngine;
using UnityEngine.Rendering;

namespace OWJam5ModProject;

/// <summary>
/// goofy hacky thing that turns off shadows for planets on the other side of the sun as the player.
/// yes, this measn the probe wont see shadows. oh well, the shadow direction is already completely wrong for probe in vanilla anway.
/// </summary>
public class ShadowFixer : MonoBehaviour
{
    private static Transform starTf;

    private Renderer[] shadowRenderers;
    private bool shadowsOn = true;

    public static void Apply()
    {
        //Get the star transform
        starTf = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star").transform;

        foreach (var planet in FindObjectsOfType<NHAstroObject>())
        {
            if (planet._customName.StartsWith("Walker_Jam5_Planet"))
            {
                planet.gameObject.AddComponent<ShadowFixer>();
            }
        }
    }

    private void Start()
    {
        shadowRenderers = gameObject.GetComponentsInChildren<Renderer>(true)
            .Where(x => x.shadowCastingMode == ShadowCastingMode.On)
            .ToArray();
    }

    private void Update()
    {
        var toPlanet = transform.position - starTf.position;
        var toPlayer = Locator.GetPlayerTransform().position - starTf.position;
        var sameSide = Vector3.Dot(toPlayer, toPlanet) > 0;

        // does NOT account for if something dynamically changes the shadows, but we dont do that anywhere so its fine
        if (shadowsOn && !sameSide)
        {
            // OWJam5ModProject.DebugLog($"{this} - shadows off");
            shadowsOn = false;
            foreach (var shadowRenderer in shadowRenderers)
                if (shadowRenderer) shadowRenderer.shadowCastingMode = ShadowCastingMode.Off;
        }
        else if (!shadowsOn && sameSide)
        {
            // OWJam5ModProject.DebugLog($"{this} - shadows on");
            shadowsOn = true;
            foreach (var shadowRenderer in shadowRenderers)
                if (shadowRenderer) shadowRenderer.shadowCastingMode = ShadowCastingMode.On;
        }
    }
}