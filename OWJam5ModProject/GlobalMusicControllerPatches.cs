using HarmonyLib;
using NewHorizons.Handlers;
using NewHorizons.Utility.Files;
using NewHorizons.Utility.OWML;
using UnityEngine;

namespace OWJam5ModProject;

/// <summary>
/// handle our custom travel audio only playing when near our system
/// </summary>
[HarmonyPatch(typeof(GlobalMusicController))]
public static class GlobalMusicControllerPatches
{
    private static OWAudioSource travelSource2;
    private static GameObject star;
    private static bool wasInOurSystem;

    [HarmonyPostfix, HarmonyPatch(nameof(GlobalMusicController.Start))]
    private static void Start(GlobalMusicController __instance)
    {
        star = OWJam5ModProject.Instance.NewHorizons.GetPlanet("Walker_Jam5_Star");
        wasInOurSystem = false;
        if (!star) return; // not in jam5 system

        var go = new GameObject("custom travel audio");
        go.SetActive(false);
        travelSource2 = go.AddComponent<OWAudioSource>();
        travelSource2._audioSource = go.GetComponent<AudioSource>();
        travelSource2.loop = true;
        travelSource2.playOnAwake = false;
        AudioUtilities.SetAudioClip(travelSource2, "planets/assets/audio/space.ogg", OWJam5ModProject.Instance);
        travelSource2.SetTrack(OWAudioMixer.TrackName.Music);
        go.SetActive(true);
        OWJam5ModProject.DebugLog("custom travel audio made!");
        
        // TEMP
        AudioUtilities.SetAudioClip(__instance._travelSource, "planets/assets/audio/jam5space.ogg", OWJam5ModProject.Instance);
    }


    [HarmonyPrefix, HarmonyPatch(nameof(GlobalMusicController.UpdateTravelMusic))]
    private static bool UpdateTravelMusic(GlobalMusicController __instance)
    {
        bool inOurSystem = Vector3.Distance(Locator.GetPlayerTransform().position, star.transform.position) < 2500;

        bool shouldPlay = PlayerState.AtFlightConsole() && !PlayerState.IsHullBreached() &&
                          Locator.GetPlayerRulesetDetector().AllowTravelMusic() && !__instance._playingFinalEndTimes;
        bool isPlaying = __instance._travelSource.isPlaying && !__instance._travelSource.IsFadingOut();
        isPlaying |= travelSource2.isPlaying && !travelSource2.IsFadingOut();

        // handle switching between systems (switch tracks)
        if (inOurSystem && !wasInOurSystem)
        {
            travelSource2.FadeIn(5);
            __instance._travelSource.FadeOut(5f, OWAudioSource.FadeOutCompleteAction.PAUSE);
            OWJam5ModProject.DebugLog("swap to custom travel");
        }
        else if (!inOurSystem && wasInOurSystem)
        {
            travelSource2.FadeOut(5, OWAudioSource.FadeOutCompleteAction.PAUSE);
            __instance._travelSource.FadeIn(5f);
            OWJam5ModProject.DebugLog("swap to default travel");
        }
        // else, handle fading in from nothing and out to nothing
        else if (inOurSystem)
        {
            if (shouldPlay && !isPlaying)
            {
                travelSource2.FadeIn(5);
                OWJam5ModProject.DebugLog("fade in custom travel");
            }
            else if (!shouldPlay && isPlaying)
            {
                travelSource2.FadeOut(5, OWAudioSource.FadeOutCompleteAction.PAUSE);
                OWJam5ModProject.DebugLog("fade out custom travel");
            }
        }
        else
        {
            if (shouldPlay && !isPlaying)
            {
                __instance._travelSource.FadeIn(5f);
                OWJam5ModProject.DebugLog("fade in default travel");
            }
            else if (!shouldPlay && isPlaying)
            {
                __instance._travelSource.FadeOut(5f, OWAudioSource.FadeOutCompleteAction.PAUSE);
                OWJam5ModProject.DebugLog("fade out default travel");
            }
        }

        wasInOurSystem = inOurSystem;

        return false;
    }
}