using HarmonyLib;
using Logger = QModManager.Utility.Logger;

namespace MoreReaperSpawns
{
    [HarmonyPatch(typeof(PlayerTool))]
    [HarmonyPatch("Awake")]
    internal class PatchPlayerToolAwake
    {
        [HarmonyPostfix]
        public static void Postfix(PlayerTool __instance)
        {
            // Check to see if this is the knife
            if (__instance.GetType() == typeof(Knife))
            {
                Knife knife = __instance as Knife;
                // Double the knife damage
                float knifeDamage = knife.damage;
                float newKnifeDamage = knifeDamage * 100;
                knife.damage = newKnifeDamage;
                Logger.Log(Logger.Level.Debug, $"Knife damage was: {knifeDamage}," +
                    $" is now: {newKnifeDamage}");
            }
        }
    }
}