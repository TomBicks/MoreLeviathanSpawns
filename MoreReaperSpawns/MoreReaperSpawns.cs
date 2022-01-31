using HarmonyLib;
using SMLHelper.V2.Handlers;
using System;
using System.IO;
using System.Xml.Serialization;
using Logger = QModManager.Utility.Logger;

namespace MoreReaperSpawns
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake")]
    internal class SpawnMoreReapers
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
                    Logger.Log(Logger.Level.Info, "MoreReaperSpawns initialised for the first time. Creating XML file...");
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

            //Get Spawn Intensity and find out if the player wants the spawns to always be randomized on every time
            //the save file is loaded.
            spawnData.Intensity = QMod.Config.SpawnIntensity;
            spawnData.AlwaysRandomized = QMod.Config.AlwaysRandomized;

            //Shuffle 2D array of coordinates. Depending on how many spawns the player wants in the game (set via
            //the in-game mod menu), spawn in that many creatures starting from the top of the 2D array.
            Shuffle(new Random(), spawnData.CoordsAndType);

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

            Logger.Log(Logger.Level.Info, $"Spawn intensity is set to: {spawnData.Intensity}");
            //this will set a general amount of spawns based on the spawn intensity the player set, defaulting to '1'
            int loopSize = (int)((spawnData.CoordsAndType.Length / 5) * spawnData.Intensity);
            Logger.Log(Logger.Level.Info, $"This save file will load {loopSize} of {spawnData.CoordsAndType.Length} total spawns");

            //if player opted for spanws to always be random, simply shuffle the spawn coordinates 2D array,
            //defaulting to 'false'
            Logger.Log(Logger.Level.Info, $"Alway randomized is set to: {spawnData.AlwaysRandomized}");
            if (spawnData.AlwaysRandomized)
            {
                Logger.Log(Logger.Level.Info, $"shuffling (randomizing) spawns...");
                Shuffle(new Random(), spawnData.CoordsAndType);
            }

            for (int i = 0; i < loopSize; i++)
            {
                TechType creatureType = new TechType();
                string spawnType = "";
                switch (spawnData.CoordsAndType[i][3])
                {
                    case 1://Reaper Leviathan
                        creatureType = TechType.ReaperLeviathan;
                        spawnType = "Reaper Leviathan";
                        break;
                    case 2://Ghost Leviathan
                        creatureType = TechType.GhostLeviathan;
                        spawnType = "Ghost Leviathan";
                        break;
                }
                Logger.Log(Logger.Level.Info, $"Spawn #{i} - Type:{spawnType} - Coords: {spawnData.CoordsAndType[i][0]} {spawnData.CoordsAndType[i][1]} {spawnData.CoordsAndType[i][2]}");
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(creatureType, new UnityEngine.Vector3(spawnData.CoordsAndType[i][0], spawnData.CoordsAndType[i][1], spawnData.CoordsAndType[i][2])));
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
        public float Intensity = 1;
        public bool AlwaysRandomized = false;
        public float[][] CoordsAndType =
        {
            new float[]{ 120, -40, -568, 1 },
            new float[]{ 1407, -190, 584, 1 },
            new float[]{ 278, -175, 1398, 1 },
            new float[]{ -811, -240, -1240, 1 },
            new float[]{ -442, -132, -912, 1 },
            new float[]{ -310, -45, 92, 1 },
            new float[]{ 190, -52, 477, 1 },
            new float[]{ 500, -98, 318, 1 },
            new float[]{ 680, -85, 331, 1 },
            new float[]{ -172, -70, -781, 1 },
            new float[]{ -692, -130, -725, 1 },
            new float[]{ -745, -80, -1050, 1 },
            new float[]{ -278, -30, -621, 1 },
            new float[]{ -295, -45, -350, 1 },
            new float[]{ -516, -110, 531, 1 },
            new float[]{ -815, -68, 316, 1 },
            new float[]{ -531, -60, -175, 1 },
            new float[]{ -250, -142, 906, 1 },
            new float[]{ -1122, -113, 710, 1 },
            new float[]{ -1190, -102, -527, 1 },
            new float[]{ -754, -102, 1334, 1 },
            new float[]{ -396, -350, -925, 2 },
            new float[]{ -196, -436, 1056, 2 },
            new float[]{ -34, -400, 926, 2 },
            new float[]{ -284, -293, 1100, 2 },
            new float[]{ -958, -300, -540, 2 },
            new float[]{ 37, -228, -211, 2 },
            new float[]{ -618, -213, -82, 2 },
            new float[]{ -695, -478, -993, 2 },
            new float[]{ -988, -885, 400, 2 },
            new float[]{ 1065, -211, 466, 2 },
            new float[]{ 1443, -292, 883, 2 },
            new float[]{ 1075, -486, 944, 2 },
            new float[]{ 876, -122, 881, 2 },
            new float[]{ -28, -318, 1296, 2 }
        };
    }
}