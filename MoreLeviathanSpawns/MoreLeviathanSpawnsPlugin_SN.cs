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


            /* --- NOTES REGARDING SAVING, LOADING, & CREATING AND CHANGING SAVE FILES ---
             - When creating a save, using OnFinishedLoading, it works perfectly fine, and restarting the game and then
               loading this same save, even with different intensity settings, works perfectly fine too
             
             - Creating/Loading a save, then creating/loading another one, will *say* it's loaded the correct intensity settings;
               whilst it technically has, because of CoordinatesSpawns spillover, registering spawns in one session will register 
               them in any save loaded. Quitting first before loading the save will probably work.
             - The spillover might be fixed by not re-registering the saves each time, but that's also liable to cause the mod
               to fail, so best-case I'll likely just have to include a warning with the mod until Nautilus fixes this.
            
             - Quitting the game, then loading an already created save, loads the correct settings and spawns. Quitting and then
               loading a different save also works, so this solution is repeatable for different saves with different settings.

             - Loading a save, then saving with different settings to the one the save files has, then reloading the save, does
               *not* cause any more leviathans to spawn. Quitting after saving, then loading has no effect either. Still the only
               issue is spillover.
            
             - Once a leviathan has been spawned in (so possibly if it hasn't been loaded its fine), and the game has been saved,
               that spawn will likely be permanent, regardless if the user wants it to be or not. So removing the mod will not fix.*/


            /*OnFinishedLoading, check whether the coord file has null values;
             - if it does, it means the save and coord files are being created for the first time and that we need
               to populate the coord file with a random selection of leviathan coordinates, based on the intensity selected
             - if it doesn't have null values, it means this is not the first time the save has been loaded and we can skip
               the process of populating the coord file with leviathan coordinates*/
            saveCoords.OnFinishedLoading += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;

                logger.LogInfo($"Loading from filepath: {coords.JsonFilePath}");

                if(coords.ReaperCoords is null)
                {
                    //If both spawn intensities are set to 0, user doesn't want leviathans spawned on this new save file
                    if(config.ReaperSpawnIntensity == 0 && config.GhostSpawnIntensity == 0)
                    {
                        logger.LogInfo("Coord File is null, but Spawn Intensity is 0. Ignoring adding leviathan spawns.");
                    }
                    else
                    {
                        //Loading the save for the first time and creating the coord file for the first time
                        logger.LogInfo("Coord File is null! Preparing to populate Leviathan Coords file...");
                        PopulateCoordArray();

                        //Now that the SaveDataCache has all the proper values assigned for this save, properly save the new coord file and its values
                        saveCoords.Save();
                    }
                }
                else
                {
                    logger.LogInfo("Coord File is already populated!");
                }

                //DEBUG!! Display values being loaded; check again if coords file is null, due to spawn intensity 0 otherwise causing issues
                if (!(coords.ReaperCoords is null))
                {
                    ErrorMessage.AddMessage($"Loading Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    ErrorMessage.AddMessage($"Loaded {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                    logger.LogInfo($"Loading Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    logger.LogInfo($"Loaded {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                }
            };

            saveCoords.OnFinishedSaving += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;

                //DEBUG!! Display values being saved; check again if coords file is null, due to spawn intensity 0 otherwise causing issues
                if (!(coords.ReaperCoords is null))
                {
                    ErrorMessage.AddMessage($"Saving Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    ErrorMessage.AddMessage($"Saved {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                    logger.LogInfo($"Saving Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    logger.LogInfo($"Saved {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                }
            };

            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(LeviathanCommands));
        }

        public static class LeviathanCommands
        {
            [ConsoleCommand("coordwarp")]
            public static void CoordWarp(int index)
            {
                //Check first if the mod is enabled (spawn intensity > 0 and coord file populated) before running the command
                if (!(saveCoords.ReaperCoords is null))
                {
                    int reaper_count = saveCoords.ReaperCoords.Count;
                    int ghost_count = saveCoords.GhostCoords.Count;

                    if (index >= 1 && index <= (reaper_count + ghost_count))
                    {
                        if (index <= reaper_count)
                        {
                            ErrorMessage.AddMessage($"Teleporting to Reaper coord #{index} - Coords: {saveCoords.ReaperCoords[index - 1]}");
                            logger.LogInfo($"Teleporting to Reaper coord #{index} - Coords: {saveCoords.ReaperCoords[index - 1]}");
                            Player.main.SetPosition(saveCoords.ReaperCoords[index - 1]);
                        }
                        else
                        {
                            ErrorMessage.AddMessage($"Teleporting to Ghost coord #{index - reaper_count} - Coords: {saveCoords.GhostCoords[index - reaper_count - 1].Coord}");
                            logger.LogInfo($"Teleporting to Ghost coord #{index - reaper_count} - Coords: {saveCoords.GhostCoords[index - reaper_count - 1].Coord}");
                            Player.main.SetPosition(saveCoords.GhostCoords[index - reaper_count - 1].Coord);
                        }
                    }
                    else
                    {
                        ErrorMessage.AddMessage($"Index {index} out of bounds!");
                        logger.LogError($"Index {index} out of bounds!");
                    }
                }
                else
                {
                    ErrorMessage.AddMessage($"Mod is not enabled! Please set Spawn Intensity to above 0 and restart to enable.");
                    logger.LogError($"Mod is not enabled! Please set Spawn Intensity to above 0 and restart to enable.");
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
            logger.LogInfo($"Reaper spawn intensity is set to: {saveCoords.ReaperSpawnIntensity}");
            logger.LogInfo($"Ghost spawn intensity is set to: {saveCoords.GhostSpawnIntensity}");

            //Determine the amount of coordinates to save, and amount of leviathans to spawn, using the SpawnIntensity of each leviathan
            int reaperSpawnTotal = (int)(saveCoords.ReaperSpawnIntensity / 6 * spawnData.ReaperCoords.Count);
            int ghostSpawnTotal = (int)(saveCoords.GhostSpawnIntensity / 6 * spawnData.GhostCoords.Count);

            //Create a list for both sets of coords; we will be adding selected coordinates to these lists, for the new coord file
            saveCoords.ReaperCoords = new List<Vector3>();
            saveCoords.GhostCoords = new List<GhostCoordsAndType>();

            /*Reseed random number generator; used to add or subtract from the amount of spawns if variable spawns is enabled 
             and generate random selection of coordinates to add to the new coord file*/
            System.Random rnd = new System.Random();

            //If variable spawns have been selected determine random amount to add or subtract from the amount (could still be 0 difference)
            if (config.AddVariableSpawns)
            {
                var reaper_intensity = saveCoords.ReaperSpawnIntensity;
                var ghost_intensity = saveCoords.GhostSpawnIntensity;
                //Only do this if the spawns are neither 0 nor set to max
                int var_reaper_spawns = rnd.Next(-2, 2);
                int var_ghost_spawns = rnd.Next(-2, 2);

                if (reaper_intensity != 0 && reaper_intensity != 6)
                {
                    logger.LogInfo($"Reaper Spawn Total of {reaperSpawnTotal} varied by {var_reaper_spawns}");
                    reaperSpawnTotal += var_reaper_spawns;
                }

                if (ghost_intensity != 0 && ghost_intensity != 6)
                {
                    logger.LogInfo($"Ghost Spawn Total of {ghostSpawnTotal} varied by {var_ghost_spawns}");
                    ghostSpawnTotal += var_ghost_spawns;
                }
            }

            //Display amount of coords to be loaded
            logger.LogInfo($"Loading {reaperSpawnTotal} of {spawnData.ReaperCoords.Count} total Reaper spawns");
            logger.LogInfo($"Loading {ghostSpawnTotal} of {spawnData.GhostCoords.Count} total Ghost spawns");

            //Randomly select reaper coordinates to add to new coord file, amount equal to reaperSpawnTotal
            for (int i = 0; i < reaperSpawnTotal; i++)
            {
                //Select an index of the ReaperCoords list randomly, and add that reaper coordinate to the new coord file
                int j = rnd.Next(0, spawnData.ReaperCoords.Count - 1);

                logger.LogInfo($"Random selection of Reaper Coord #{j + 1}");
                logger.LogInfo($"Adding Reaper spawn #{i + 1} - Coords: {spawnData.ReaperCoords[j]}");

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
                logger.LogInfo($"Random selection of Ghost {ghostType} Coord #{j + 1}");
                logger.LogInfo($"Adding Ghost ({ghostType}) spawn #{i + 1} - Coords: {spawnData.GhostCoords[j].Coord}");

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
        //Intensity Table, showing how much each intensity rounds to when spawning leviathans
        //Reaper Spawn Intensity - 1=3, 2=7, 3=11, 4=15, 5=19, 6=23
        //Ghost Spawn Intensity - 1=2, 2=4, 3=7, 4=9, 5=11, 6=14

        [Slider("Reaper Spawn Intensity", Min = 0, Max = 6, DefaultValue = 3, Step = 1, Id = "ReaperSpawnIntensity", Tooltip = "Defines intensity of additional reaper leviathan spawns to add to the game. A value of 1 will add 3 spawns. A value of 6 will add 23 spawns. A value of 0 will add no additional reaper leviathans spawns to game.")]
        public int ReaperSpawnIntensity = 3;
        [Slider("Ghost Spawn Intensity", Min = 0, Max = 6, DefaultValue = 3, Step = 1, Id = "GhostSpawnIntensity", Tooltip = "Defines intensity of additional ghost leviathan spawns to add to the game. A value of 1 will add roughly 2 spawns. A value of 6 will add roughly 14 spawns. A value of 0 will add no additional ghost leviathans spawns to game.")]
        public int GhostSpawnIntensity = 3;
        [Toggle("Add variable spawns?", Id = "AddVariableSpawns", Tooltip = "By default, a static amount of spawns will be added. By selecting this, that amount could be anywhere from 2 more to 2 less than expected, adding some variability on creation.")]
        public bool AddVariableSpawns = false;
    }
}