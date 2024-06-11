using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json.Attributes;
using Nautilus.Json;
using Nautilus.Options.Attributes;
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
        private const string versionString = "3.1.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static ManualLogSource logger { get; private set; }

        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        internal static SaveCoords saveCoords { get; } = SaveDataHandler.RegisterSaveDataCache<SaveCoords>();

        [FileName("leviathans_coords")]
        internal class SaveCoords : SaveDataCache
        {
            public float ReaperSpawnIntensity { get; set; }
            public float GhostSpawnIntensity { get; set; }

            public List<ReaperCoords> ReaperCoords { get; set; }
            public List<GhostCoords> GhostCoords { get; set; }
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

                    //Check if the coord file needs updating
                    if(CoordUpdateNeeded())
                    {
                        UpdateCoords();
                    }
                }

                //DEBUG!! Display values being loaded; check again if coords file is null, due to spawn intensity 0 otherwise causing issues
                if (!(coords.ReaperCoords is null))
                {
                    //ErrorMessage.AddMessage($"Loading Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    logger.LogInfo($"Loading Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    //ErrorMessage.AddMessage($"Loaded {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                    logger.LogInfo($"Loaded {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                }
            };

            saveCoords.OnFinishedSaving += (object sender, JsonFileEventArgs e) =>
            {
                SaveCoords coords = e.Instance as SaveCoords;

                //DEBUG!! Display values being saved; check again if coords file is null, due to spawn intensity 0 otherwise causing issues
                if (!(coords.ReaperCoords is null))
                {
                    //ErrorMessage.AddMessage($"Saving Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    logger.LogInfo($"Saving Reaper Spawn Intensity - {coords.ReaperSpawnIntensity}, Ghost Spawn Intensity - {coords.GhostSpawnIntensity}");
                    //ErrorMessage.AddMessage($"Saved {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                    logger.LogInfo($"Saved {coords.ReaperCoords.Count + coords.GhostCoords.Count} leviathan coords");
                }
            };

            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(LeviathanCommands));
        }

        public static bool ModEnabled()
        {
            //Check first if the mod is enabled (if coord file populated) before running the command
            //If not even reapercoords was populated, neither will ghost, so this shows coord file is empty
            var modEnabled = !(saveCoords.ReaperCoords is null);

            //Display error message if command is attempted without mod being enabled
            if (!modEnabled)
            {
                ErrorMessage.AddError($"Mod is not enabled! Please set Spawn Intensity to above 0 and restart to enable.");
                logger.LogError($"Mod is not enabled! Please set Spawn Intensity to above 0 and restart to enable.");
            }

            return modEnabled;
        }

        public bool CoordUpdateNeeded()
        {
            /*NOTE!! According to Eldritch, "json deserializing creates an instance of a class first, and then sets the fields it found in 
             he json. So if it doesn’t find a field in the json, it just won’t set that field on the instance. So that field will still 
             be whatever the default value is for it"
             What this means is, that whenever I'm checking for a field that shouldn't exist in an older save-file, it will default to
             whatever the default value *currently* is in the relating class and field! So it should be super simple in future to check.
             Also learned that some data types are inherently non-nullable, hence why ListIndex returned 0 by default despite not existing.
             Thus, depending on the data type I check for existance in future, I will need to either check for a default value, 0, or null.*/

            //NOTE!! Essence mentioned using 'int?' could be a good alternative to, making an int nullable to always test for a null value

            //Check for the specific outdated change; if found, the coord file needs to be updated
            //Check for outdated format (no ListIndexes) from version 3.0.0
            bool updateNeeded = (saveCoords.ReaperCoords[0].ListIndex == -1 || saveCoords.GhostCoords[0].ListIndex == -1);

            //Log whether update is needed
            if(updateNeeded)
            {
                logger.LogWarning($"Coord File requires updating from mod version 3.0.0 to {versionString}");
            }
            else
            {
                logger.LogInfo($"Coord File is up to date");
            }

            return updateNeeded;
        }

        //Function to call if I need to update the coords file at all without breaking the user's registered spawns
        //This function will change depending on what differences I'm searching for between the outdated and updated versions of the mod and coord file
        public void UpdateCoords()
        {
            logger.LogWarning($"Updating Coord File for mod version {versionString}");

            //Search for the corresponding reaper coordinate in the updated Coord File structure and match it to the same coordinate in the older one
            //Then, wholy replace it; we're updating ReaperCoord from a list of vectors to a class
            //NOTE!! Adding the check for list index, in case somehow it tries to update twice and breaks everything
            if (saveCoords.ReaperSpawnIntensity != 0 && saveCoords.ReaperCoords[0].ListIndex == -1)
            {
                //Create a new list of ReaperCoords, to replace the old format list
                List<ReaperCoords> newReaperCoords = new List<ReaperCoords>();

                //Create a new spawnData (a fresh list of all coordinates) to replace the older formats with
                SpawnData spawnData = new SpawnData();

                for (int i = 0; i < saveCoords.ReaperCoords.Count; i++)
                {
                    //Obtain the coords of thd old format from saveCoords (saved to the current savefile)
                    ReaperCoords oldReaperCoord = saveCoords.ReaperCoords[i];
                    Vector3 oldCoord = new Vector3(oldReaperCoord.X, oldReaperCoord.Y, oldReaperCoord.Z);

                    //Obtain the coords of the new format from spawnData and add them to the new list
                    for (int j = 0; j < spawnData.ReaperCoords.Count; j++)
                    {
                        Vector3 newCoord = spawnData.ReaperCoords[j].Coord;
                        if (spawnData.ReaperCoords[j].Coord == oldCoord)
                        {
                            logger.LogInfo($"spawnData coord #{j}: {newCoord} matches coord #{i}: {oldCoord}");
                            logger.LogInfo($"ListIndex of our match is {spawnData.ReaperCoords[j].ListIndex}");
                            newReaperCoords.Add(spawnData.ReaperCoords[j]);
                            break; //If we've found our match, stop looping
                        }
                    }
                }

                //Replace ReaperCoords in the coord file with the newly formatted ones
                saveCoords.ReaperCoords = newReaperCoords;
            }

            //Search for the corresponding ghost coordinate in the updated Coord File structure and match it to the same coordinate in the older one
            //Then, get the older GhostCoord and add the ListIndex to it; it's a class, so we can do this simply, as opposed to the replacing we did with the ReaperCoord
            //NOTE!! Adding the check for list index, in case somehow it tries to update twice and breaks everything
            if (saveCoords.GhostSpawnIntensity != 0 && saveCoords.GhostCoords[0].ListIndex == -1)
            {
                logger.LogInfo("Running Ghost Updater");

                //Create a new spawnData (a fresh list of all coordinates) to replace the older formats with
                SpawnData spawnData = new SpawnData();

                for (int i = 0; i < saveCoords.GhostCoords.Count; i++)
                {
                    //Obtain the coords of thd old format from saveCoords (saved to the current savefile)
                    GhostCoords oldGhostCoord = saveCoords.GhostCoords[i];

                    //Obtain the coords of the new format from spawnData and add them to the new list
                    for (int j = 0; j < spawnData.GhostCoords.Count; j++)
                    {
                        Vector3 newCoord = spawnData.GhostCoords[j].Coord;
                        if (spawnData.GhostCoords[j].Coord == oldGhostCoord.Coord)
                        {
                            logger.LogInfo($"spawnData coord #{j}: {newCoord} matches coord #{i}: {oldGhostCoord.Coord}");
                            logger.LogInfo($"ListIndex of our match is {spawnData.GhostCoords[j].ListIndex}");
                            saveCoords.GhostCoords[i].ListIndex = spawnData.GhostCoords[j].ListIndex;
                            break; //If we've found our match, stop looping
                        }
                    }
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
            saveCoords.ReaperCoords = new List<ReaperCoords>();
            saveCoords.GhostCoords = new List<GhostCoords>();

            /*Reseed random number generator; used to add or subtract from the amount of spawns if variable spawns is enabled 
             and generate random selection of coordinates to add to the new coord file*/
            System.Random rnd = new System.Random();

            //If variable spawns have been selected determine random amount to add or subtract from the amount (could still be 0 difference)
            if (config.AddVariableSpawns)
            {
                var reaperIntensity = saveCoords.ReaperSpawnIntensity;
                var ghostIntensity = saveCoords.GhostSpawnIntensity;

                //Only account for variable spawns if the spawns are neither 0 nor set to max
                if (reaperIntensity != 0 && reaperIntensity != 6)
                {
                    int varReaperSpawns = rnd.Next(-2, 2);
                    logger.LogInfo($"Reaper Spawn Total of {reaperSpawnTotal} varied by {varReaperSpawns}");
                    reaperSpawnTotal += varReaperSpawns;
                }

                if (ghostIntensity != 0 && ghostIntensity != 6)
                {
                    int var_ghost_spawns = rnd.Next(-2, 2);
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

                logger.LogInfo($"Adding Reaper spawn #{i + 1} (Index {j + 1}) - Coords: {spawnData.ReaperCoords[j].Coord}");

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
                logger.LogInfo($"Adding Ghost ({ghostType}) spawn #{i + 1} (Index {j + 1}) - Coords: {spawnData.GhostCoords[j].Coord}");

                //Add the selected ghost coordinate to the new coord file
                saveCoords.GhostCoords.Add(spawnData.GhostCoords[j]);

                //Remove the added ghost coordinate afterwards, to ensure it's not accidentally selected twice
                spawnData.GhostCoords.RemoveAt(j);
            }
        }

        //Textual list of reaper locations, sorted the same as the coords in SpawnData
        public static readonly List<string> ReaperLocations = new List<string>
        {
            "Grassy Plateaus, South",
            "Bulb Zone, East-North-East",
            "Mountains, North",
            "Grand Reef, South-South-West",
            "Grand Reef, South",
            "Kelp Forest, West",
            "Kelp Forest, North",
            "Mushroom Forest, East",
            "Mushroom Forest, East",
            "Crag Field, South",
            "Sparse Reef, South-West",
            "Grand Reef, South-South-West (Under Floating Island)",
            "Kelp Forest, South",
            "Grassy Plateaus, South-West",
            "Mushroom Forest, North-West",
            "Mushroom Forest, West-North-West",
            "Grassy Plateaus, ",
            "Underwater Islands, North",
            "Mushroom Forest, North-West",
            "Sea Treader's Path, West-South-West",
            "Blood Kelp Zone, North-North-West",
            "Kelp Forest, North-North-East",
            "Grassy Plateaus, East"
        };

        //Textual list of ghost locations, sorted the same as the coords in SpawnData
        public static readonly List<string> GhostLocations = new List<string>
        {
            "Underwater Islands, North",
            "Bulb Zone, East-North-East",
            "Bulb Zone, North-East",
            "Mountains, North",
            "Jellyshroom Cave, South",
            "Grand Reef, South",
            "Blood Kelp Trench, South-West",
            "Lost River, West-North-West (Tree Cove)",
            "Grand Reef, South-South-West (Degasi Base)",
            "Jellyshroom Cave, West",
            "Underwater Islands, North",
            "Underwater Islands, North",
            "Bulb Zone, North-East",
            "Mountains, North-East (Lost River Entrance)"
        };
    }

    [Menu("More Leviathan Spawns")]
    public class Config : ConfigFile
    {
        //Intensity Table, showing how much each intensity rounds to when spawning leviathans
        //Reaper Spawn Intensity - 1=3, 2=7, 3=11, 4=15, 5=19, 6=23
        //Ghost Spawn Intensity - 1=2, 2=4, 3=7, 4=9, 5=11, 6=14

        [Slider("Reaper Spawn Intensity", Min = 0, Max = 6, DefaultValue = 2, Step = 1, Id = "ReaperSpawnIntensity", Tooltip = "Defines intensity of additional reaper leviathan spawns to add to the game. A value of 1 will add 3 spawns. A value of 6 will add 23 spawns. A value of 0 will add no additional reaper leviathans spawns to game.")]
        public int ReaperSpawnIntensity = 2;
        [Slider("Ghost Spawn Intensity", Min = 0, Max = 6, DefaultValue = 2, Step = 1, Id = "GhostSpawnIntensity", Tooltip = "Defines intensity of additional ghost leviathan spawns to add to the game. A value of 1 will add roughly 2 spawns. A value of 6 will add roughly 14 spawns. A value of 0 will add no additional ghost leviathans spawns to game.")]
        public int GhostSpawnIntensity = 2;
        [Toggle("Add variable spawns?", Id = "AddVariableSpawns", Tooltip = "By default, a static amount of spawns will be added. By selecting this, that amount could be anywhere from 2 more to 2 less than expected, adding some variability on creation.")]
        public bool AddVariableSpawns = false;
    }
}