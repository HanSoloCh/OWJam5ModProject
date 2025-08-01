using NewHorizons.Utility;
using OWML.Common;
using System;
using System.Collections;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class DeveloperCommentaryWarpController : MonoBehaviour
    {
        const string WARP_BUTTON_PROMPT_TEXT = "DEVELOPER_COMMENTARY_WARP_PROMPT";
        const string WARP_UNREAD_BUTTON_PROMPT_TEXT = "DEVELOPER_COMMENTARY_WARP_UNREAD_PROMPT";
        const float MIN_SIGNAL_STRENGTH = 0.85f;
        readonly Vector3 SINGULARITY_OFFSET = Vector3.forward * 0.5f;
        readonly IInputCommands WARP_COMMAND = InputLibrary.interact;
        readonly IInputCommands WARP_UNREAD_COMMAND = InputLibrary.autopilot;

        bool commentaryEnabled = false;
        bool warping = false;
        ScreenPrompt warpButtonPrompt;
        ScreenPrompt warpUnreadButtonPrompt;
        Signalscope signalscope;
        OWRigidbody playerBody;
        GameObject blackHoleRoot;
        SingularityController blackHoleController;
        GameObject whiteHoleRoot;
        SingularityController whiteHoleController;

        void Start()
        {
            OWJam5ModProject.Instance.OnConfigurationChanged += OnConfigurationChanged;

            OnConfigurationChanged(OWJam5ModProject.Instance.ModHelper.Config);

            string warpPromptText = OWJam5ModProject.Instance.NewHorizons.GetTranslationForUI(WARP_BUTTON_PROMPT_TEXT);
            warpButtonPrompt = new ScreenPrompt(WARP_COMMAND, warpPromptText);
            Locator.GetPromptManager().AddScreenPrompt(warpButtonPrompt, PromptPosition.BottomCenter);

            string warpUnreadPromptText = OWJam5ModProject.Instance.NewHorizons.GetTranslationForUI(WARP_UNREAD_BUTTON_PROMPT_TEXT);
            warpUnreadButtonPrompt = new ScreenPrompt(WARP_UNREAD_COMMAND, warpUnreadPromptText);
            Locator.GetPromptManager().AddScreenPrompt(warpUnreadButtonPrompt, PromptPosition.BottomCenter);

            playerBody = Locator.GetPlayerBody();

            blackHoleRoot = SearchUtilities.Find("Walker_Jam5_Platform_Body/Sector/DeveloperCommentaryBlackHole");
            blackHoleController = blackHoleRoot.GetComponentInChildren<SingularityController>();
            blackHoleRoot.transform.SetParent(Locator.GetPlayerBody().transform);
            whiteHoleRoot = SearchUtilities.Find("Walker_Jam5_Platform_Body/Sector/DeveloperCommentaryWhiteHole");
            whiteHoleController = whiteHoleRoot.GetComponentInChildren<SingularityController>();

            signalscope = GetComponent<Signalscope>();
        }

        void OnDestroy()
        {
            OWJam5ModProject.Instance.OnConfigurationChanged -= OnConfigurationChanged;
        }

        void OnConfigurationChanged(IModConfig config)
        {
            commentaryEnabled = !config.GetSettingsValue<string>(DeveloperCommentaryEntry.DEVELOPER_COMMENTARY_OPTION).Equals("None");
        }

        void Update()
        {
            if (!commentaryEnabled || !signalscope.IsEquipped() || PlayerState.IsInsideShip() || signalscope.GetFrequencyFilter().ToString() != DeveloperCommentaryEntry.SIGNAL_FREQUENCY_NAME)
            {
                warpButtonPrompt.SetVisibility(false);
                warpUnreadButtonPrompt.SetVisibility(false);
                return;
            }

            // Warp to targeted commentary
            bool canWarp = false;
            if (signalscope.GetStrongestSignalStrength(signalscope._frequencyFilterIndex) > MIN_SIGNAL_STRENGTH)
                canWarp = true;
            warpButtonPrompt.SetVisibility(canWarp);

            if (canWarp && OWInput.IsNewlyPressed(WARP_COMMAND))
            {
                if (!warping)
                {
                    StartCoroutine(Warp(signalscope.GetStrongestSignal()));
                    warping = true;
                }
            }

            // Warp to unread commentary
            bool canWarpToUnread = false;
            AudioSignal highestUnreadSignal = null;
            float highestUnreadSignalStrength = 0;
            foreach (AudioSignal signal in Locator.GetAudioSignals())
            {
                if (signal == null || PlayerData.KnowsSignal(signal.GetName()))
                    continue; // Signals player knows are commentary entries that have already been read

                if (signal.GetSignalStrength() > highestUnreadSignalStrength)
                {
                    highestUnreadSignalStrength = signal.GetSignalStrength();
                    highestUnreadSignal = signal;
                    canWarpToUnread = true;
                }
            }
            warpUnreadButtonPrompt.SetVisibility(canWarpToUnread);

            if (canWarpToUnread && OWInput.IsNewlyPressed(WARP_UNREAD_COMMAND))
            {
                if (!warping)
                {
                    StartCoroutine(Warp(highestUnreadSignal));
                    warping = true;
                }
            }
        }

        IEnumerator Warp(AudioSignal targetSignal)
        {
            // Can get away with not accounting for planet motion because all our planets are static

            Transform warpArrivalMarker = targetSignal.transform.parent.Find("WarpArrivalMarker");
            Transform playerCameraTransform = Locator.GetPlayerCamera().transform;

            blackHoleRoot.transform.position = playerCameraTransform.TransformPoint(SINGULARITY_OFFSET);
            whiteHoleRoot.transform.position = targetSignal.transform.position;
            blackHoleController.Create();
            whiteHoleController.Create();

            yield return new WaitUntil(() => { return blackHoleController.GetState() != SingularityController.State.Creating; });
            
            playerBody.WarpToPositionRotation(warpArrivalMarker.position, warpArrivalMarker.transform.rotation);
            playerBody.SetVelocity(Vector3.zero);
            //whiteHoleRoot.transform.position = playerCameraTransform.TransformPoint(SINGULARITY_OFFSET);

            blackHoleController.CollapseImmediate();
            whiteHoleController.Collapse();

            warping = false;
        }
    }
}
