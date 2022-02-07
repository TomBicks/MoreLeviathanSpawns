using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
// ### Enhancing the mod ###
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Options;
using SMLHelper.V2.Interfaces;
using UnityEngine;

namespace MoreLeviathanSpawns
{
    [QModCore]
    public static class QMod
    {
        internal static Config Config { get; } = OptionsPanelHandler.Main.RegisterModOptions<Config>();

        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var modName = ($"haulinoats_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {modName}");
            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }

    [Menu("More Leviathan Spawns")]
    public class Config : ConfigFile
    {
        [Slider("Reaper Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 1F, Step = 1F, Id = "reaperSpawnIntensity", Tooltip = "Defines general intensity of additional reaper leviathan spawns to add to the game. A value of 1 will add roughly 2 - 4 spawns. A value of 6 will add roughly 20 - 23 spawns. A value of 0 will add no additional reaper leviathan spawns to game."), OnChange(nameof(SpawnIntensityChanged))]
        public float ReaperSpawnIntensity = 1F;
       
        [Slider("Ghost Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 1F, Step = 1F, Id = "ghostSpawnIntensity", Tooltip = "Defines general intensity of additional ghost leviathan spawns to add to the game. A value of 1 will add roughly 1 - 3 spawns. A value of 6 will add roughly 12 - 14 spawns. A value of 0 will add no additional ghost leviathan spawns to game."), OnChange(nameof(SpawnIntensityChanged))]
        public float GhostSpawnIntensity = 1F;
        [Toggle("Always randomize spawns", Id = "alwaysRandomize", Tooltip = "By default, spawn locations are chosen randomly then saved and remain static for rest of playthrough. If this option is checked, spawns will always randomize when opening that save file."), OnChange(nameof(ToggleChanged))]
        public bool AlwaysRandomized = false;

        private void SpawnIntensityChanged(SliderChangedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, $"Spawn itensity for {e.Id} is now set to {e.Value}");
            switch (e.Id)
            {
                case "reaperSpawnIntensity":
                    ReaperSpawnIntensity = e.Value;
                    break;
                case "ghostSpawnIntensity":
                    GhostSpawnIntensity = e.Value;
                    break;
            }
        }

        private void ToggleChanged(ToggleChangedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, $"Selected option {e.Id} is now set to {e.Value}");
            switch (e.Id)
            {
                case "alwaysRandomized":
                    AlwaysRandomized = e.Value;
                    break;
            }
        }
    }
}