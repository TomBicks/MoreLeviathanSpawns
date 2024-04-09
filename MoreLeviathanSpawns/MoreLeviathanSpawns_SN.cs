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

                    logger.LogInfo($"Spawning Reaper spawn #{i + 1} - Coords: {reaperCoord}");
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

                    logger.LogInfo($"Spawning Ghost {ghostType} spawn #{i + 1} - Coords: {ghostCoord.Coord}");
                    CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(ghostTechType, ghostCoord.Coord));
                }
            }
        }
    }

    public class ReaperCoords
    {
        public Vector3 Coord { get; set; }
        public int ListIndex { get; set; }
    }

    public class GhostCoords
    {
        public Vector3 Coord { get; set; }
        //1 for "Adult", 2 for "Juvenile"
        public int GhostType { get; set; }
        public int ListIndex { get; set; }
    }

    public class SpawnData
    {
        //23 Reaper Coordinates
        public List<ReaperCoords> ReaperCoords = new List<ReaperCoords>
        {
            new ReaperCoords { Coord = new Vector3( 120, -40, -568 ), ListIndex = 1 }, //Grassy Plateaus, South
            new ReaperCoords { Coord = new Vector3( 1407, -190, 584 ), ListIndex = 2 }, //Bulb Zone, East-North-East
            new ReaperCoords { Coord = new Vector3( 278, -175, 1398 ), ListIndex = 3 }, //Mountains, North
            new ReaperCoords { Coord = new Vector3( -811, -240, -1240 ), ListIndex = 4 }, //Grand Reef, South-South-West
            new ReaperCoords { Coord = new Vector3( -442, -132, -912 ), ListIndex = 5 }, //Grand Reef, South
            new ReaperCoords { Coord = new Vector3( -310, -45, 92 ), ListIndex = 6 }, //Kelp Forest, West
            new ReaperCoords { Coord = new Vector3( 190, -52, 477 ), ListIndex = 7 }, //Kelp Forest, North
            new ReaperCoords { Coord = new Vector3( 500, -98, 318 ), ListIndex = 8 }, //Mushroom Forest, East
            new ReaperCoords { Coord = new Vector3( 680, -85, 331 ), ListIndex = 9 }, //Mushroom Forest, East
            new ReaperCoords { Coord = new Vector3( -172, -70, -781 ), ListIndex = 10 }, //Crag Field, South
            new ReaperCoords { Coord = new Vector3( -692, -130, -725 ), ListIndex = 11 }, //Sparse Reef, South-West
            new ReaperCoords { Coord = new Vector3( -745, -80, -1050 ), ListIndex = 12 }, //Grand Reef, South-South-West (Under Floating Island)
            new ReaperCoords { Coord = new Vector3( -278, -30, -621 ), ListIndex = 13 }, //Kelp Forest, South
            new ReaperCoords { Coord = new Vector3( -295, -45, -350 ), ListIndex = 14 }, //Grassy Plateaus, South-West
            new ReaperCoords { Coord = new Vector3( -516, -110, 531 ), ListIndex = 15 }, //Mushroom Forest, North-West
            new ReaperCoords { Coord = new Vector3( -815, -68, 316 ), ListIndex = 16 }, //Mushroom Forest, West-North-West
            new ReaperCoords { Coord = new Vector3( -531, -60, -175 ), ListIndex = 17 }, //Grassy Plateaus, West
            new ReaperCoords { Coord = new Vector3( -250, -142, 906 ), ListIndex = 18 }, //Underwater Islands, North
            new ReaperCoords { Coord = new Vector3( -1122, -113, 710 ), ListIndex = 19 }, //Mushroom Forest, North-West
            new ReaperCoords { Coord = new Vector3( -1190, -102, -527 ), ListIndex = 20 }, //Sea Treader's Path, West-South-West
            new ReaperCoords { Coord = new Vector3( -754, -102, 1334 ), ListIndex = 21 }, //Blood Kelp Zone, North-North-West
            new ReaperCoords { Coord = new Vector3( 432, -65, 690 ), ListIndex = 22 }, //Kelp Forest, North-North-East
            new ReaperCoords { Coord = new Vector3( 383, -60, 40 ), ListIndex = 23 } //Grassy Plateaus, East
        };
        //14 Ghost Coordinates
        public List<GhostCoords> GhostCoords = new List<GhostCoords>
        {
            new GhostCoords { Coord = new Vector3( -284, -293, 1100 ), GhostType = 1, ListIndex = 1 }, //Adult, Underwater Islands, North
            new GhostCoords { Coord = new Vector3( 1065, -211, 466 ), GhostType = 1, ListIndex = 2 }, //Adult, Bulb Zone, East-North-East
            new GhostCoords { Coord = new Vector3( 876, -122, 881 ), GhostType = 1, ListIndex = 3 }, //Adult, Bulb Zone, North-East
            new GhostCoords { Coord = new Vector3( -28, -318, 1296 ), GhostType = 1, ListIndex = 4 }, //Adult, Mountains, North
            new GhostCoords { Coord = new Vector3( 10, -219, -220 ), GhostType = 2, ListIndex = 5 }, //Juvenile, Jellyshroom Cave, South
            new GhostCoords { Coord = new Vector3( -396, -350, -925 ), GhostType = 2, ListIndex = 6 }, //Juvenile, Grand Reef, South
            new GhostCoords { Coord = new Vector3( -958, -300, -540 ), GhostType = 2, ListIndex = 7 }, //Juvenile, Blood Kelp Trench, South-West
            new GhostCoords { Coord = new Vector3( -988, -885, 400 ), GhostType = 2, ListIndex = 8 }, //Juvenile, Lost River, West-North-West (Tree Cove)
            new GhostCoords { Coord = new Vector3( -695, -478, -993 ), GhostType = 2, ListIndex = 9 }, //Juvenile, Grand Reef, South-South-West (Degasi Base)
            new GhostCoords { Coord = new Vector3( -618, -213, -82 ), GhostType = 2, ListIndex = 10 }, //Juvenile, Jellyshroom Cave, West
            new GhostCoords { Coord = new Vector3( -34, -400, 926 ), GhostType = 2, ListIndex = 11 }, //Juvenile, Underwater Islands, North
            new GhostCoords { Coord = new Vector3( -196, -436, 1056 ), GhostType = 2, ListIndex = 12 }, //Juvenile, Underwater Islands, North
            new GhostCoords { Coord = new Vector3( 1443, -260, 883 ), GhostType = 2, ListIndex = 13 }, //Juvenile, Bulb Zone, North-East
            new GhostCoords { Coord = new Vector3( 1075, -475, 944 ), GhostType = 2, ListIndex = 14 } //Juvenile, Mountains, North-East (Lost River Entrance)
        };
    }

    public class LeviathanLocations
    {
        public readonly List<string> ReaperLocations = new List<string>
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
        public readonly List<string> GhostLocations = new List<string>
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
}
