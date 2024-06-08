using HarmonyLib;
using Nautilus.Handlers;
using static MoreLeviathanSpawns.MoreLeviathanSpawnsPlugin_SN;
using System.Collections.Generic;
using UnityEngine;

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
                if (!(saveCoords.ReaperCoords is null))
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
                    ReaperCoords reaperCoord = saveCoords.ReaperCoords[i];

                    logger.LogInfo($"Spawning Reaper spawn #{i + 1} (Index {reaperCoord.ListIndex + 1}) - Coords: {reaperCoord.Coord}");
                    CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.ReaperLeviathan, reaperCoord.Coord));
                }
            }

            //Register ghost spawns from selected list of coordinates with the CoordinatedSpawnsHandler
            if (saveCoords.GhostSpawnIntensity != 0)
            {
                for (int i = 0; i < saveCoords.GhostCoords.Count; i++)
                {
                    GhostCoords ghostCoord = saveCoords.GhostCoords[i];

                    //Determine whether ghost to spawn is an adult or juvenile; defaults to adult
                    string ghostType = "Adult";
                    TechType ghostTechType = TechType.GhostLeviathan;
                    if (ghostCoord.GhostType == 2)
                    {
                        ghostType = "Juvenile";
                        ghostTechType = TechType.GhostLeviathanJuvenile;
                    }

                    logger.LogInfo($"Spawning Ghost {ghostType} spawn #{i + 1} (Index {ghostCoord.ListIndex + 1}) - Coords: {ghostCoord.Coord}");
                    CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(ghostTechType, ghostCoord.Coord));
                }
            }
        }
    }

    public class ReaperCoords
    {
        public Vector3 Coord { get; set; }
        public int ListIndex = -1;

        //DEBUG!! Used to convert to class ReaperCoords from Vector3 ReaperCoords (will be removed next update)
        public float X;
        public float Y;
        public float Z;
    }

    public class GhostCoords
    {
        public Vector3 Coord { get; set; }
        //1 for "Adult", 2 for "Juvenile"
        public int GhostType { get; set; }
        public int ListIndex = -1;
    }

    public class SpawnData
    {
        //23 Reaper Coordinates
        public List<ReaperCoords> ReaperCoords = new List<ReaperCoords>
        {
            new ReaperCoords { Coord = new Vector3( 120, -40, -568 ), ListIndex = 0 }, //Grassy Plateaus, South
            new ReaperCoords { Coord = new Vector3( 1407, -190, 584 ), ListIndex = 1 }, //Bulb Zone, East-North-East
            new ReaperCoords { Coord = new Vector3( 278, -175, 1398 ), ListIndex = 2 }, //Mountains, North
            new ReaperCoords { Coord = new Vector3( -811, -240, -1240 ), ListIndex = 3 }, //Grand Reef, South-South-West
            new ReaperCoords { Coord = new Vector3( -442, -132, -912 ), ListIndex = 4 }, //Grand Reef, South
            new ReaperCoords { Coord = new Vector3( -310, -45, 92 ), ListIndex = 5 }, //Kelp Forest, West
            new ReaperCoords { Coord = new Vector3( 190, -52, 477 ), ListIndex = 6 }, //Kelp Forest, North
            new ReaperCoords { Coord = new Vector3( 500, -98, 318 ), ListIndex = 7 }, //Mushroom Forest, East
            new ReaperCoords { Coord = new Vector3( 680, -85, 331 ), ListIndex = 8 }, //Mushroom Forest, East
            new ReaperCoords { Coord = new Vector3( -172, -70, -781 ), ListIndex = 9 }, //Crag Field, South
            new ReaperCoords { Coord = new Vector3( -692, -130, -725 ), ListIndex = 10 }, //Sparse Reef, South-West
            new ReaperCoords { Coord = new Vector3( -745, -80, -1050 ), ListIndex = 11 }, //Grand Reef, South-South-West (Under Floating Island)
            new ReaperCoords { Coord = new Vector3( -278, -30, -621 ), ListIndex = 12 }, //Kelp Forest, South
            new ReaperCoords { Coord = new Vector3( -295, -45, -350 ), ListIndex = 13 }, //Grassy Plateaus, South-West
            new ReaperCoords { Coord = new Vector3( -516, -110, 531 ), ListIndex = 14 }, //Mushroom Forest, North-West
            new ReaperCoords { Coord = new Vector3( -815, -68, 316 ), ListIndex = 15 }, //Mushroom Forest, West-North-West
            new ReaperCoords { Coord = new Vector3( -531, -60, -175 ), ListIndex = 16 }, //Grassy Plateaus, West
            new ReaperCoords { Coord = new Vector3( -250, -142, 906 ), ListIndex = 17 }, //Underwater Islands, North
            new ReaperCoords { Coord = new Vector3( -1122, -113, 710 ), ListIndex = 18 }, //Mushroom Forest, North-West
            new ReaperCoords { Coord = new Vector3( -1190, -102, -527 ), ListIndex = 19 }, //Sea Treader's Path, West-South-West
            new ReaperCoords { Coord = new Vector3( -754, -102, 1334 ), ListIndex = 20 }, //Blood Kelp Zone, North-North-West
            new ReaperCoords { Coord = new Vector3( 432, -65, 690 ), ListIndex = 21 }, //Kelp Forest, North-North-East
            new ReaperCoords { Coord = new Vector3( 383, -60, 40 ), ListIndex = 22 } //Grassy Plateaus, East
        };
        //14 Ghost Coordinates
        public List<GhostCoords> GhostCoords = new List<GhostCoords>
        {
            new GhostCoords { Coord = new Vector3( -284, -293, 1100 ), GhostType = 1, ListIndex = 0 }, //Adult, Underwater Islands, North
            new GhostCoords { Coord = new Vector3( 1065, -211, 466 ), GhostType = 1, ListIndex = 1 }, //Adult, Bulb Zone, East-North-East
            new GhostCoords { Coord = new Vector3( 876, -122, 881 ), GhostType = 1, ListIndex = 2 }, //Adult, Bulb Zone, North-East
            new GhostCoords { Coord = new Vector3( -28, -318, 1296 ), GhostType = 1, ListIndex = 3 }, //Adult, Mountains, North
            new GhostCoords { Coord = new Vector3( 10, -219, -220 ), GhostType = 2, ListIndex = 4 }, //Juvenile, Jellyshroom Cave, South
            new GhostCoords { Coord = new Vector3( -396, -350, -925 ), GhostType = 2, ListIndex = 5 }, //Juvenile, Grand Reef, South
            new GhostCoords { Coord = new Vector3( -958, -300, -540 ), GhostType = 2, ListIndex = 6 }, //Juvenile, Blood Kelp Trench, South-West
            new GhostCoords { Coord = new Vector3( -988, -885, 400 ), GhostType = 2, ListIndex = 7 }, //Juvenile, Lost River, West-North-West (Tree Cove)
            new GhostCoords { Coord = new Vector3( -695, -478, -993 ), GhostType = 2, ListIndex = 8 }, //Juvenile, Grand Reef, South-South-West (Degasi Base)
            new GhostCoords { Coord = new Vector3( -618, -213, -82 ), GhostType = 2, ListIndex = 9 }, //Juvenile, Jellyshroom Cave, West
            new GhostCoords { Coord = new Vector3( -34, -400, 926 ), GhostType = 2, ListIndex = 10 }, //Juvenile, Underwater Islands, North
            new GhostCoords { Coord = new Vector3( -196, -436, 1056 ), GhostType = 2, ListIndex = 11 }, //Juvenile, Underwater Islands, North
            new GhostCoords { Coord = new Vector3( 1443, -260, 883 ), GhostType = 2, ListIndex = 12 }, //Juvenile, Bulb Zone, North-East
            new GhostCoords { Coord = new Vector3( 1075, -475, 944 ), GhostType = 2, ListIndex = 13 } //Juvenile, Mountains, North-East (Lost River Entrance)
        };
    }
}
