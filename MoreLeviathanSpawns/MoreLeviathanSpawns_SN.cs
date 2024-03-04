using HarmonyLib;
//using System;
//using System.IO;
using BepInEx.Logging;
using Nautilus.Json;
using Nautilus.Handlers;
using static MoreLeviathanSpawns.MoreLeviathanSpawnsPlugin_SN;
using System.Collections.Generic;
//using System.Xml.Serialization;

namespace MoreLeviathanSpawns
{
    [HarmonyPatch]
    internal class SpawnMoreLeviathans_SN
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            // Check to see if this is the player
            if (__instance.GetType() == typeof(Player))
            {
                PopulateCoordArray();
            }
        }

        #region CreateXMLFile
        /*static void CreateXMLFile(string filepath)
        {
            //SpawnData spawnData = new SpawnData();

            //NOTE!! This way might work a little faster? A little neater? Test!
            SpawnData spawnData = new SpawnData
            {
                //Get values from whatever the player selected from the mod menu
                AlwaysRandomized = config.AlwaysRandomized,
                ReaperSpawnIntensity = config.ReaperSpawnIntensity,
                GhostSpawnIntensity = config.GhostSpawnIntensity
            };

            //Shuffle 2D array of coordinates. Depending on how many spawns the player wants in the game (set via
            //the in-game mod menu), spawn in that many creatures starting from the top of the 2D array.

            Shuffle(spawnData.ReaperCoords);
            Shuffle(spawnData.GhostCoordsAndType);

            //create the XML file
            XmlSerializer writer = new XmlSerializer(typeof(SpawnData));
            FileStream file = File.Create(filepath);
            writer.Serialize(file, spawnData);
            file.Close();
            logger.Log(LogLevel.Info, $"xml file path: {filepath}");
        }*/
        #endregion

        //NOTE!! File is no longer required for this mod, meaning we can ditch the file serializer and thus rmeove the unnecassary file creation in TempData
        static void PopulateCoordArray()
        {
            SpawnData spawnData = new SpawnData
            {
                //AlwaysRandomized = config.AlwaysRandomized;
                ReaperSpawnIntensity = config.ReaperSpawnIntensity,
                GhostSpawnIntensity = config.GhostSpawnIntensity
            };

            logger.Log(LogLevel.Info, $"Reaper spawn intensity is set to: {spawnData.ReaperSpawnIntensity}");
            logger.Log(LogLevel.Info, $"Ghost spawn intensity is set to: {spawnData.GhostSpawnIntensity}");

            //this will set a general amount of spawns based on the spawn intensity the player set, defaulting to '3'
            int reaperSpawnTotal = (int)(spawnData.ReaperSpawnIntensity / 6 * spawnData.ReaperCoords.Count);
            int ghostSpawnTotal = (int)(spawnData.GhostSpawnIntensity / 6 * spawnData.GhostCoords.Count);

            logger.Log(LogLevel.Info, $"Loading {reaperSpawnTotal} of {spawnData.ReaperCoords.Count} total reaper spawns");
            logger.Log(LogLevel.Info, $"Loading {ghostSpawnTotal} of {spawnData.GhostCoords.Count} total ghost spawns");

            //Create a list for both sets of coords; we will be adding selected spawns to these lists, for the new save file
            saveCoords.ReaperCoords = new List<UnityEngine.Vector3>();
            saveCoords.GhostCoords = new List<GhostCoordsAndType>();

            //Reseed; used to generate random selection of spawns to add to the new save file
            System.Random rnd = new System.Random();

            //Randomly select reaper spawns to add to new save file, amount equal to reaperSpawnTotal
            for (int i = 0; i < reaperSpawnTotal; i++)
            {
                //Select an index of the ReaperCoords list randomly, and add that reaper coord to the new save file
                int j = rnd.Next(0, spawnData.ReaperCoords.Count - 1);
                logger.Log(LogLevel.Info, $"Random selection of Reaper Coord #{j + 1}");
                logger.Log(LogLevel.Info, $"Reaper spawn #{i + 1} - Coords: {spawnData.ReaperCoords[j]}");

                //Add the selected coordinates to the list of coordinates for the new save file
                saveCoords.ReaperCoords.Add(spawnData.ReaperCoords[j]);

                //Remove the added reaper coord afterwards, to ensure it's not accidentally selected twice
                spawnData.ReaperCoords.RemoveAt(j);
            }

            //Randomly select ghost spawns to add to new save file, amount equal to ghostSpawnTotal
            for (int i = 0; i < ghostSpawnTotal; i++)
            {
                //DEBUG!! Log whether the ghost leviathan added is an adult or a juvenile
                string ghostType = "Adult";
                if (spawnData.GhostCoords[i].GhostType == 2) { ghostType = "Juvenile"; }

                //Select an index of the GhostCoords list randomly, and add that ghost coord to the new save file
                int j = rnd.Next(0, spawnData.GhostCoords.Count - 1);
                logger.Log(LogLevel.Info, $"Random selection of Ghost {ghostType} Coord #{j + 1}");
                logger.Log(LogLevel.Info, $"Ghost ({ghostType}) spawn #{i + 1} - Coords: {spawnData.GhostCoords[j].Coords}");

                //Add the selected coordinates to the list of coordinates for the new save file
                saveCoords.GhostCoords.Add(spawnData.GhostCoords[j]);

                //Remove the added ghost coord afterwards, to ensure it's not accidentally selected twice
                spawnData.GhostCoords.RemoveAt(j);
            }
        }
    }

    public class GhostCoordsAndType
    {
        public UnityEngine.Vector3 Coords { get; set; }
        //1 for "Adult", 2 for "Juvenile"
        public int GhostType { get; set; }
    }

    #region SpawnData
    //NOTE!! Given these are static, could make it something other than an entire class?
    public class SpawnData
    {
        public bool AlwaysRandomized = false;
        public float ReaperSpawnIntensity = 3;
        public float GhostSpawnIntensity = 3;
        //23 Reaper Coordinates
        public List<UnityEngine.Vector3> ReaperCoords = new List<UnityEngine.Vector3>
        {
            new UnityEngine.Vector3( 120, -40, -568 ), //Grassy Plateaus, South
            new UnityEngine.Vector3( 1407, -190, 584 ), //Bulb Zone, East-North-East
            new UnityEngine.Vector3( 278, -175, 1398 ), //Mountains, North
            new UnityEngine.Vector3( -811, -240, -1240 ), //Grand Reef, South-South-West
            new UnityEngine.Vector3( -442, -132, -912 ), //Grand Reef, South
            new UnityEngine.Vector3( -310, -45, 92 ), //Kelp Forest, West
            new UnityEngine.Vector3( 190, -52, 477 ), //Kelp Forest, North
            new UnityEngine.Vector3( 500, -98, 318 ), //Mushroom Forest, East
            new UnityEngine.Vector3( 680, -85, 331 ), //Mushroom Forest, East
            new UnityEngine.Vector3( -172, -70, -781 ), //Crag Field, South
            new UnityEngine.Vector3( -692, -130, -725 ), //Sparse Reef, South-West
            new UnityEngine.Vector3( -745, -80, -1050 ), //Grand Reef, South-South-West (Under Floating Island)
            new UnityEngine.Vector3( -278, -30, -621 ), //Kelp Forest, South
            new UnityEngine.Vector3( -295, -45, -350 ), //Grassy Plateaus, South-West
            new UnityEngine.Vector3( -516, -110, 531 ), //Mushroom Forest, North-West
            new UnityEngine.Vector3( -815, -68, 316 ), //Mushroom Forest, West-North-West
            new UnityEngine.Vector3( -531, -60, -175 ), //Grassy Plateaus, West
            new UnityEngine.Vector3( -250, -142, 906 ), //Underwater Islands, North
            new UnityEngine.Vector3( -1122, -113, 710 ), //Mushroom Forest, North-West
            new UnityEngine.Vector3( -1190, -102, -527 ), //Sea Treader's Path, West-South-West
            new UnityEngine.Vector3( -754, -102, 1334 ), //Blood Kelp Zone, North-North-West
            new UnityEngine.Vector3( 432, -65, 690 ), //Kelp Forest, North-North-East
            new UnityEngine.Vector3( 383, -60, 40 ) //Grassy Plateaus, East
        };
        //14 Ghost Coordinates
        public List<GhostCoordsAndType> GhostCoords = new List<GhostCoordsAndType>
        {
            //NOTE!! Should I be using "new int 1"?
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -284, -293, 1100 ), GhostType = 1 }, //Adult, Underwater Islands, North
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( 1065, -211, 466 ), GhostType = 1 }, //Adult, Bulb Zone, East-North-East
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( 876, -122, 881 ), GhostType = 1 }, //Adult, Bulb Zone, North-East
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -28, -318, 1296 ), GhostType = 1 }, //Adult, Mountains, North
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( 10, -219, -220 ), GhostType = 2 }, //Juvenile, Jellyshroom Cave, South
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -396, -350, -925 ), GhostType = 2 }, //Juvenile, Grand Reef, South
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -958, -300, -540 ), GhostType = 2 }, //Juvenile, Blood Kelp Trench, South-West
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -988, -885, 400 ), GhostType = 2 }, //Juvenile, Lost River, West-North-West (Tree Cove)
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -695, -478, -993 ), GhostType = 2 }, //Juvenile, Grand Reef, South-South-West (Degasi Base)
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -618, -213, -82 ), GhostType = 2 }, //Juvenile, Jellyshroom Cave, West
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -34, -400, 926 ), GhostType = 2 }, //Juvenile, Underwater Islands, North
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( -196, -436, 1056 ), GhostType = 2 }, //Juvenile, Underwater Islands, North
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( 1443, -260, 883 ), GhostType = 2 }, //Juvenile, Bulb Zone, North-East
            new GhostCoordsAndType { Coords = new UnityEngine.Vector3( 1075, -475, 944 ), GhostType = 2 } //Juvenile, Mountains, North-East (Lost River Entrance)
        };
    }
    #endregion
}
