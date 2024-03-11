using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Commands;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using Nautilus.Options.Attributes;
using System;
using System.Collections.Generic;
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

        [FileName("leviathans_coords")]
        internal class SaveCoords : SaveDataCache
        {
            public float ReaperSpawnIntensity { get; set; }
            public float GhostSpawnIntensity { get; set; }

            public List<Vector3> ReaperCoords { get; set; }
            public List<GhostCoordsAndType> GhostCoords { get; set; }
        }

        public void Awake()
        {
            harmony.PatchAll();
            Logger.LogInfo(pluginName + " " + versionString + " " + "loaded.");
            logger = Logger;

            /*OnStartLoading, check whether the coord file has null values;
             - if it does, it means the save and coord files are being created for the first time and that we need
               to populate the coord file with a random selection of leviathan coordinates, based on the intensity selected
             - if it doesn't have null values, it means this is not the first time the save has been loaded and we can skip
               the process of populating the coord file with levaithan coordinates*/
            saveCoords.OnStartedLoading += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;

                logger.LogInfo($"Loading from filepath: {coords.JsonFilePath}");

                if(coords.ReaperCoords is null)
                {
                    //Loading the save for the first time and creating the coord file for the first time
                    logger.LogInfo("Coord File is null! Preparing to populate Leviathan Coords file...");
                    PopulateCoordArray();

                    //Now that the SaveDataCache has all the proper values assigned for this save, properly save the new coord file and its values
                    saveCoords.Save();
                }
                else
                {
                    logger.LogInfo("Coord File is already populated!");
                }
            };

            //Display coords upon loading a save
            saveCoords.OnFinishedLoading += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;

                ErrorMessage.AddMessage($"Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                ErrorMessage.AddMessage($"Loaded {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
            };

            saveCoords.OnFinishedSaving += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;


                ErrorMessage.AddMessage($"Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                ErrorMessage.AddMessage($"Saved {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
            };

            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(LeviathanCommands));
        }

        public static class LeviathanCommands
        {
            [ConsoleCommand("coordwarp")]
            public static void CoordWarp(int index)
            {
                int reaper_count = saveCoords.ReaperCoords.Count;
                int ghost_count = saveCoords.GhostCoords.Count;

                if (index >= 1 || index <= (reaper_count + ghost_count))
                {
                    if (index <= reaper_count)
                    {
                        ErrorMessage.AddMessage($"Teleporting to Reaper coord #{index} - Coords: {saveCoords.ReaperCoords[index - 1]}");
                        Player.main.SetPosition(saveCoords.ReaperCoords[index - 1]);
                    }
                    else
                    {
                        ErrorMessage.AddMessage($"Teleporting to Ghost coord #{index - reaper_count} - Coords: {saveCoords.GhostCoords[index - reaper_count - 1].Coord}");
                        Player.main.SetPosition(saveCoords.GhostCoords[index - reaper_count - 1].Coord);
                    }
                }
                else
                {
                    ErrorMessage.AddMessage($"Index {index-1} out of bounds!");
                }
            }
        }

        static void PopulateCoordArray()
        {
            //Get a new set of possible coordinates, listed in the SpawnData class, as well as the SpawnIntensity of each leviathan
            SpawnData spawnData = new SpawnData();

            //Set new coord file's spawn intensity to that selected in the config/menu
            saveCoords.ReaperSpawnIntensity = config.ReaperSpawnIntensity;
            saveCoords.GhostSpawnIntensity = config.GhostSpawnIntensity;
            logger.Log(LogLevel.Info, $"Reaper spawn intensity is set to: {saveCoords.ReaperSpawnIntensity}");
            logger.Log(LogLevel.Info, $"Ghost spawn intensity is set to: {saveCoords.GhostSpawnIntensity}");

            logger.Log(LogLevel.Info, $"0/6*count = {(int)(0F / 6 * spawnData.ReaperCoords.Count)}");
            logger.Log(LogLevel.Info, $"1/6*count = {(int)(1F / 6 * spawnData.ReaperCoords.Count)}");
            logger.Log(LogLevel.Info, $"2/6*count = {(int)(2F / 6 * spawnData.ReaperCoords.Count)}");
            logger.Log(LogLevel.Info, $"3/6*count = {(int)(3F / 6 * spawnData.ReaperCoords.Count)}");
            logger.Log(LogLevel.Info, $"4/6*count = {(int)(4F / 6 * spawnData.ReaperCoords.Count)}");
            logger.Log(LogLevel.Info, $"5/6*count = {(int)(5F / 6 * spawnData.ReaperCoords.Count)}");
            logger.Log(LogLevel.Info, $"6/6*count = {(int)(6F / 6 * spawnData.ReaperCoords.Count)}");


            //Determine the amount of coordinates to save, and amount of leviathans to spawn, using the SpawnIntensity of each leviathan
            int reaperSpawnTotal = (int)(saveCoords.ReaperSpawnIntensity / 6 * spawnData.ReaperCoords.Count);
            logger.Log(LogLevel.Info, $"Loading {reaperSpawnTotal} of {spawnData.ReaperCoords.Count} total Reaper spawns");
            int ghostSpawnTotal = (int)(saveCoords.GhostSpawnIntensity / 6 * spawnData.GhostCoords.Count);
            logger.Log(LogLevel.Info, $"Loading {ghostSpawnTotal} of {spawnData.GhostCoords.Count} total Ghost spawns");

            //Create a list for both sets of coords; we will be adding selected coordinates to these lists, for the new coord file
            saveCoords.ReaperCoords = new List<Vector3>();
            saveCoords.GhostCoords = new List<GhostCoordsAndType>();

            //Reseed random number generator; used to generate random selection of coordinates to add to the new coord file
            System.Random rnd = new System.Random();

            //Randomly select reaper coordinates to add to new coord file, amount equal to reaperSpawnTotal
            for (int i = 0; i < reaperSpawnTotal; i++)
            {
                //Select an index of the ReaperCoords list randomly, and add that reaper coordinate to the new coord file
                int j = rnd.Next(0, spawnData.ReaperCoords.Count - 1);

                logger.Log(LogLevel.Info, $"Random selection of Reaper Coord #{j + 1}");
                logger.Log(LogLevel.Info, $"Adding Reaper spawn #{i + 1} - Coords: {spawnData.ReaperCoords[j]}");

                //Add the selected reaper coordinate to the new coord file
                saveCoords.ReaperCoords.Add(spawnData.ReaperCoords[j]);

                //Remove the added reaper coordinate afterwards, to ensure it's not accidentally selected twice
                spawnData.ReaperCoords.RemoveAt(j);
            }

            //Randomly select ghost spawns to add to new save file, amount equal to ghostSpawnTotal
            for (int i = 0; i < ghostSpawnTotal; i++)
            {
                //Select an index of the GhostCoords list randomly, and add that ghost coordinate to the new coord file, alongside its maturity
                int j = rnd.Next(0, spawnData.GhostCoords.Count - 1);

                //Log whether the ghost leviathan added is an adult or a juvenile
                string ghostType = "Adult";
                if (spawnData.GhostCoords[j].GhostType == 2) { ghostType = "Juvenile"; }
                logger.Log(LogLevel.Info, $"Random selection of Ghost {ghostType} Coord #{j + 1}");
                logger.Log(LogLevel.Info, $"Adding Ghost ({ghostType}) spawn #{i + 1} - Coords: {spawnData.GhostCoords[j].Coord}");

                //Add the selected ghost coordinate to the new coord file
                saveCoords.GhostCoords.Add(spawnData.GhostCoords[j]);

                //Remove the added ghost coordinate afterwards, to ensure it's not accidentally selected twice
                spawnData.GhostCoords.RemoveAt(j);
            }
        }
    }

    [Menu("More Leviathan Spawns")]
    public class Config : Nautilus.Json.ConfigFile
    {
        [Slider("Reaper Spawn Intensity", Min = 0, Max = 6, DefaultValue = 3, Step = 1, Id = "ReaperSpawnIntensity", Tooltip = "Defines general intensity of additional reaper leviathan spawns to add to the game. A value of 1 will add roughly 2 - 4 spawns. A value of 6 will add roughly 20 - 23 spawns. A value of 0 will add no additional reaper leviathan spawns to game.")]
        public int ReaperSpawnIntensity = 3;
        [Slider("Ghost Spawn Intensity", Min = 0, Max = 6, DefaultValue = 3, Step = 1, Id = "GhostSpawnIntensity", Tooltip = "Defines general intensity of additional ghost leviathan spawns to add to the game. A value of 1 will add roughly 1 - 3 spawns. A value of 6 will add roughly 12 - 14 spawns. A value of 0 will add no additional ghost leviathan spawns to game.")]
        public int GhostSpawnIntensity = 3;
        //NOTE!! How could this have possible worked, if you can't unspawn leviathans? All this would have done would randomise until all unique spawns were registered
        //In essence, this just eventually hit the max, always.
        //Need to either figure out if leviathans can be unregistered from the world spawn thing, or need to remove this option entirely.
        [Toggle("Always randomize spawns?", Id = "alwaysRandomize", Tooltip = "By default, spawn locations are chosen randomly then saved and remain static for rest of playthrough. If this option is checked, spawns will always randomize when opening that save file.")]
        public bool AlwaysRandomized = false;
    }
}