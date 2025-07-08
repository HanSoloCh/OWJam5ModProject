using System;
using System.Collections;
using UnityEngine;
using NewHorizons.Components.SizeControllers;

namespace OWJam5ModProject
{
    internal class FunnelProximityActivator : MonoBehaviour
    {
        const float SCALE_SPEED = 2f;
        const float SIZE = 2;
        const float TRANSFER_DURATION = 15;
        const float ACTIVATION_DISTANCE = 1000;

        float transferProgress;
        FunnelController funnelController;
        GameObject sourceFluid;
        GameObject targetFluid;
        bool funnelActive;
        Vector2 sourceHeightRange;
        Vector2 targetHeightRange;
        FunnelProximityActivator additionalHeightFunnel;
        float additionalHeight;
        Coroutine scaleCoroutine;

        public void Initialize(GameObject sourceFluid, float sourceDrainedHeight, GameObject targetFluid, float targetFilledHeight, FunnelProximityActivator additionalHeightFunnel = null, float additionalHeight = 0)
        {
            this.sourceFluid = sourceFluid;
            this.targetFluid = targetFluid;

            sourceHeightRange = new Vector2(sourceFluid.transform.localScale.x, sourceDrainedHeight);
            targetHeightRange = new Vector2(targetFluid.transform.localScale.x, targetFilledHeight);

            this.additionalHeightFunnel = additionalHeightFunnel;
            this.additionalHeight = additionalHeight;
        }

        void Start()
        {
            funnelController = GetComponent<FunnelController>();
            funnelController.size = 0;
        }

        void Update()
        {
            float distance = (sourceFluid.transform.position - targetFluid.transform.position).magnitude;

            //Check if the source has an ice sphere and, if it does, allow it to disable this
            IceSphere iceSphere = sourceFluid.transform.parent.GetComponentInChildren<IceSphere>();

            if (distance < ACTIVATION_DISTANCE && (iceSphere == null || !iceSphere.isFreezing))
                ActivateFunnel();
            else
                DeactivateFunnel();

            if (funnelActive)
            {
                transferProgress += Time.deltaTime / TRANSFER_DURATION;
                transferProgress = Mathf.Clamp(transferProgress, 0, 1);
                sourceFluid.transform.localScale = Vector3.one * Mathf.Lerp(sourceHeightRange.x, sourceHeightRange.y, transferProgress);
                targetFluid.transform.localScale = Vector3.one * Mathf.Lerp(targetHeightRange.x, targetHeightRange.y, transferProgress);

                if (additionalHeightFunnel != null)
                    targetFluid.transform.localScale = targetFluid.transform.localScale + Vector3.one * Mathf.Lerp(0, additionalHeight, additionalHeightFunnel.transferProgress);

                if (transferProgress >= 1)
                    DeactivateFunnel();
            }
        }

        void ActivateFunnel()
        {
            if (funnelActive)
                return;

            if (scaleCoroutine != null)
                StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleFunnel(SIZE));
            funnelActive = true;
        }

        void DeactivateFunnel()
        {
            if (!funnelActive)
                return;

            if (scaleCoroutine != null)
                StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleFunnel(0));
            funnelActive = false;
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
