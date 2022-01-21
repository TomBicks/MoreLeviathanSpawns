using HarmonyLib;
using SMLHelper.V2.Handlers;
using System;
using System.IO;
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
                CheckFile();
            }
        }

        static void CheckFile()
        {
            //var file = Directory.GetCurrentDirectory() + "/QMods/MoreReaperSpawns/spawns.txt";
            var file = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "MoreReaperSpawns");
            if (File.Exists(file))
            {
                // already initialized, return to prevent from spawn duplications.
                Logger.Log(Logger.Level.Info, "MoreReaperSpawns file was found");
                Logger.Log(Logger.Level.Info, $"file: {file}");
                return;
            }

            File.Create(file);
            Logger.Log(Logger.Level.Info, "MoreReaperSpawns initialised for the first time");
            spawnReapers();
        }

        static void spawnReapers()
        {
            //float[] floatingIslandsCoords = { -854f, -80f, -1138f };
            //Random number generator will spawn reapers within a min/max area so they're nevery exactly the same

            //Floating Islands Reaper
            float[] floatingIslandsCoords = {
                (float)new Random().Next(-1000, -500),
                (float)new Random().Next(-100, -60),
                (float)new Random().Next(-1300, -750)
            };
            Logger.Log(Logger.Level.Info, $"Floating Islands Reaper Coords: {floatingIslandsCoords[0]+ " " + floatingIslandsCoords[1] + " " + floatingIslandsCoords[2]}");
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(floatingIslandsCoords[0], floatingIslandsCoords[1], floatingIslandsCoords[2])));

            //Underwater Islands Reaper
            float[] underwaterIslandsCoords = {
                (float)new Random().Next(-385, 50),
                (float)new Random().Next(-150, -100),
                (float)new Random().Next(825, 1120)
            };
            Logger.Log(Logger.Level.Info, $"Underwater Islands Reaper Coords: {underwaterIslandsCoords[0] + " " + underwaterIslandsCoords[1] + " " + underwaterIslandsCoords[2]}");
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(underwaterIslandsCoords[0], underwaterIslandsCoords[1], underwaterIslandsCoords[2])));

            //Mushroom Forest Reaper
            float[] mushroomForestCoords = {
                (float)new Random().Next(-1250, -900),
                (float)new Random().Next(-100, -40),
                (float)new Random().Next(600, 900)
            };
            Logger.Log(Logger.Level.Info, $"Mushroom Forest Reaper Coords: {mushroomForestCoords[0] + " " + mushroomForestCoords[1] + " " + mushroomForestCoords[2]}");
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(mushroomForestCoords[0], mushroomForestCoords[1], mushroomForestCoords[2])));

            //Blood Kelp Reaper
            float[] bloodKelpCoords = {
                (float)new Random().Next(-1300, -1000),
                (float)new Random().Next(-120, -70),
                (float)new Random().Next(-600, -400)
            };
            Logger.Log(Logger.Level.Info, $"Blood Kelp Reaper Coords: {bloodKelpCoords[0] + " " + bloodKelpCoords[1] + " " + bloodKelpCoords[2]}");
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(bloodKelpCoords[0], bloodKelpCoords[1], bloodKelpCoords[2])));

            //Blood Kelp 2 Reaper
            float[] bloodKelp2Coords = {
                (float)new Random().Next(-900, -500),
                (float)new Random().Next(-120, -70),
                (float)new Random().Next(1180, 1600)
            };
            Logger.Log(Logger.Level.Info, $"Blood Kelp 2 Reaper Coords: {bloodKelp2Coords[0] + " " + bloodKelp2Coords[1] + " " + bloodKelp2Coords[2]}");
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(bloodKelp2Coords[0], bloodKelp2Coords[1], bloodKelp2Coords[2])));

            //Grassy Plateau Reaper
            float[] grassyPlateauCoords = {
                (float)new Random().Next(400, 640),
                (float)new Random().Next(-70, -40),
                (float)new Random().Next(150, 280)
            };
            Logger.Log(Logger.Level.Info, $"Grassy Plateau Reaper Coords: {grassyPlateauCoords[0] + " " + grassyPlateauCoords[1] + " " + grassyPlateauCoords[2]}");
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, new UnityEngine.Vector3(grassyPlateauCoords[0], grassyPlateauCoords[1], grassyPlateauCoords[2])));
        }
    }
}