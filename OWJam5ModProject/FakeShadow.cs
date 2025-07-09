using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace OWJam5ModProject;

/// <summary>
/// draws extra stuff into the screen space shadow mask
/// </summary>
[RequireComponent(typeof(Renderer))]
public class FakeShadow : MonoBehaviour
{
    private Light light;
    private Renderer renderer;

    private CommandBuffer cmd;

    private void Start()
    {
        // grab the stuff we need
        renderer = GetComponent<Renderer>();
        light = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star").GetComponentsInChildren<Light>()
            .First(x => x.type == LightType.Directional);

        cmd = new CommandBuffer();

        // draw our stuff after screen space shadow mask gets drawn into by normal shadow stuff
        light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, cmd);

        Camera.onPreRender += BuildCmd;
        Camera.onPostRender += ClearCmd;
    }

    private void OnDestroy()
    {
        cmd.Release();

        Camera.onPreRender -= BuildCmd;
        Camera.onPostRender -= ClearCmd;
    }

    private void BuildCmd(Camera cam)
    {
        // set matrices back to regular camera stuff
        cmd.SetViewProjectionMatrices(cam.worldToCameraMatrix, cam.projectionMatrix);
        // draw our decal like a normal renderer into screen space shadow mask
        cmd.DrawRenderer(renderer, renderer.sharedMaterial);
    }

    private void ClearCmd(Camera cam)
    {
        // clear it out since we'll fill it with new data next frame
        cmd.Clear();
    }
}