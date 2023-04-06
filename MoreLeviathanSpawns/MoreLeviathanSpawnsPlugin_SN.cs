using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Options.Attributes;

namespace MoreLeviathanSpawns
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class MoreLeviathanSpawnsPlugin_SN : BaseUnityPlugin
    {
        private const string myGUID = "com.haulinoats.moreleviathanspawnssn";
        private const string pluginName = "More Leviathan Spawns";
        private const string versionString = "2.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ManualLogSource logger;

        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        private void Awake()
        {
            harmony.PatchAll();
            Logger.LogInfo(pluginName + " " + versionString + " " + "loaded.");
            logger = Logger;
        }
    }

    [Menu("More Leviathan Spawns")]
    public class Config : SMLHelper.V2.Json.ConfigFile
    {
        [Slider("Reaper Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 3F, Step = 1F, Id = "reaperSpawnIntensity", Tooltip = "Defines general intensity of additional reaper leviathan spawns to add to the game. A value of 1 will add roughly 2 - 4 spawns. A value of 6 will add roughly 20 - 23 spawns. A value of 0 will add no additional reaper leviathan spawns to game."), OnChange(nameof(SpawnIntensityChanged))]
        public float ReaperSpawnIntensity = 3F;
        [Slider("Ghost Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 3F, Step = 1F, Id = "ghostSpawnIntensity", Tooltip = "Defines general intensity of additional ghost leviathan spawns to add to the game. A value of 1 will add roughly 1 - 3 spawns. A value of 6 will add roughly 12 - 14 spawns. A value of 0 will add no additional ghost leviathan spawns to game."), OnChange(nameof(SpawnIntensityChanged))]
        public float GhostSpawnIntensity = 3F;
        [Toggle("Always randomize spawns", Id = "alwaysRandomize", Tooltip = "By default, spawn locations are chosen randomly then saved and remain static for rest of playthrough. If this option is checked, spawns will always randomize when opening that save file."), OnChange(nameof(ToggleChanged))]
        public bool AlwaysRandomized = false;

        private void SpawnIntensityChanged(SliderChangedEventArgs e)
        {
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
            switch (e.Id)
            {
                case "alwaysRandomized":
                    AlwaysRandomized = e.Value;
                    break;
            }
        }
    }
}