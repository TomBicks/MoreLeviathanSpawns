using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                //string filepath = @"C:\Users\Brett\Documents\spawnData.json";
                Logger.Log(Logger.Level.Info, "Save file location" + SaveLoadManager.GetTemporarySavePath());

                if (!File.Exists(filepath))
                {
                    Logger.Log(Logger.Level.Info, "MoreReaperSpawns initialised for the first time. Creating spawns and saving json...");
                    SeedCreatureCoordsAndSaveJSON(filepath);
                } else
                {
                    Logger.Log(Logger.Level.Info, "Json found. Loading spawns from json file...");
                }
                SpawnCreatures(filepath);
            }
        }

        static void SeedCreatureCoordsAndSaveJSON(string filepath)
        {
            //Random number generator will spawn reapers within a min/max area so they're never exactly the same
            //Useful resource for figuring out general coordinates: https://subnauticamap.io

            SpawnCoords spawnCoords = new SpawnCoords();

            //Floating Islands Reaper
            spawnCoords.Coords[0] = new float[3]{
                new Random().Next(-1000, -500),
                new Random().Next(-100, -60),
                new Random().Next(-1300, -750)
            };

            //Underwater Islands Reaper
            spawnCoords.Coords[1] = new float[3]{
                new Random().Next(-385, 50),
                new Random().Next(-150, -100),
                new Random().Next(825, 1120)
            };

            //Mushroom Forest Reaper
            spawnCoords.Coords[2] = new float[3]{
                new Random().Next(-1250, -900),
                new Random().Next(-100, -40),
                new Random().Next(600, 900)
            };

            //Blood Kelp Reaper
            spawnCoords.Coords[3] = new float[3]{
                new Random().Next(-1300, -1000),
                new Random().Next(-120, -70),
                new Random().Next(-600, -400)
            };

            //Blood Kelp 2 Reaper
            spawnCoords.Coords[4] = new float[3]{
                new Random().Next(-900, -500),
                new Random().Next(-120, -70),
                new Random().Next(1180, 1600)
            };

            //Grassy Plateau Reaper
            spawnCoords.Coords[5] = new float[3]{
                new Random().Next(400, 640),
                new Random().Next(-70, -40),
                new Random().Next(150, 280)
            };

            XmlSerializer writer = new XmlSerializer(typeof(SpawnCoords));
            FileStream file = File.Create(filepath);
            writer.Serialize(file, spawnCoords);
            file.Close();
            Logger.Log(Logger.Level.Info, $"xml file path: {filepath}");
        }

        static void SpawnCreatures(string filepath)
        {
            XmlSerializer reader = new XmlSerializer(typeof(SpawnCoords));
            StreamReader file = new StreamReader(filepath);
            SpawnCoords spawnCoords = (SpawnCoords) reader.Deserialize(file);
            file.Close();
            for (int i = 0; i < spawnCoords.Coords.Length; i++)
            {
                Logger.Log(Logger.Level.Info, $"Spawn #{i}: " + spawnCoords.Coords[i][0] + " " + spawnCoords.Coords[i][1] + " " + spawnCoords.Coords[i][2]);
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(spawnCoords.Coords[i][0], spawnCoords.Coords[i][1], spawnCoords.Coords[i][2])));
            }
        }
    }
    [Serializable]
    public class SpawnCoords
    {
        public float[][] Coords = new float[6][];
    }
}