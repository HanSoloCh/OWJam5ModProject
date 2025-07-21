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
    public static class BundleBrokenMessage
    {

        //static ScreenPrompt prompt;
        public static void BeginCheck()
        {
            bool loadedCorrectly = false;
            if (GameObject.FindObjectOfType<GrandOrreryController>())
            {
                OWJam5ModProject.DebugLog("Bundle loaded correctly!! Thank goodness");
                loadedCorrectly = true;
            }
            if (!loadedCorrectly)
            {
                OWJam5ModProject.DebugLog("Assetbundles not loaded for Heliostudy! This was likely caused by the New Horizons \"Reload Config\" option.");
                GameObject.Find("Walker_Jam5_Platform_Body").gameObject.SetActive(false);
                GameObject newObj = new GameObject();
                newObj.AddComponent<ActualBundleMessagerFix>();
            }
        }
    }
}
