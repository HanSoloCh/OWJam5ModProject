using OWML.Common;
using System;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class DeveloperCommentaryWarpController : MonoBehaviour
    {
        const string BUTTON_PROMPT_TEXT = "DEVELOPER_COMMENTARY_WARP_PROMPT";
        const float MIN_SIGNAL_STRENGTH = 0.9f;
        readonly Vector3 WARP_OFFSET = Vector3.up * 2;

        bool commentaryEnabled = false;
        ScreenPrompt buttonPrompt;
        Signalscope signalscope;
        OWRigidbody playerBody;

        void Start()
        {
            OWJam5ModProject.Instance.OnConfigurationChanged += OnConfigurationChanged;

            commentaryEnabled = OWJam5ModProject.Instance.ModHelper.Config.GetSettingsValue<bool>(DeveloperCommentaryEntry.DEVELOPER_COMMENTARY_OPTION);

            string promptText = OWJam5ModProject.Instance.NewHorizons.GetTranslationForUI(BUTTON_PROMPT_TEXT);
            buttonPrompt = new ScreenPrompt(InputLibrary.interactSecondary, promptText);
            Locator.GetPromptManager().AddScreenPrompt(buttonPrompt, PromptPosition.BottomCenter);

            playerBody = Locator.GetPlayerBody();

            signalscope = GetComponent<Signalscope>();
        }

        void OnDestroy()
        {
            OWJam5ModProject.Instance.OnConfigurationChanged -= OnConfigurationChanged;
        }

        void OnConfigurationChanged(IModConfig config)
        {
            commentaryEnabled = config.GetSettingsValue<bool>(DeveloperCommentaryEntry.DEVELOPER_COMMENTARY_OPTION);
        }

        void Update()
        {
            bool canWarp = false;
            if (commentaryEnabled && signalscope._isEquipped && !PlayerState.IsInsideShip())
            {
                if (signalscope.GetFrequencyFilter().ToString() == DeveloperCommentaryEntry.SIGNAL_FREQUENCY_NAME)
                {
                    if (signalscope.GetStrongestSignalStrength(signalscope._frequencyFilterIndex) > MIN_SIGNAL_STRENGTH)
                        canWarp = true;
                }
            }
            buttonPrompt.SetVisibility(canWarp);

            if (canWarp && OWInput.IsNewlyPressed(InputLibrary.interactSecondary))
            {
                // TODO: Add singularity effects
                AudioSignal targetCommentarySignal = signalscope.GetStrongestSignal();
                playerBody.WarpToPositionRotation(targetCommentarySignal.transform.TransformPoint(WARP_OFFSET), targetCommentarySignal.transform.rotation);
                playerBody.SetVelocity(Vector3.zero);
            }
        }
    }
}
