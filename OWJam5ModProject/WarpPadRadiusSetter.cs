using UnityEngine;

namespace OWJam5ModProject
{
    internal class WarpPadRadiusSetter : MonoBehaviour
    {
        [SerializeField] float warpRadius = 0;

        void Start()
        {
            NomaiWarpPlatform warpPlatform = GetComponentInChildren<NomaiWarpPlatform>();
            warpPlatform._warpRadius = warpRadius;
        }
    }
}
