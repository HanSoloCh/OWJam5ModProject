using System;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class RequirementsScreenPrompt : MonoBehaviour
    {
        const string SCREEN_PROMPT_TEXT = "REQUIREMENTS_SCREEN_PROMPT";

        readonly string[] REQUIRED_SHIP_LOG_FACTS = new string[]
        {
            "Walker_Jam5_Station_ProjectRequirements_OppositeSides_Fact",
            "Walker_Jam5_Station_ProjectRequirements_DoubleSand_Fact",
            "Walker_Jam5_Station_ProjectRequirements_Geysers_Fact",
            "Walker_Jam5_Station_ProjectRequirements_FrozenPlanet_Fact",
            "Walker_Jam5_Station_ProjectRequirements_WarpAlignment_Fact",
            "Walker_Jam5_Station_ProjectRequirements_MiddleOrbit_Fact",
            "Walker_Jam5_Station_ProjectRequirements_FinalLocation_Fact"
        };

        [SerializeField] OWTriggerVolume playerDetectorTrigger = null;

        static ScreenPrompt prompt;
        static bool dialogCommentaryPromptWasVisible = false;

        void Start()
        {
            InitializePrompt();

            // Initialize player detector trigger
            playerDetectorTrigger.OnEntry += PlayerDetectorTrigger_OnEntry;
            playerDetectorTrigger.OnExit += PlayerDetectorTrigger_OnExit;
        }

        private void PlayerDetectorTrigger_OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                if (CheckRequiredShipLogFacts())
                    prompt.SetVisibility(true);
            }
        }

        private void PlayerDetectorTrigger_OnExit(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                prompt.SetVisibility(false);
            }
        }

        bool CheckRequiredShipLogFacts()
        {
            foreach (string factID in REQUIRED_SHIP_LOG_FACTS)
            {
                if (!PlayerData._currentGameSave.shipLogFactSaves.ContainsKey(factID) || PlayerData._currentGameSave.shipLogFactSaves[factID].revealOrder <= -1)
                {
                    return false;
                }
            }

            return true;
        }

        static void InitializePrompt()
        {
            string promptText = OWJam5ModProject.Instance.NewHorizons.GetTranslationForUI(SCREEN_PROMPT_TEXT);
            prompt = new ScreenPrompt(promptText);
            Locator.GetPromptManager().AddScreenPrompt(prompt);
        }

        // Hides the screen prompt when dialog commentary is being read
        public static void CommentaryDialogStarted()
        {
            dialogCommentaryPromptWasVisible = prompt._isVisible;
            prompt.SetVisibility(false);
        }

        public static void CommentaryDialogEnded()
        {
            prompt.SetVisibility(dialogCommentaryPromptWasVisible);
        }
    }
}
