using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace BailOutMode.Harmony_Patches
{
    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.multipliedScore), MethodType.Getter)]
    public class ScoreController_prevFrameRawScore
    {
        static bool Prefix(ref int ____multipliedScore, ref int __result)
        {
            int lastMultipliedScore = MultiplayerLocalActiveClient_ScoreControllerHandleScoreDidChange.lastMultipliedScore;
            if (BailOutController.instance.numFails > 0 && lastMultipliedScore >= 0)
            {
#if DEBUG
                Logger.log?.Debug($"Multiplayer Bailout detected. Overriding multiplied score '{____multipliedScore}' with '{lastMultipliedScore}'");
#endif
                __result = lastMultipliedScore;
                return false;
            }
            return true;                
        }
    }

    [HarmonyPatch(typeof(ScoreController), nameof(ScoreController.modifiedScore), MethodType.Getter)]
    public class ScoreController_prevprevFrameModifiedScore
    {
        static bool Prefix(ref int ____modifiedScore, ref float ____gameplayModifiersScoreMultiplier, ref int __result)
        {
            int lastModifiedScore = MultiplayerLocalActiveClient_ScoreControllerHandleScoreDidChange.lastModifiedScore;
            if (BailOutController.instance.numFails > 0 && lastModifiedScore >= 0)
            {
                int modifiedScore = ScoreModel.GetModifiedScoreForGameplayModifiersScoreMultiplier(____modifiedScore, ____gameplayModifiersScoreMultiplier);
#if DEBUG
                Logger.log?.Debug($"Multiplayer Bailout detected. Overriding modified score '{modifiedScore}' with '{lastModifiedScore}'");
#endif
                __result = lastModifiedScore;
                return false;
            }
            return true;
        }
    }
}