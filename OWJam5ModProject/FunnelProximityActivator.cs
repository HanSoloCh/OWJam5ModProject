using System;
using System.Collections;
using UnityEngine;
using NewHorizons.Components.SizeControllers;
using System.Xml;
using NewHorizons.Utility;

namespace OWJam5ModProject
{
    internal class FunnelProximityActivator : MonoBehaviour
    {
        const float SCALE_UP_DELAY = 5f;
        const float SCALE_UP_SPEED = 1f;
        const float SCALE_DOWN_SPEED = 10f;
        const float SIZE = 2;
        const float TRANSFER_DURATION = 15;
        const float ACTIVATION_DISTANCE = 1000;

        float transferProgress;
        FunnelController funnelController;
        IceSphere iceSphere = null;
        GameObject sourceFluid;
        GameObject targetFluid;
        SizeController sourceFluidSizeController;
        SizeController targetFluidSizeController;
        bool funnelActive;
        bool transferingFluid;
        Vector2 sourceHeightRange;
        Vector2 targetHeightRange;
        FunnelProximityActivator additionalHeightFunnel;
        float additionalHeight;
        Coroutine scaleCoroutine;

        public void Initialize(GameObject sourceFluid, float sourceDrainedHeight, GameObject targetFluid, float targetFilledHeight, FunnelProximityActivator additionalHeightFunnel = null, float additionalHeight = 0)
        {
            this.sourceFluid = sourceFluid;
            sourceFluidSizeController = AddSizeController(sourceFluid);
            this.targetFluid = targetFluid;
            targetFluidSizeController = AddSizeController(targetFluid);

            sourceHeightRange = new Vector2(sourceFluid.transform.localScale.x, sourceDrainedHeight);
            targetHeightRange = new Vector2(targetFluid.transform.localScale.x, targetFilledHeight);

            this.additionalHeightFunnel = additionalHeightFunnel;
            this.additionalHeight = additionalHeight;

            iceSphere = sourceFluid.transform.parent.GetComponentInChildren<IceSphere>();
        }

        SizeController AddSizeController(GameObject fluid)
        {
            if (fluid.name == "Water")
            {
                WaterSizeController waterSizeController = fluid.AddComponent<WaterSizeController>();
                waterSizeController.oceanFogMaterial = fluid.FindChild("OceanFog").GetComponent<MeshRenderer>().material;
                waterSizeController.fluidVolume = fluid.FindChild("WaterVolume").GetComponent<RadialFluidVolume>();
                
                return waterSizeController;
            }

            return null;
        }

        void Start()
        {
            funnelController = GetComponent<FunnelController>();
            funnelController.size = 0;
        }

        void Update()
        {
            float distance = (sourceFluid.transform.position - targetFluid.transform.position).magnitude;

            if (distance < ACTIVATION_DISTANCE && (iceSphere == null || !iceSphere.isFreezing) && transferProgress < 1)
                ActivateFunnel();
            else
                DeactivateFunnel();

            if (transferingFluid)
            {
                transferProgress += Time.deltaTime / TRANSFER_DURATION;
                transferProgress = Mathf.Clamp(transferProgress, 0, 1);
                
                if (transferProgress >= 1)
                    DeactivateFunnel();
            }

            if (Time.timeScale > 0)
            {
                ScaleFluid(sourceFluid, sourceFluidSizeController, Mathf.Lerp(sourceHeightRange.x, sourceHeightRange.y, transferProgress));

                float targetFluidScale = Mathf.Lerp(targetHeightRange.x, targetHeightRange.y, transferProgress);
                if (additionalHeightFunnel != null)
                    targetFluidScale += Mathf.Lerp(0, additionalHeight, additionalHeightFunnel.transferProgress);
                ScaleFluid(targetFluid, targetFluidSizeController, targetFluidScale);
            }
        }

        void ScaleFluid(GameObject fluid, SizeController fluidSizeController, float scale)
        {
            if (fluidSizeController != null)
                fluidSizeController.size = scale;
            else
                fluid.transform.localScale = Vector3.one * scale;
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
            if (funnelController.size == 0)
            {
                yield return new WaitForSeconds(SCALE_UP_DELAY);
                transferingFluid = true;
            }

            float startTime = Time.time;
            float initialSize = funnelController.size;

            while (funnelController.size != targetSize)
            {
                float scaleSpeed = targetSize > funnelController.size ? SCALE_UP_SPEED : SCALE_DOWN_SPEED;
                funnelController.size = Mathf.MoveTowards(funnelController.size, targetSize, scaleSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            if (targetSize == 0)
                transferingFluid = false;
        }
    }
}
