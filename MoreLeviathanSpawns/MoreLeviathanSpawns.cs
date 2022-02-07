using HarmonyLib;
using SMLHelper.V2.Handlers;
using System;
using System.IO;
using System.Xml.Serialization;
using Logger = QModManager.Utility.Logger;

namespace MoreLeviathanSpawns
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake")]
    internal class SpawnMoreLeviathans
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            // Check to see if this is the player
            if (__instance.GetType() == typeof(Player))
            {
                string filepath = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "spawnData.xml");
                Logger.Log(Logger.Level.Info, "Save file location" + SaveLoadManager.GetTemporarySavePath());

                if (!File.Exists(filepath))
                {
                    Logger.Log(Logger.Level.Info, "MoreLeviathanSpawns initialised for the first time. Creating XML file...");
                    CreateXMLFile(filepath);
                } else
                {
                    Logger.Log(Logger.Level.Info, "XML file found. Loading spawns from file...");
                }
                SpawnCreatures(filepath);
            }
        }

        static void CreateXMLFile(string filepath)
        {
            SpawnData spawnData = new SpawnData();

            //Get values from whatever the player selected from the mod menu
            spawnData.AlwaysRandomized = QMod.Config.AlwaysRandomized;
            spawnData.ReaperSpawnIntensity = QMod.Config.ReaperSpawnIntensity;
            spawnData.GhostSpawnIntensity = QMod.Config.GhostSpawnIntensity;

            //Shuffle 2D array of coordinates. Depending on how many spawns the player wants in the game (set via
            //the in-game mod menu), spawn in that many creatures starting from the top of the 2D array.
            Shuffle(new Random(), spawnData.ReaperCoords);
            Shuffle(new Random(), spawnData.GhostCoordsAndType);

            //create the XML file
            XmlSerializer writer = new XmlSerializer(typeof(SpawnData));
            FileStream file = File.Create(filepath);
            writer.Serialize(file, spawnData);
            file.Close();
            Logger.Log(Logger.Level.Info, $"xml file path: {filepath}");
        }

        static void SpawnCreatures(string filepath)
        {
            XmlSerializer reader = new XmlSerializer(typeof(SpawnData));
            StreamReader file = new StreamReader(filepath);
            SpawnData spawnData = (SpawnData) reader.Deserialize(file);
            file.Close();

            Logger.Log(Logger.Level.Info, $"Reaper spawn intensity is set to: {spawnData.ReaperSpawnIntensity}");
            Logger.Log(Logger.Level.Info, $"Ghost spawn intensity is set to: {spawnData.GhostSpawnIntensity}");
            
            //this will set a general amount of spawns based on the spawn intensity the player set, defaulting to '1'
            int reaperSpawnTotal = (int)(spawnData.ReaperCoords.Length / 3 * spawnData.ReaperSpawnIntensity);
            int ghostSpawnTotal = (int)(spawnData.GhostCoordsAndType.Length / 3 * spawnData.GhostSpawnIntensity);

            Logger.Log(Logger.Level.Info, $"Loading {reaperSpawnTotal} of {spawnData.ReaperCoords.Length} total reaper spawns");
            Logger.Log(Logger.Level.Info, $"Loading {ghostSpawnTotal} of {spawnData.GhostCoordsAndType.Length} total ghost spawns");

            //if player opted for spanws to always be random, simply shuffle the spawn coordinates 2D array,
            //defaulting to 'false'
            Logger.Log(Logger.Level.Info, $"Alway randomized is set to: {spawnData.AlwaysRandomized}");
            if (spawnData.AlwaysRandomized)
            {
                Logger.Log(Logger.Level.Info, $"shuffling (randomizing) spawns...");
                Shuffle(new Random(), spawnData.ReaperCoords);
                Shuffle(new Random(), spawnData.GhostCoordsAndType);
            }

            //load reaper spawns
            for(int i = 0; i < reaperSpawnTotal; i++)
            {
                Logger.Log(Logger.Level.Info, $"Reaper spawn #{i + 1} - Coords: {spawnData.ReaperCoords[i][0]} {spawnData.ReaperCoords[i][1]} {spawnData.ReaperCoords[i][2]}");
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(spawnData.ReaperCoords[i][0], spawnData.ReaperCoords[i][1], spawnData.ReaperCoords[i][2])));
            }

            //load ghost spawns
            for (int i = 0; i < ghostSpawnTotal; i++)
            {
                TechType creatureType = new TechType();
                string ghostType = "Adult";
                switch (spawnData.GhostCoordsAndType[i][3])
                {
                    case 2://Ghost (Adult)
                        creatureType = TechType.GhostLeviathan;
                        break;
                    case 3://Ghost (Juvenile)
                        creatureType = TechType.GhostLeviathanJuvenile;
                        ghostType = "Juvenile";
                        break;
                }
                Logger.Log(Logger.Level.Info, $"Ghost ({ghostType}) spawn #{i + 1} - Coords: {spawnData.GhostCoordsAndType[i][0]} {spawnData.GhostCoordsAndType[i][1]} {spawnData.GhostCoordsAndType[i][2]}");
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(creatureType, new UnityEngine.Vector3(spawnData.GhostCoordsAndType[i][0], spawnData.GhostCoordsAndType[i][1], spawnData.GhostCoordsAndType[i][2])));
            }
        }

        public static void Shuffle(Random random, float[][] arr)
        {
            Random rnd = new Random();
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
    [Serializable]
    public class SpawnData
    { 
        public float ReaperSpawnIntensity = 1;
        public float GhostSpawnIntensity = 1;
        public bool AlwaysRandomized = false;
        public float[][] ReaperCoords =
        {
            new float[]{ 120, -40, -568 },
            new float[]{ 1407, -190, 584 },
            new float[]{ 278, -175, 1398 },
            new float[]{ -811, -240, -1240 },
            new float[]{ -442, -132, -912 },
            new float[]{ -310, -45, 92 },
            new float[]{ 190, -52, 477 },
            new float[]{ 500, -98, 318 },
            new float[]{ 680, -85, 331 },
            new float[]{ -172, -70, -781 },
            new float[]{ -692, -130, -725 },
            new float[]{ -745, -80, -1050 },
            new float[]{ -278, -30, -621 },
            new float[]{ -295, -45, -350 },
            new float[]{ -516, -110, 531 },
            new float[]{ -815, -68, 316 },
            new float[]{ -531, -60, -175 },
            new float[]{ -250, -142, 906 },
            new float[]{ -1122, -113, 710 },
            new float[]{ -1190, -102, -527 },
            new float[]{ -754, -102, 1334 },
            new float[]{ 432, -65, 690 },
            new float[]{ 383, -60, 40 }
        };
        public float[][] GhostCoordsAndType =
        {
            new float[]{ -284, -293, 1100, 2 },
            new float[]{ 1065, -211, 466, 2 },
            new float[]{ 876, -122, 881, 2 },
            new float[]{ -28, -318, 1296, 2 },
            new float[]{ 10, -219, -220, 3 },
            new float[]{ -396, -350, -925, 3 },
            new float[]{ -958, -300, -540, 3 },
            new float[]{ -988, -885, 400, 3 },
            new float[]{ -695, -478, -993, 3 },
            new float[]{ -618, -213, -82, 3 },
            new float[]{ -34, -400, 926, 3 },
            new float[]{ -196, -436, 1056, 3 },
            new float[]{ 1443, -260, 883, 3 },
            new float[]{ 1075, -475, 944, 3 }
        };
    }
}