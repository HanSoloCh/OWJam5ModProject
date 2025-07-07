using System;
using System.Collections;
using UnityEngine;
using NewHorizons.Components.SizeControllers;

namespace OWJam5ModProject
{
    internal class FunnelProximityActivator : MonoBehaviour
    {
        const float SCALE_SPEED = 0.4f;
        const float SIZE = 2;

        FunnelController funnelController;

        void Start()
        {
            funnelController = GetComponent<FunnelController>();
            funnelController.size = 0;
        }

        void ActivateFunnel()
        {
            StartCoroutine(ScaleFunnel(SIZE));
        }

        void DeactivateFunnel()
        {
            StartCoroutine(ScaleFunnel(0));
        }

        IEnumerator ScaleFunnel(float targetSize)
        {
            float startTime = Time.time;
            float initialSize = funnelController.size;

            while (funnelController.size != targetSize)
            {
                funnelController.size = Mathf.MoveTowards(funnelController.size, targetSize, SCALE_SPEED * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
