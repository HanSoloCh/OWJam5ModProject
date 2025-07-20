using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using HarmonyLib;

namespace OWJam5ModProject
{
    [HarmonyPatch]
    public class FinalDoorController : MonoBehaviour
    {
        [Range(0f, 5f)]
        public float t;
        public float speed;
        private OWTriggerVolume _triggerVolume;
        public GameObject[] enabledLights;
        public GameObject[] disabledLights;
        FinalDoorAnimator[] animators;
        public Material completedMat;
        public Material defaultMat;
        public MeshRenderer swapMat;
        public UnityEvent onPlay;
        [SerializeField] Transform computerParent = null;
        [SerializeField] RequirementsScreenPrompt screenPromptsVolume = null;
        private static FinalDoorController instance;

        NomaiComputer computer;

        public void Start()
        {
            animators = FindObjectsOfType<FinalDoorAnimator>();
            enabled = false;
            PlayAll(false);
            _triggerVolume = base.gameObject.GetAddComponent<OWTriggerVolume>();
            _triggerVolume.OnEntry += OnEntry;
            computer = computerParent.GetComponentInChildren<NomaiComputer>();
            instance = this;
        }

        public void UpdateOrbs()
        {
            SetOrbOn(0, FinalRequirementManager.CheckIceReq());
            SetOrbOn(1, FinalRequirementManager.CheckGeyserReq());
            SetOrbOn(2, FinalRequirementManager.CheckWarpReq());
            SetOrbOn(3, FinalRequirementManager.CheckSandReq());
            SetOrbOn(4, FinalRequirementManager.CheckAngleReq());
            SetOrbOn(5, FinalRequirementManager.CheckLargePlanetOrbit());

            if (FinalRequirementManager.CheckAllReqs())
            {
                swapMat.materials[0] = completedMat;

                if (t == 0)
                {
                    computer.ClearAllEntries();
                }

                screenPromptsVolume.gameObject.SetActive(false);
            } else
            {
                swapMat.materials[0] = defaultMat;

                if (t == 0)
                {
                    computer.ClearAllEntries();
                    computer.DisplayAllEntries();
                }

                screenPromptsVolume.gameObject.SetActive(true);
            }
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
            if (t == 0)
            {
                onPlay.Invoke();
                enabled = true;
                PlayAll(true);
                computer.ClearAllEntries();
            }
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProbeCamera), nameof(ProbeCamera.TakeSnapshot))]
        private static void ScoutUpdateReqs()
        {
            if (instance != null)
            {
                instance.SetOrbOn(0, FinalRequirementManager.CheckIceReq());
                instance.SetOrbOn(1, FinalRequirementManager.CheckGeyserReq());
                instance.SetOrbOn(2, FinalRequirementManager.CheckWarpReq());
                instance.SetOrbOn(3, FinalRequirementManager.CheckSandReq());
                instance.SetOrbOn(4, FinalRequirementManager.CheckAngleReq());
                instance.SetOrbOn(5, FinalRequirementManager.CheckLargePlanetOrbit());
            }
        }
    }
}
