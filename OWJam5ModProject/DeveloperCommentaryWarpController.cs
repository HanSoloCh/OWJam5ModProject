using NewHorizons.Utility;
using OWML.Common;
using System;
using System.Collections;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class DeveloperCommentaryWarpController : MonoBehaviour
    {
        const string BUTTON_PROMPT_TEXT = "DEVELOPER_COMMENTARY_WARP_PROMPT";
        const float MIN_SIGNAL_STRENGTH = 0.9f;
        readonly Vector3 SINGULARITY_OFFSET = Vector3.forward * 0.5f;

        bool commentaryEnabled = false;
        bool warping = false;
        ScreenPrompt buttonPrompt;
        Signalscope signalscope;
        OWRigidbody playerBody;
        GameObject blackHoleRoot;
        SingularityController blackHoleController;
        GameObject whiteHoleRoot;
        SingularityController whiteHoleController;

        void Start()
        {
            OWJam5ModProject.Instance.OnConfigurationChanged += OnConfigurationChanged;

            commentaryEnabled = OWJam5ModProject.Instance.ModHelper.Config.GetSettingsValue<bool>(DeveloperCommentaryEntry.DEVELOPER_COMMENTARY_OPTION);

            string promptText = OWJam5ModProject.Instance.NewHorizons.GetTranslationForUI(BUTTON_PROMPT_TEXT);
            buttonPrompt = new ScreenPrompt(InputLibrary.interactSecondary, promptText);
            Locator.GetPromptManager().AddScreenPrompt(buttonPrompt, PromptPosition.BottomCenter);

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
                if (!warping)
                {
                    StartCoroutine(Warp());
                    warping = true;
                }
            }
        }

        IEnumerator Warp()
        {
            // Can get away with not accounting for planet motion because all our planets are static

            AudioSignal targetCommentarySignal = signalscope.GetStrongestSignal();
            Transform warpArrivalMarker = targetCommentarySignal.transform.parent.Find("WarpArrivalMarker");
            Transform playerCameraTransform = Locator.GetPlayerCamera().transform;

            blackHoleRoot.transform.position = playerCameraTransform.TransformPoint(SINGULARITY_OFFSET);
            whiteHoleRoot.transform.position = targetCommentarySignal.transform.position;
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
