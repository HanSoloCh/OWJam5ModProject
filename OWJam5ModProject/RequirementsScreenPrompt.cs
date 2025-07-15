using System;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class RequirementsScreenPrompt : MonoBehaviour
    {
        const string SCREEN_PROMPT_TEXT = "REQUIREMENTS_SCREEN_PROMPT";

        readonly string[] REQUIRED_SHIP_LOG_FACTS = new string[]
        {
            "Walker_Jam5_Station_ProjectRequirements_Count",
            "Walker_Jam5_Station_ProjectRequirements_OppositeSides",
            "Walker_Jam5_Station_ProjectRequirements_DoubleSand",
            "Walker_Jam5_Station_ProjectRequirements_DriedSeabed",
            "Walker_Jam5_Station_ProjectRequirements_FrozenPlanet",
            "Walker_Jam5_Station_ProjectRequirements_WarpAlignment",
            "Walker_Jam5_Station_ProjectRequirements_MiddleOrbit",
            "Walker_Jam5_Station_ProjectRequirements_FinalLocation"
        };

        [SerializeField] OWTriggerVolume playerDetectorTrigger;

        static ScreenPrompt prompt;

        void Start()
        {
            if (prompt == null)
                InitializePrompt();

            // Initialize player detector trigger
            playerDetectorTrigger.OnEntry += PlayerDetectorTrigger_OnEntry;
            playerDetectorTrigger.OnExit += PlayerDetectorTrigger_OnExit;
        }

        private void PlayerDetectorTrigger_OnEntry(GameObject hitObj)
        {
            if (hitObj == Locator.GetPlayerDetector())
            {
                if (CheckRequiredShipLogFacts())
                    prompt.SetVisibility(true);
            }
        }

        private void PlayerDetectorTrigger_OnExit(GameObject hitObj)
        {
            if (hitObj == Locator.GetPlayerDetector())
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
    }
}
