using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class FinalDoorController : MonoBehaviour
    {
        [Range(0f, 5f)]
        public float t;
        public float speed;
        private OWTriggerVolume _triggerVolume;
        public GameObject[] enabledLights;
        public GameObject[] disabledLights;
        FinalDoorAnimator[] animators;

        public void Start()
        {
            animators = FindObjectsOfType<FinalDoorAnimator>();
            enabled = false;
            PlayAll(false);
            _triggerVolume = base.gameObject.GetAddComponent<OWTriggerVolume>();
            _triggerVolume.OnEntry += OnEntry;
        }

        public void UpdateOrbs()
        {
            SetOrbOn(0, FinalRequirementManager.CheckIceReq());
            SetOrbOn(1, FinalRequirementManager.CheckDryReq());
            SetOrbOn(2, FinalRequirementManager.CheckWarpReq());
            SetOrbOn(3, FinalRequirementManager.CheckSandReq());
            SetOrbOn(4, FinalRequirementManager.CheckAngleReq());
            SetOrbOn(5, FinalRequirementManager.CheckLargePlanetOrbit());
        }

        public void SetOrbOn(int Orb, bool IsOn)
        {
            enabledLights[Orb].SetActive(IsOn);
            disabledLights[Orb].SetActive(!IsOn);
        }

        private void OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                if (FinalRequirementManager.CheckAllReqs())
                {
                    Play();
                }
            }
        }

        public void PlayAll(bool isOn)
        {
            foreach (FinalDoorAnimator anim in animators)
            {
                anim.enabled = isOn;
            }
        }



        public void Play()
        {
            enabled = true;
            PlayAll(true);
        }

        public void Stop()
        {
            enabled = false;
            PlayAll(false);
        }

        public void Update()
        {
            t += Time.deltaTime * speed;
            if (t > 5f) Stop();
        }
    }
}
