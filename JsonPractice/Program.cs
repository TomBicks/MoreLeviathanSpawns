using Newtonsoft.Json;
using System;
using System.IO;

namespace JsonPractice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filepath = @"C:\Users\Brett\Documents";
            if (File.Exists(filepath))
            {
                saveSpawnJson(filepath);
            }
            else
            {
                loadSpawnJson(filepath);
            }
        }

        static void saveSpawnJson(string filepath)
        {
            Console.WriteLine("file doesn't exist. Creating json file...");
            //Random number generator will spawn reapers within a min/max area so they're never exactly the same
            //Useful resource for figuring out general coordinates: https://subnauticamap.io

            float[][] spawnCoords = new float[6][];

            //Floating Islands Reaper
            spawnCoords[0] = new float[3]{
                new Random().Next(-1000, -500),
                new Random().Next(-100, -60),
                new Random().Next(-1300, -750)
            };

            //Underwater Islands Reaper
            spawnCoords[1] = new float[3]{
                new Random().Next(-385, 50),
                new Random().Next(-150, -100),
                new Random().Next(825, 1120)
            };

            //Mushroom Forest Reaper
            spawnCoords[2] = new float[3]{
                new Random().Next(-1250, -900),
                new Random().Next(-100, -40),
                new Random().Next(600, 900)
            };

            //Blood Kelp Reaper
            spawnCoords[3] = new float[3]{
                new Random().Next(-1300, -1000),
                new Random().Next(-120, -70),
                new Random().Next(-600, -400)
            };

            //Blood Kelp 2 Reaper
            spawnCoords[4] = new float[3]{
                new Random().Next(-900, -500),
                new Random().Next(-120, -70),
                new Random().Next(1180, 1600)
            };

            //Grassy Plateau Reaper
            spawnCoords[5] = new float[3]{
                new Random().Next(400, 640),
                new Random().Next(-70, -40),
                new Random().Next(150, 280)
            };

            string json = JsonConvert.SerializeObject(spawnCoords);
            File.WriteAllText(filepath, json);

            Console.Write($"json file path: {filepath}");
        }

        static void loadSpawnJson(string filepath)
        {
            Console.WriteLine("file exists. Getting json file...");
            string jsonString = File.ReadAllText(filepath);
            float[][] spawnCoords = JsonConvert.DeserializeObject<float[][]>(jsonString);
            for (int i = 0; i < spawnCoords.Length; i++)
            {
                Logger.Log(Logger.Level.Info, $"Spawn #{i}: " + spawnCoords[i][0] + " " + spawnCoords[i][1] + " " + spawnCoords[i][2]);
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(spawnCoords[i][0], spawnCoords[i][1], spawnCoords[i][2])));
            }
        }
    }
}
