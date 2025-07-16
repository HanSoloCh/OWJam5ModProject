using UnityEngine;

namespace OWJam5ModProject
{
    internal class WarpPadRadiusSetter : MonoBehaviour
    {
        [SerializeField] float warpRadius;

        void Start()
        {
            NomaiWarpPlatform warpPlatform = GetComponentInChildren<NomaiWarpPlatform>();
            warpPlatform._warpRadius = warpRadius;
        }
    }
}
