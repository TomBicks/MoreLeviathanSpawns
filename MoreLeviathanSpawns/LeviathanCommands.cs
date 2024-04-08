using static MoreLeviathanSpawns.MoreLeviathanSpawnsPlugin_SN;
using Nautilus.Commands;

namespace MoreLeviathanSpawns
{
    public static class LeviathanCommands
    {
        [ConsoleCommand("coordwarp")]
        public static void CoordWarp(int index)
        {
            //Check first if the mod is enabled (if coord file populated) before running the command
            //If not even reapercoords was populated, neither will ghost, so this works as a test
            if (ModEnabled())
            {
                //Set total count of both types of leviathans for reference
                int reaper_count = saveCoords.ReaperCoords.Count;
                int ghost_count = saveCoords.GhostCoords.Count;

                //Attempt to warp to reaper or ghost leviathan, if attempted index is within the range of either of the totals of both leivathans
                //'1' to 'total number of reapers' = reaper warp, 'total numbers of reapers' + 1 to 'total number of reapers' + 'total number of ghosts' = ghost warp
                if (index >= 1 && index <= (reaper_count + ghost_count))
                {
                    if (index <= reaper_count)
                    {
                        ErrorMessage.AddMessage($"Teleporting to Reaper coord #{index} - Coords: {saveCoords.ReaperCoords[index - 1]}");
                        logger.LogInfo($"Teleporting to Reaper coord #{index} - Coords: {saveCoords.ReaperCoords[index - 1]}");
                        Player.main.SetPosition(saveCoords.ReaperCoords[index - 1].Coord);
                    }
                    else
                    {
                        ErrorMessage.AddMessage($"Teleporting to Ghost coord #{index - reaper_count} - Coords: {saveCoords.GhostCoords[index - reaper_count - 1].Coord}");
                        logger.LogInfo($"Teleporting to Ghost coord #{index - reaper_count} - Coords: {saveCoords.GhostCoords[index - reaper_count - 1].Coord}");
                        Player.main.SetPosition(saveCoords.GhostCoords[index - reaper_count - 1].Coord);
                    }
                }
                else
                {
                    ErrorMessage.AddMessage($"Index {index} out of bounds!");
                    logger.LogError($"Index {index} out of bounds!");
                }
            }
        }

        [ConsoleCommand("leviathantotal")]
        public static void LeviathanTotal()
        {
            //Check first if the mod is enabled (if coord file populated) before running the command
            if (ModEnabled())
            {
                var reaperTotal = saveCoords.ReaperCoords.Count;
                var ghostTotal = saveCoords.GhostCoords.Count;
                ErrorMessage.AddMessage($"{reaperTotal + ghostTotal} leviathans spawned\n{reaperTotal} Reapers spawned and {ghostTotal} Ghosts spawned");
                logger.LogInfo($"{reaperTotal + ghostTotal} leviathans spawned; {reaperTotal} Reapers spawned and {ghostTotal} Ghosts spawned");
            }
        }
    }
}
