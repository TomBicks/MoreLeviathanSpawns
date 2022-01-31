using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
// ### Enhancing the mod ###
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Options;

namespace MoreReaperSpawns
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

    [Menu("More Reaper Spawns")]
    public class Config : ConfigFile
    {
        [Slider("Spawn Intensity", Min = 1F, Max = 5F, DefaultValue = 1F, Step = 1F, Tooltip = "Defines how many leviathan spawns you want to add to the game"), OnChange(nameof(SpawnIntensityChanged))]
        public float SpawnIntensity = 1F;
        [Toggle("Always randomize spawns", Tooltip = "By default, spawn locations are chosen randomly then saved and remain static for rest of playthrough. If this option is checked, spawns will always randomize when opening that save file."), OnChange(nameof(AlwaysRandomizedChanged))]
        public bool AlwaysRandomized = false;

        private void SpawnIntensityChanged(SliderChangedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, $"Spawn itensity set to: {e.Value}");
            SpawnIntensity = e.Value;
        }

        private void AlwaysRandomizedChanged(ToggleChangedEventArgs e)
        {
            Logger.Log(Logger.Level.Info, $"Always Randomized changed to: {e.Value}");
            AlwaysRandomized = e.Value;
        }
    }
}