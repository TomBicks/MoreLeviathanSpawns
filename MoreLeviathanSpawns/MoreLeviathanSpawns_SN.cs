using HarmonyLib;
//using System;
//using System.IO;
using BepInEx.Logging;
using Nautilus.Json;
using Nautilus.Handlers;
using static MoreLeviathanSpawns.MoreLeviathanSpawnsPlugin_SN;
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
                /*string filepath = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "spawnData.xml");
                logger.Log(LogLevel.Info, "Save file location" + SaveLoadManager.GetTemporarySavePath());

                if (!File.Exists(filepath))
                {
                    logger.Log(LogLevel.Info, "MoreLeviathanSpawns initialised for the first time. Creating XML file...");
                    CreateXMLFile(filepath);
                }
                else
                {
                    logger.Log(LogLevel.Info, "XML file found. Loading spawns from file...");
                }*/
                SpawnCreatures();

                //saveCoords.Save();
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
        static void SpawnCreatures()
        {
            SpawnData spawnData = new SpawnData();
            //spawnData.AlwaysRandomized = config.AlwaysRandomized;
            spawnData.ReaperSpawnIntensity = config.ReaperSpawnIntensity;
            spawnData.GhostSpawnIntensity = config.GhostSpawnIntensity;

            logger.Log(LogLevel.Info, $"Reaper spawn intensity is set to: {spawnData.ReaperSpawnIntensity}");
            logger.Log(LogLevel.Info, $"Ghost spawn intensity is set to: {spawnData.GhostSpawnIntensity}");

            //this will set a general amount of spawns based on the spawn intensity the player set, defaulting to '3'
            int reaperSpawnTotal = (int)(spawnData.ReaperSpawnIntensity / 6 * spawnData.ReaperCoords.Length);
            int ghostSpawnTotal = (int)(spawnData.GhostSpawnIntensity / 6 * spawnData.GhostCoordsAndType.Length);

            logger.Log(LogLevel.Info, $"Loading {reaperSpawnTotal} of {spawnData.ReaperCoords.Length} total reaper spawns");
            logger.Log(LogLevel.Info, $"Loading {ghostSpawnTotal} of {spawnData.GhostCoordsAndType.Length} total ghost spawns");

            //if player opted for spanws to always be random, simply shuffle the spawn coordinates 2D array,
            //defaulting to 'false'
            /*logger.Log(LogLevel.Info, $"Alway randomized is set to: {spawnData.AlwaysRandomized}");
            if (spawnData.AlwaysRandomized)
            {
                logger.Log(LogLevel.Info, $"shuffling (randomizing) spawns...");
                Shuffle(spawnData.ReaperCoords);
                Shuffle(spawnData.GhostCoordsAndType);
            }*/

            //load reaper spawns
            for(int i = 0; i < reaperSpawnTotal; i++)
            {
                logger.Log(LogLevel.Info, $"Reaper spawn #{i + 1} - Coords: {spawnData.ReaperCoords[i][0]} {spawnData.ReaperCoords[i][1]} {spawnData.ReaperCoords[i][2]}");
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(spawnData.ReaperCoords[i][0], spawnData.ReaperCoords[i][1], spawnData.ReaperCoords[i][2])));
                
            }

            //load ghost spawns
            for (int i = 0; i < ghostSpawnTotal; i++)
            {
                TechType creatureType = new TechType();
                string ghostType = "Adult";
                switch (spawnData.GhostCoordsAndType[i][3])
                {
                    case 1://Ghost (Adult)
                        creatureType = TechType.GhostLeviathan;
                        break;
                    case 2://Ghost (Juvenile)
                        creatureType = TechType.GhostLeviathanJuvenile;
                        ghostType = "Juvenile";
                        break;
                }
                logger.Log(LogLevel.Info, $"Ghost ({ghostType}) spawn #{i + 1} - Coords: {spawnData.GhostCoordsAndType[i][0]} {spawnData.GhostCoordsAndType[i][1]} {spawnData.GhostCoordsAndType[i][2]}");
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(creatureType, new UnityEngine.Vector3(spawnData.GhostCoordsAndType[i][0], spawnData.GhostCoordsAndType[i][1], spawnData.GhostCoordsAndType[i][2])));
            }
        }

        public static void Shuffle(float[][] arr)
        {
            System.Random rnd = new System.Random();
            for (int i = arr.Length - 1; i >= 1; i--)
            {
                // Random.Next generates numbers between min and max - 1 value, so we have to balance this
                int j = rnd.Next(0, i + 1);

                if (i != j)
                {
                    var temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
        }
    }

    #region SpawnData
    //[Serializable]
    //Given these are static, could make it something other than an entire class?
    public class SpawnData
    {
        public bool AlwaysRandomized = false;
        public float ReaperSpawnIntensity = 3;
        public float GhostSpawnIntensity = 3;
        //23 Reaper Coordinates
        public float[][] ReaperCoords =
        {
            new float[]{ 120, -40, -568 }, //Grassy Plateaus, South
            new float[]{ 1407, -190, 584 }, //Bulb Zone, East-North-East
            new float[]{ 278, -175, 1398 }, //Mountains, North
            new float[]{ -811, -240, -1240 }, //Grand Reef, South-South-West
            new float[]{ -442, -132, -912 }, //Grand Reef, South
            new float[]{ -310, -45, 92 }, //Kelp Forest, West
            new float[]{ 190, -52, 477 }, //Kelp Forest, North
            new float[]{ 500, -98, 318 }, //Mushroom Forest, East
            new float[]{ 680, -85, 331 }, //Mushroom Forest, East
            new float[]{ -172, -70, -781 }, //Crag Field, South
            new float[]{ -692, -130, -725 }, //Sparse Reef, South-West
            new float[]{ -745, -80, -1050 }, //Grand Reef, South-South-West (Under Floating Island)
            new float[]{ -278, -30, -621 }, //Kelp Forest, South
            new float[]{ -295, -45, -350 }, //Grassy Plateaus, South-West
            new float[]{ -516, -110, 531 }, //Mushroom Forest, North-West
            new float[]{ -815, -68, 316 }, //Mushroom Forest, West-North-West
            new float[]{ -531, -60, -175 }, //Grassy Plateaus, West
            new float[]{ -250, -142, 906 }, //Underwater Islands, North
            new float[]{ -1122, -113, 710 }, //Mushroom Forest, North-West
            new float[]{ -1190, -102, -527 }, //Sea Treader's Path, West-South-West
            new float[]{ -754, -102, 1334 }, //Blood Kelp Zone, North-North-West
            new float[]{ 432, -65, 690 }, //Kelp Forest, North-North-East
            new float[]{ 383, -60, 40 } //Grassy Plateaus, East
        };
        //14 Ghost Coordinates
        public float[][] GhostCoordsAndType =
        {
            new float[]{ -284, -293, 1100, 1 }, //Adult, Underwater Islands, North
            new float[]{ 1065, -211, 466, 1 }, //Adult, Bulb Zone, East-North-East
            new float[]{ 876, -122, 881, 1 }, //Adult, Bulb Zone, North-East
            new float[]{ -28, -318, 1296, 1 }, //Adult, Mountains, North
            new float[]{ 10, -219, -220, 2 }, //Juvenile, Jellyshroom Cave, South
            new float[]{ -396, -350, -925, 2 }, //Juvenile, Grand Reef, South
            new float[]{ -958, -300, -540, 2 }, //Juvenile, Blood Kelp Trench, South-West
            new float[]{ -988, -885, 400, 2 }, //Juvenile, Lost River, West-North-West (Tree Cove)
            new float[]{ -695, -478, -993, 2 }, //Juvenile, Grand Reef, South-South-West (Degasi Base)
            new float[]{ -618, -213, -82, 2 }, //Juvenile, Jellyshroom Cave, West
            new float[]{ -34, -400, 926, 2 }, //Juvenile, Underwater Islands, North
            new float[]{ -196, -436, 1056, 2 }, //Juvenile, Underwater Islands, North
            new float[]{ 1443, -260, 883, 2 }, //Juvenile, Bulb Zone, North-East
            new float[]{ 1075, -475, 944, 2 } //Juvenile, Mountains, North-East (Lost River Entrance)
        };
    }
    #endregion
}
