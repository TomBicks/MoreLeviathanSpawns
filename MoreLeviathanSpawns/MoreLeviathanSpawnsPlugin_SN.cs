using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using Nautilus.Options.Attributes;
using UnityEngine;

#pragma warning disable IDE1006 // Suppress warnings related to "Naming Styles"

namespace MoreLeviathanSpawns
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class MoreLeviathanSpawnsPlugin_SN : BaseUnityPlugin
    {
        private const string myGUID = "com.haulinoats.moreleviathanspawnssn";
        private const string pluginName = "More Leviathan Spawns";
        private const string versionString = "2.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static ManualLogSource logger { get; private set; }

        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        internal static SaveCoords saveCoords { get; } = SaveDataHandler.RegisterSaveDataCache<SaveCoords>();

        //SAVEDATACACHE TESTING!!
        [FileName("leviathans_coords")]
        internal class SaveCoords : SaveDataCache
        {
            public static SaveCoords Main;
            public UnityEngine.Vector3 ReaperCoords { get; set; }
            public UnityEngine.Vector3 GhostCoords { get; set; }
        }

        public void Awake()
        {
            harmony.PatchAll();
            Logger.LogInfo(pluginName + " " + versionString + " " + "loaded.");
            logger = Logger;

            /*saveCoords.ReaperCoords = {
                new Vector3(120, -40, -568), //Grassy Plateaus, South
                new Vector3(1407, -190, 584), //Bulb Zone, East-North-East
                new Vector3(278, -175, 1398} //Mountains, North
            };*/

            //Check whether it's loading correctly
            saveCoords.OnStartedLoading += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;

                logger.LogInfo($"loading from filepath: {coords.JsonFilePath}");

                logger.LogInfo($"loading reaper coords from save slot: {coords.ReaperCoords}");
                ErrorMessage.AddMessage($"loading reaper coords from save slot: {coords.ReaperCoords}");
            };

            //Display coords upon loading a save file
            saveCoords.OnFinishedLoading += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;

                logger.LogInfo($"loaded from filepath: {coords.JsonFilePath}");

                logger.LogInfo($"loaded reaper coords from save slot: {coords.ReaperCoords}");
                //logger.LogInfo($"loaded ghost coords from save slot: {coords.GhostCoords}");
                ErrorMessage.AddMessage($"loaded reaper coords from save slot: {coords.ReaperCoords}");
                //ErrorMessage.AddMessage($"loaded ghost coords from save slot: {coords.GhostCoords}");
            };

            saveCoords.OnFinishedSaving += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;
                logger.LogInfo($"saved reaper coords from save slot: {coords.ReaperCoords}");
                //logger.LogInfo($"saved ghost coords from save slot: {coords.GhostCoords}");
                ErrorMessage.AddMessage($"saved reaper coords from save slot: {coords.ReaperCoords}");
                //ErrorMessage.AddMessage($"saved ghost coords from save slot: {coords.GhostCoords}");
            };
        }
    }

    [Menu("More Leviathan Spawns")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Slider("Reaper Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 3F, Step = 1F, Id = "reaperSpawnIntensity", Tooltip = "Defines general intensity of additional reaper leviathan spawns to add to the game. A value of 1 will add roughly 2 - 4 spawns. A value of 6 will add roughly 20 - 23 spawns. A value of 0 will add no additional reaper leviathan spawns to game.")]
        public float ReaperSpawnIntensity = 3F;
        [Slider("Ghost Spawn Intensity", Min = 0F, Max = 6F, DefaultValue = 3F, Step = 1F, Id = "ghostSpawnIntensity", Tooltip = "Defines general intensity of additional ghost leviathan spawns to add to the game. A value of 1 will add roughly 1 - 3 spawns. A value of 6 will add roughly 12 - 14 spawns. A value of 0 will add no additional ghost leviathan spawns to game.")]
        public float GhostSpawnIntensity = 3F;
        //NOTE!! How could this have possible worked, if you can't unspawn leviathans? All this would have done would randomise until all unique spawns were registered
        //In essence, this just eventually hit the max, always.
        //Need to either figure out if leviathans can be unregistered from the world spawn thing, or need to remove this option entirely.
        [Toggle("Always randomize spawns?", Id = "alwaysRandomize", Tooltip = "By default, spawn locations are chosen randomly then saved and remain static for rest of playthrough. If this option is checked, spawns will always randomize when opening that save file.")]
        public bool AlwaysRandomized = false;

        /*OKAY!! So, the plan is (best case; can work down from here);
         * - Create a new json file per save, recognising the identifier for each new save, that includes the randomly generated list of coordinates of leviathans for that world
         * - json file is split between the name of the save, to identify it, and the coordinates used, which are run through the registering code, in case any are missed
         * - Creates if it can find no file with the appropirate naming convention (being the name of the save plus something like "leviathancoords"
         * - Loads from file if it does find it
        */

        /*private void SpawnIntensityChanged(SliderChangedEventArgs e)
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
        }*/
    }
}