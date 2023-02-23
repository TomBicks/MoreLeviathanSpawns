using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SMLHelper.V2.Json;
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

        //public static bool AlwaysRandomized;
        //public static int ReaperSpawnIntensity;
        //public static int ConfigGhostSpawnIntensity;

        private void Awake()
        {
            //ConfigAlwaysRandomized = Config.Bind("General",
            //    "alwaysRandomized",
            //    false,
            //    "Always randomize spawns on file load");
            //ConfigReaperSpawnIntensity = Config.Bind("General",
            //    "reaperSpawnIntensity",
            //    3,
            //    new ConfigDescription("Reaper leviathan spawn Intensity (0 - 6). 0 = no spawns added, 1 = 2 - 4 spawns added, 6 = 20 - 23 spawns added.", new AcceptableValueRange<int>(0, 6)));
            //ConfigGhostSpawnIntensity = Config.Bind("General",
            //    "ghostSpawnIntensity",
            //    3,
            //    new ConfigDescription("Ghost leviathan spawn Intensity(0 - 6). 0 = no spawns added, 1 = 1 - 3 spawns added, 6 = 12 - 14 spawns added.", new AcceptableValueRange<int>(0, 6)));

            harmony.PatchAll();
            Logger.LogInfo(pluginName + " " + versionString + " " + "loaded.");
            logger = Logger;
        }
    }

    [Menu("More Leviathan Spawns")]
    public class Config : SMLHelper.V2.Json.ConfigFile
    {
        [Slider("Reaper Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 1F, Step = 1F, Id = "reaperSpawnIntensity", Tooltip = "Defines general intensity of additional reaper leviathan spawns to add to the game. A value of 1 will add roughly 2 - 4 spawns. A value of 6 will add roughly 20 - 23 spawns. A value of 0 will add no additional reaper leviathan spawns to game."), OnChange(nameof(SpawnIntensityChanged))]
        public static float ReaperSpawnIntensity = 3F;
        [Slider("Ghost Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 1F, Step = 1F, Id = "ghostSpawnIntensity", Tooltip = "Defines general intensity of additional ghost leviathan spawns to add to the game. A value of 1 will add roughly 1 - 3 spawns. A value of 6 will add roughly 12 - 14 spawns. A value of 0 will add no additional ghost leviathan spawns to game."), OnChange(nameof(SpawnIntensityChanged))]
        public static float GhostSpawnIntensity = 3F;
        [Toggle("Always randomize spawns", Id = "alwaysRandomize", Tooltip = "By default, spawn locations are chosen randomly then saved and remain static for rest of playthrough. If this option is checked, spawns will always randomize when opening that save file."), OnChange(nameof(ToggleChanged))]
        public static bool AlwaysRandomized = false;

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