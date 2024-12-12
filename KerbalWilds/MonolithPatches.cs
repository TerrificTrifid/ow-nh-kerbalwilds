using HarmonyLib;
using UnityEngine;

namespace KerbalWilds;

[HarmonyPatch]
public class MonolithPatches : MonoBehaviour
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.UpdateTimeFreeze))]
    public static bool NomaiTranslatorProp_UpdateTimeFreeze_Prefix()
    {
        // for monolith visuals
        if (KerbalWilds.Instance.InSystem) return false;
        return true;
    }
}