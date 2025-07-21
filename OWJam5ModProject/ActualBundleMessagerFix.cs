using Epic.OnlineServices.Stats;
using NewHorizons.Components.SizeControllers;
using NewHorizons.Utility.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class ActualBundleMessagerFix : MonoBehaviour
    {
        static ScreenPrompt prompt;
        public void Start()
        {
            InitializePrompt();
        }

        static void InitializePrompt()
        {
            string promptText = "Assetbundles not loaded for Heliostudy!\nThis was likely caused by the New Horizons \"Reload Config\" option.";
            prompt = new ScreenPrompt(promptText);
            Locator.GetPromptManager().AddScreenPrompt(prompt);
            prompt.SetVisibility(true);
        }
    }
}
