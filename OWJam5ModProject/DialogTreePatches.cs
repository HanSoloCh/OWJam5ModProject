using HarmonyLib;
using NewHorizons.Components.Props;

namespace OWJam5ModProject
{
    [HarmonyPatch]
    internal class DialogTreePatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.LateInitialize))]
        public static void NHCharacterDialogTree_LateInitialize_Postfix(CharacterDialogueTree __instance)
        {
            if (__instance._initialized)
            {
                if (__instance.transform.parent.GetComponent<DeveloperCommentaryEntry>())
                {
                    __instance._interactVolume.SetPromptText(UITextType.RecordingPrompt, "\"" + TextTranslation.Translate(__instance._characterName) + "\"");
                    __instance._isRecording = true;
                }
            }
        }
    }
}
