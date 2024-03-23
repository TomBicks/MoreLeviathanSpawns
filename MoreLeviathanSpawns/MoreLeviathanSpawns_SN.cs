using HarmonyLib;
//using System;
//using System.IO;
using BepInEx.Logging;
using Nautilus.Json;
using Nautilus.Handlers;
using static MoreLeviathanSpawns.MoreLeviathanSpawnsPlugin_SN;
using System.Collections.Generic;
using UnityEngine;
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
                //If mod is enabled
                if(!(saveCoords.ReaperCoords is null))
                {
                    //Spawn in all leviathans from the Coord File
                    SpawnLeviathans();
                }
            }
        }

        static void SpawnLeviathans()
        {
            logger.LogInfo($"Loading spawns from filepath: {saveCoords.JsonFilePath}");
            logger.LogInfo($"Loading {saveCoords.ReaperCoords.Count} Reaper spawns");
            logger.LogInfo($"Loading {saveCoords.GhostCoords.Count} Ghost spawns");

            //Register reaper spawns from selected list of coordinates with the CoordinatedSpawnsHandler
            if (saveCoords.ReaperSpawnIntensity != 0)
            {
                for (int i = 0; i < saveCoords.ReaperCoords.Count; i++)
                {
                    Vector3 __reaperCoord = saveCoords.ReaperCoords[i];

                    logger.LogInfo($"Spawning Reaper spawn #{i + 1} - Coords: {__reaperCoord}");
                    CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, __reaperCoord));
                }
            }

            //Register ghost spawns from selected list of coordinates with the CoordinatedSpawnsHandler
            if (saveCoords.GhostSpawnIntensity != 0)
            {
                for (int i = 0; i < saveCoords.GhostCoords.Count; i++)
                {
                    GhostCoordsAndType __ghostCoord = saveCoords.GhostCoords[i];

                    //Determine whether ghost to spawn is an adult or juvenile; defaults to adult
                    string __ghostType = "Adult";
                    TechType __ghostTechType = TechType.GhostLeviathan;
                    if (__ghostCoord.GhostType == 2)
                    {
                        __ghostType = "Juvenile";
                        __ghostTechType = TechType.GhostLeviathanJuvenile;
                    }

                    logger.LogInfo($"Spawning Ghost {__ghostType} spawn #{i + 1} - Coords: {__ghostCoord.Coord}");
                    CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(__ghostTechType, __ghostCoord.Coord));
                }
            }
        }
    }

    public class GhostCoordsAndType
    {
        public UnityEngine.Vector3 Coord { get; set; }
        //1 for "Adult", 2 for "Juvenile"
        public int GhostType { get; set; }
    }

    public class SpawnData
    {
        //23 Reaper Coordinates
        public List<Vector3> ReaperCoords = new List<Vector3>
        {
            new Vector3( 120, -40, -568 ), //Grassy Plateaus, South
            new Vector3( 1407, -190, 584 ), //Bulb Zone, East-North-East
            new Vector3( 278, -175, 1398 ), //Mountains, North
            new Vector3( -811, -240, -1240 ), //Grand Reef, South-South-West
            new Vector3( -442, -132, -912 ), //Grand Reef, South
            new Vector3( -310, -45, 92 ), //Kelp Forest, West
            new Vector3( 190, -52, 477 ), //Kelp Forest, North
            new Vector3( 500, -98, 318 ), //Mushroom Forest, East
            new Vector3( 680, -85, 331 ), //Mushroom Forest, East
            new Vector3( -172, -70, -781 ), //Crag Field, South
            new Vector3( -692, -130, -725 ), //Sparse Reef, South-West
            new Vector3( -745, -80, -1050 ), //Grand Reef, South-South-West (Under Floating Island)
            new Vector3( -278, -30, -621 ), //Kelp Forest, South
            new Vector3( -295, -45, -350 ), //Grassy Plateaus, South-West
            new Vector3( -516, -110, 531 ), //Mushroom Forest, North-West
            new Vector3( -815, -68, 316 ), //Mushroom Forest, West-North-West
            new Vector3( -531, -60, -175 ), //Grassy Plateaus, West
            new Vector3( -250, -142, 906 ), //Underwater Islands, North
            new Vector3( -1122, -113, 710 ), //Mushroom Forest, North-West
            new Vector3( -1190, -102, -527 ), //Sea Treader's Path, West-South-West
            new Vector3( -754, -102, 1334 ), //Blood Kelp Zone, North-North-West
            new Vector3( 432, -65, 690 ), //Kelp Forest, North-North-East
            new Vector3( 383, -60, 40 ) //Grassy Plateaus, East
        };
        //14 Ghost Coordinates
        public List<GhostCoordsAndType> GhostCoords = new List<GhostCoordsAndType>
        {
            //NOTE!! Should I be using "new int 1"?
            new GhostCoordsAndType { Coord = new Vector3( -284, -293, 1100 ), GhostType = 1 }, //Adult, Underwater Islands, North
            new GhostCoordsAndType { Coord = new Vector3( 1065, -211, 466 ), GhostType = 1 }, //Adult, Bulb Zone, East-North-East
            new GhostCoordsAndType { Coord = new Vector3( 876, -122, 881 ), GhostType = 1 }, //Adult, Bulb Zone, North-East
            new GhostCoordsAndType { Coord = new Vector3( -28, -318, 1296 ), GhostType = 1 }, //Adult, Mountains, North
            new GhostCoordsAndType { Coord = new Vector3( 10, -219, -220 ), GhostType = 2 }, //Juvenile, Jellyshroom Cave, South
            new GhostCoordsAndType { Coord = new Vector3( -396, -350, -925 ), GhostType = 2 }, //Juvenile, Grand Reef, South
            new GhostCoordsAndType { Coord = new Vector3( -958, -300, -540 ), GhostType = 2 }, //Juvenile, Blood Kelp Trench, South-West
            new GhostCoordsAndType { Coord = new Vector3( -988, -885, 400 ), GhostType = 2 }, //Juvenile, Lost River, West-North-West (Tree Cove)
            new GhostCoordsAndType { Coord = new Vector3( -695, -478, -993 ), GhostType = 2 }, //Juvenile, Grand Reef, South-South-West (Degasi Base)
            new GhostCoordsAndType { Coord = new Vector3( -618, -213, -82 ), GhostType = 2 }, //Juvenile, Jellyshroom Cave, West
            new GhostCoordsAndType { Coord = new Vector3( -34, -400, 926 ), GhostType = 2 }, //Juvenile, Underwater Islands, North
            new GhostCoordsAndType { Coord = new Vector3( -196, -436, 1056 ), GhostType = 2 }, //Juvenile, Underwater Islands, North
            new GhostCoordsAndType { Coord = new Vector3( 1443, -260, 883 ), GhostType = 2 }, //Juvenile, Bulb Zone, North-East
            new GhostCoordsAndType { Coord = new Vector3( 1075, -475, 944 ), GhostType = 2 } //Juvenile, Mountains, North-East (Lost River Entrance)
        };
    }
}
