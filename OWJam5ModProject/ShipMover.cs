using System;
using System.Linq;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// moves the ship with whatever planet it's on so it doesn't explode as often
/// </summary>
public class ShipMover : MonoBehaviour
{
    private OWRigidbody[] planets;
    private Vector3[] prevPos, delta;

    private void Start()
    {
        planets = CenterOfTheUniverse.s_rigidbodies.Where(x => x.name.StartsWith("Walker_Jam5_Planet")).ToArray();
    }

    private void FixedUpdate()
    {
        // calculate delta pos for each planet
        for (var i = 0; i < planets.Length; i++)
        {
            var pos = planets[i].GetPosition();
            delta[i] = pos - prevPos[i];
            prevPos[i] = pos;
        }
        
        if (!PlanetOrb.OrbsActivated) return;

        var closestPlanet = planets.OrderBy(x => Vector3.Distance(x.GetPosition(), Locator.GetShipBody().GetPosition())).First();
        var closestPlanetIndex = Array.IndexOf(planets, closestPlanet);
        // apply delta to ship
        Locator.GetShipBody().SetPosition(Locator.GetShipBody().GetPosition() + delta[closestPlanetIndex]);
        
        OWJam5ModProject.DebugLog($"delta is {delta[closestPlanetIndex]}");
    }
}